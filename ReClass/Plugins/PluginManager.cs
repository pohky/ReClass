using System.Diagnostics;
using ReClass.CodeGenerator;
using ReClass.DataExchange.ReClass;
using ReClass.Logger;
using ReClass.Nodes;
using ReClass.UI;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace ReClass.Plugins;

internal sealed class PluginManager {
    private readonly IPluginHost host;
    private readonly List<PluginInfo> plugins = [];

    public IEnumerable<PluginInfo> Plugins => plugins;

    public PluginManager(IPluginHost host) {
        this.host = host;
    }

    public void LoadAllPlugins(string path, ILogger logger) {
        if (!Directory.Exists(path)) {
            return;
        }

        var directory = new DirectoryInfo(path);

        foreach (var fi in directory.GetFiles("*.dll", SearchOption.AllDirectories)) {
            FileVersionInfo fvi;
            try {
                fvi = FileVersionInfo.GetVersionInfo(fi.FullName);

                if (fvi.ProductName != PluginInfo.PluginName && fvi.ProductName != PluginInfo.PluginNativeName) {
                    continue;
                }
            } catch {
                continue;
            }

            try {
                var pi = new PluginInfo(fi.FullName, fvi);
                if (!pi.IsNative) {
                    pi.Interface = CreatePluginInstance(pi.FilePath);

                    if (!pi.Interface.Initialize(host)) {
                        continue;
                    }

                    RegisterNodeInfoReaders(pi);
                    RegisterCustomNodeTypes(pi);
                } else {
                    pi.NativeHandle = CreateNativePluginInstance(pi.FilePath);

                    // TODO: reimplement? what's this for?
                    /*
                    Program.CoreFunctions.RegisterFunctions(
                        pi.Name,
                        new NativeCoreWrapper(pi.NativeHandle)
                    );
                    */
                }

                plugins.Add(pi);
            } catch (Exception ex) {
                logger.Log(ex);
            }
        }
    }

    public void UnloadAllPlugins() {
        foreach (var pi in plugins) {
            if (pi.Interface != null) // Exclude native plugins
            {
                DeregisterNodeInfoReaders(pi);
                DeregisterCustomNodeTypes(pi);
            }

            pi.Dispose();
        }

        plugins.Clear();
    }

    private static Plugin CreatePluginInstance(string filePath) {
        var type = Path.GetFileNameWithoutExtension(filePath);
        type = type + "." + type + "Ext";

        var handle = (object?)Activator.CreateInstanceFrom(filePath, type);
        if (handle is not Plugin plugin) {
            throw new FileLoadException();
        }

        return plugin;
    }

    private static HMODULE CreateNativePluginInstance(string filePath) {
        var handle = PInvoke.LoadLibrary(filePath);
        if (handle.IsNull) {
            throw new FileLoadException($"Failed to load native plugin: {Path.GetFileName(filePath)}");
        }
        return handle;
    }

    private static void RegisterNodeInfoReaders(PluginInfo pluginInfo) {
        var nodeInfoReaders = pluginInfo.Interface.GetNodeInfoReaders();

        if (nodeInfoReaders == null || nodeInfoReaders.Count == 0) {
            return;
        }

        pluginInfo.NodeInfoReaders = nodeInfoReaders;

        BaseNode.NodeInfoReader.AddRange(nodeInfoReaders);
    }

    private static void DeregisterNodeInfoReaders(PluginInfo pluginInfo) {
        if (pluginInfo.NodeInfoReaders == null) {
            return;
        }

        foreach (var reader in pluginInfo.NodeInfoReaders) {
            BaseNode.NodeInfoReader.Remove(reader);
        }
    }

    private static void RegisterCustomNodeTypes(PluginInfo pluginInfo) {
        var customNodeTypes = pluginInfo.Interface.GetCustomNodeTypes();

        if (customNodeTypes == null) {
            return;
        }

        if (customNodeTypes.NodeTypes == null || customNodeTypes.Serializer == null || customNodeTypes.CodeGenerator == null) {
            throw new ArgumentException(); // TODO
        }

        foreach (var nodeType in customNodeTypes.NodeTypes) {
            if (!nodeType.IsSubclassOf(typeof(BaseNode))) {
                throw new ArgumentException($"Type '{nodeType}' is not a valid node.");
            }
        }

        pluginInfo.CustomNodeTypes = customNodeTypes;

        NodeTypesBuilder.AddPluginNodeGroup(pluginInfo.Interface, customNodeTypes.NodeTypes);

        CustomNodeSerializer.Add(customNodeTypes.Serializer);
        CppCodeGenerator.Add(customNodeTypes.CodeGenerator);
    }

    private static void DeregisterCustomNodeTypes(PluginInfo pluginInfo) {
        if (pluginInfo.CustomNodeTypes == null) {
            return;
        }

        NodeTypesBuilder.RemovePluginNodeGroup(pluginInfo.Interface);

        CustomNodeSerializer.Remove(pluginInfo.CustomNodeTypes.Serializer);
        CppCodeGenerator.Remove(pluginInfo.CustomNodeTypes.CodeGenerator);
    }
}
