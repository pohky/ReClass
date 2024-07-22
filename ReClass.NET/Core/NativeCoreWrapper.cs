using System.Runtime.InteropServices;
using ReClassNET.Extensions;
using ReClassNET.Native;
using ReClassNET.Native.Imports;

namespace ReClassNET.Core;

public class NativeCoreWrapper : ICoreProcessFunctions {
    public NativeCoreWrapper(IntPtr handle) {
        if (handle.IsNull()) {
            throw new ArgumentNullException();
        }

        enumerateProcessesDelegate = GetFunctionDelegate<EnumerateProcessesDelegate>(handle, "EnumerateProcesses");
        enumerateRemoteSectionsAndModulesDelegate = GetFunctionDelegate<EnumerateRemoteSectionsAndModulesDelegate>(handle, "EnumerateRemoteSectionsAndModules");
        openRemoteProcessDelegate = GetFunctionDelegate<OpenRemoteProcessDelegate>(handle, "OpenRemoteProcess");
        isProcessValidDelegate = GetFunctionDelegate<IsProcessValidDelegate>(handle, "IsProcessValid");
        closeRemoteProcessDelegate = GetFunctionDelegate<CloseRemoteProcessDelegate>(handle, "CloseRemoteProcess");
        readRemoteMemoryDelegate = GetFunctionDelegate<ReadRemoteMemoryDelegate>(handle, "ReadRemoteMemory");
        writeRemoteMemoryDelegate = GetFunctionDelegate<WriteRemoteMemoryDelegate>(handle, "WriteRemoteMemory");
        controlRemoteProcessDelegate = GetFunctionDelegate<ControlRemoteProcessDelegate>(handle, "ControlRemoteProcess");
    }

    public void EnumerateProcesses(EnumerateProcessCallback callbackProcess) {
        enumerateProcessesDelegate(callbackProcess);
    }

    public void EnumerateRemoteSectionsAndModules(IntPtr process, EnumerateRemoteSectionCallback callbackSection, EnumerateRemoteModuleCallback callbackModule) {
        enumerateRemoteSectionsAndModulesDelegate(process, callbackSection, callbackModule);
    }

    public IntPtr OpenRemoteProcess(IntPtr pid, ProcessAccess desiredAccess) => openRemoteProcessDelegate(pid, desiredAccess);

    public bool IsProcessValid(nint process) {
        if (process == nint.Zero) {
            return false;
        }

        var retn = Kernel32.WaitForSingleObject(process, 0);
        if (retn == 0xFFFFFFFF) { // WAIT_FAILED
            return false;
        }

        return retn == 0x00000102L; // WAIT_TIMEOUT
    }

    public void CloseRemoteProcess(IntPtr process) {
        closeRemoteProcessDelegate(process);
    }

    public bool ReadRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size) => readRemoteMemoryDelegate(process, address, buffer, offset, size);

    public bool WriteRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size) => writeRemoteMemoryDelegate(process, address, buffer, offset, size);

    public void ControlRemoteProcess(IntPtr process, ControlRemoteProcessAction action) {
        controlRemoteProcessDelegate(process, action);
    }

    protected static TDelegate GetFunctionDelegate<TDelegate>(IntPtr handle, string function) {
        var address = NativeMethods.GetProcAddress(handle, function);
        if (address.IsNull()) {
            throw new Exception($"Function '{function}' not found.");
        }
        return Marshal.GetDelegateForFunctionPointer<TDelegate>(address);
    }

    #region Native Delegates

    private delegate void EnumerateProcessesDelegate([MarshalAs(UnmanagedType.FunctionPtr)] EnumerateProcessCallback callbackProcess);

    private delegate void EnumerateRemoteSectionsAndModulesDelegate(IntPtr process, [MarshalAs(UnmanagedType.FunctionPtr)] EnumerateRemoteSectionCallback callbackSection, [MarshalAs(UnmanagedType.FunctionPtr)] EnumerateRemoteModuleCallback callbackModule);

    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool IsProcessValidDelegate(IntPtr process);

    private delegate IntPtr OpenRemoteProcessDelegate(IntPtr pid, ProcessAccess desiredAccess);

    private delegate void CloseRemoteProcessDelegate(IntPtr process);

    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool ReadRemoteMemoryDelegate(IntPtr process, IntPtr address, [Out] byte[] buffer, int offset, int size);

    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool WriteRemoteMemoryDelegate(IntPtr process, IntPtr address, [In] byte[] buffer, int offset, int size);

    private delegate void ControlRemoteProcessDelegate(IntPtr process, ControlRemoteProcessAction action);

    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool AttachDebuggerToProcessDelegate(IntPtr id);

    private delegate void DetachDebuggerFromProcessDelegate(IntPtr id);

    private readonly EnumerateProcessesDelegate enumerateProcessesDelegate;
    private readonly EnumerateRemoteSectionsAndModulesDelegate enumerateRemoteSectionsAndModulesDelegate;
    private readonly OpenRemoteProcessDelegate openRemoteProcessDelegate;
    private readonly IsProcessValidDelegate isProcessValidDelegate;
    private readonly CloseRemoteProcessDelegate closeRemoteProcessDelegate;
    private readonly ReadRemoteMemoryDelegate readRemoteMemoryDelegate;
    private readonly WriteRemoteMemoryDelegate writeRemoteMemoryDelegate;
    private readonly ControlRemoteProcessDelegate controlRemoteProcessDelegate;

    #endregion

}
