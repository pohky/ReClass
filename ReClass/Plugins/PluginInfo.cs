using System.Diagnostics;
using ReClass.Extensions;
using ReClass.Native;
using ReClass.Nodes;

namespace ReClass.Plugins;

internal class PluginInfo : IDisposable {
    public const string PluginName = "ReClass.NET Plugin";
    public const string PluginNativeName = "ReClass.NET Native Plugin";

    public string FilePath { get; }

    public string FileVersion { get; }

    public string Name { get; }

    public string Description { get; }

    public string Author { get; }

    public bool IsNative { get; }

    public Plugin Interface { get; set; }

    public IntPtr NativeHandle { get; set; }

    public IReadOnlyList<INodeInfoReader> NodeInfoReaders { get; set; }

    public Plugin.CustomNodeTypes CustomNodeTypes { get; set; }

    public PluginInfo(string filePath, FileVersionInfo versionInfo) {
        FilePath = filePath;
        IsNative = versionInfo.ProductName == null /* Unix */ || versionInfo.ProductName == PluginNativeName;

        FileVersion = (versionInfo.FileVersion ?? string.Empty).Trim();

        Description = (versionInfo.Comments ?? string.Empty).Trim();
        Author = (versionInfo.CompanyName ?? string.Empty).Trim();

        Name = (versionInfo.FileDescription ?? string.Empty).Trim();
        if (Name == string.Empty) {
            Name = Path.GetFileNameWithoutExtension(FilePath);
        }
    }

    public void Dispose() {
        if (Interface != null) {
            try {
                Interface.Terminate();
            } catch {
                // ignored
            }
        }

        ReleaseUnmanagedResources();

        GC.SuppressFinalize(this);
    }

    ~PluginInfo() {
        ReleaseUnmanagedResources();
    }

    private void ReleaseUnmanagedResources() {
        if (!NativeHandle.IsNull()) {
            NativeMethods.FreeLibrary(NativeHandle);

            NativeHandle = IntPtr.Zero;
        }
    }
}
