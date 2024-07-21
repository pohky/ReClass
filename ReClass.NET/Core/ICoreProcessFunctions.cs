namespace ReClassNET.Core;

public delegate void EnumerateProcessCallback(ref EnumerateProcessData data);

public delegate void EnumerateRemoteSectionCallback(ref EnumerateRemoteSectionData data);

public delegate void EnumerateRemoteModuleCallback(ref EnumerateRemoteModuleData data);

public interface ICoreProcessFunctions {
    void EnumerateProcesses(EnumerateProcessCallback callbackProcess);

    void EnumerateRemoteSectionsAndModules(IntPtr process, EnumerateRemoteSectionCallback callbackSection, EnumerateRemoteModuleCallback callbackModule);

    IntPtr OpenRemoteProcess(IntPtr pid, ProcessAccess desiredAccess);

    bool IsProcessValid(IntPtr process);

    void CloseRemoteProcess(IntPtr process);

    bool ReadRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size);

    bool WriteRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size);

    void ControlRemoteProcess(IntPtr process, ControlRemoteProcessAction action);

    bool AttachDebuggerToProcess(IntPtr id);

    void DetachDebuggerFromProcess(IntPtr id);
}
