namespace ReClassNET.Native;

internal interface INativeMethods {
    IntPtr LoadLibrary(string fileName);

    IntPtr GetProcAddress(IntPtr handle, string name);

    void FreeLibrary(IntPtr handle);

    Icon GetIconForFile(string path);

    void EnableDebugPrivileges();

    string UndecorateSymbolName(string name);

    void SetProcessDpiAwareness();

    bool RegisterExtension(string fileExtension, string extensionId, string applicationPath, string applicationName);

    void UnregisterExtension(string fileExtension, string extensionId);
}
