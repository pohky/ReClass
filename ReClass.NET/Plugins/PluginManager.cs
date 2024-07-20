using System.Diagnostics;
using ReClassNET.CodeGenerator;
using ReClassNET.Core;
using ReClassNET.DataExchange.ReClass;
using ReClassNET.Extensions;
using ReClassNET.Logger;
using ReClassNET.Native;
using ReClassNET.Nodes;
using ReClassNET.UI;

namespace ReClassNET.Plugins;

internal sealed class PluginManager {
    private readonly IPluginHost host;
    private readonly List<PluginInfo> plugins = [];

    public IEnumerable<PluginInfo> Plugins => plugins;

    public PluginManager(IPluginHost host) {
        this.host = host;
    }

    public void LoadAllPlugins(string path, ILogger logger) {
        try {
            if (!Directory.Exists(path)) {
                return;
            }

            var directory = new DirectoryInfo(path);

            LoadPlugins(directory.GetFiles("*.dll", SearchOption.AllDirectories), logger, true);

            LoadPlugins(directory.GetFiles("*.exe", SearchOption.AllDirectories), logger, true);

            LoadPlugins(directory.GetFiles("*.so", SearchOption.AllDirectories), logger, false);
        } catch (Exception ex) {
            logger.Log(ex);
        }
    }

    private void LoadPlugins(IEnumerable<FileInfo> files, ILogger logger, bool checkProductName) {
        // TODO: How to include plugin infos for unix files as they don't have embedded version info.




        foreach (var fi in files) {
            FileVersionInfo fvi;
            try {
                fvi = FileVersionInfo.GetVersionInfo(fi.FullName);

                if (checkProductName && fvi.ProductName != PluginInfo.PluginName && fvi.ProductName != PluginInfo.PluginNativeName) {
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

                    Program.CoreFunctions.RegisterFunctions(
                        pi.Name,
                        new NativeCoreWrapper(pi.NativeHandle)
                    );
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

        var handle = Activator.CreateInstanceFrom(filePath, type);

        if (!(handle.Unwrap() is Plugin plugin)) {
            throw new FileLoadException();
        }
        return plugin;
    }

    private static IntPtr CreateNativePluginInstance(string filePath) {
        var handle = NativeMethods.LoadLibrary(filePath);
        if (handle.IsNull()) {
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
