using System.Runtime.InteropServices;

namespace ReClassNET.Native;

internal class NativeMethodsUnix : INativeMethods {
    public IntPtr LoadLibrary(string fileName) => dlopen(fileName, RTLD_NOW);

    public IntPtr GetProcAddress(IntPtr handle, string name) =>
        // Warning: dlsym could return IntPtr.Zero to a valid function.
        // Error checking with dlerror is needed but we treat IntPtr.Zero as error value...
        dlsym(handle, name);

    public void FreeLibrary(IntPtr handle) {
        dlclose(handle);
    }

    public Icon GetIconForFile(string path) => null;

    public void EnableDebugPrivileges() {
    }

    public string UndecorateSymbolName(string name) => name;

    public void SetProcessDpiAwareness() {
    }

    public bool RegisterExtension(string fileExtension, string extensionId, string applicationPath, string applicationName) => false;

    public void UnregisterExtension(string fileExtension, string extensionId) {
    }

    #region Imports

    private const int RTLD_NOW = 2;

    [DllImport("__Internal")]
    private static extern IntPtr dlopen(string fileName, int flags);

    [DllImport("__Internal")]
    private static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("__Internal")]
    private static extern int dlclose(IntPtr handle);

    #endregion

}
