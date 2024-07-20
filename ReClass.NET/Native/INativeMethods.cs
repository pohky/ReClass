using System.Diagnostics.Contracts;


/* Unmerged change from project 'ReClass.NET'
Before:
namespace ReClassNET.Native
{
    [ContractClass(typeof(NativeMethodsContract))]
    internal interface INativeMethods
    {
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

    [ContractClassFor(typeof(INativeMethods))]
    internal abstract class NativeMethodsContract : INativeMethods
    {
        public IntPtr LoadLibrary(string fileName)
        {
            throw new NotImplementedException();
        }

        public IntPtr GetProcAddress(IntPtr handle, string name)
        {
            throw new NotImplementedException();
        }

        public void FreeLibrary(IntPtr handle)
        {
            throw new NotImplementedException();
        }

        public Icon GetIconForFile(string path)
        {
            throw new NotImplementedException();
        }

        public void EnableDebugPrivileges()
        {
            throw new NotImplementedException();
        }

        public string UndecorateSymbolName(string name)
        {
            throw new NotImplementedException();
        }

        public void SetProcessDpiAwareness()
        {
            throw new NotImplementedException();
        }

        public bool RegisterExtension(string fileExtension, string extensionId, string applicationPath, string applicationName)
        {
            throw new NotImplementedException();
        }

        public void UnregisterExtension(string fileExtension, string extensionId)
        {
            throw new NotImplementedException();
        }
    }
}
After:
namespace ReClassNET.Native;

    [ContractClass(typeof(NativeMethodsContract))]
    internal interface INativeMethods
    {
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

    [ContractClassFor(typeof(INativeMethods))]
    internal abstract class NativeMethodsContract : INativeMethods
    {
        public IntPtr LoadLibrary(string fileName)
        {
            throw new NotImplementedException();
        }

        public IntPtr GetProcAddress(IntPtr handle, string name)
        {
            throw new NotImplementedException();
        }

        public void FreeLibrary(IntPtr handle)
        {
            throw new NotImplementedException();
        }

        public Icon GetIconForFile(string path)
        {
            throw new NotImplementedException();
        }

        public void EnableDebugPrivileges()
        {
            throw new NotImplementedException();
        }

        public string UndecorateSymbolName(string name)
        {
            throw new NotImplementedException();
        }

        public void SetProcessDpiAwareness()
        {
            throw new NotImplementedException();
        }

        public bool RegisterExtension(string fileExtension, string extensionId, string applicationPath, string applicationName)
        {
            throw new NotImplementedException();
        }

        public void UnregisterExtension(string fileExtension, string extensionId)
        {
            throw new NotImplementedException();
        }
    }
*/
namespace ReClassNET.Native;

[ContractClass(typeof(NativeMethodsContract))]
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

[ContractClassFor(typeof(INativeMethods))]
internal abstract class NativeMethodsContract : INativeMethods {
    public IntPtr LoadLibrary(string fileName) {
        throw new NotImplementedException();
    }

    public IntPtr GetProcAddress(IntPtr handle, string name) {
        throw new NotImplementedException();
    }

    public void FreeLibrary(IntPtr handle) {
        throw new NotImplementedException();
    }

    public Icon GetIconForFile(string path) {
        throw new NotImplementedException();
    }

    public void EnableDebugPrivileges() {
        throw new NotImplementedException();
    }

    public string UndecorateSymbolName(string name) {
        throw new NotImplementedException();
    }

    public void SetProcessDpiAwareness() {
        throw new NotImplementedException();
    }

    public bool RegisterExtension(string fileExtension, string extensionId, string applicationPath, string applicationName) {
        throw new NotImplementedException();
    }

    public void UnregisterExtension(string fileExtension, string extensionId) {
        throw new NotImplementedException();
    }
}
