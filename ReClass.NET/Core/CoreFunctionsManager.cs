using ReClassNET.Extensions;
using ReClassNET.Memory;

namespace ReClassNET.Core;

public class CoreFunctionsManager : IDisposable {
    private readonly Dictionary<string, ICoreProcessFunctions> functionsRegistry = [];

    private readonly InternalCoreFunctions internalCoreFunctions;

    public IEnumerable<string> FunctionProviders => functionsRegistry.Keys;

    public ICoreProcessFunctions CurrentFunctions { get; private set; }

    public string CurrentFunctionsProvider => functionsRegistry
        .Where(kv => kv.Value == CurrentFunctions)
        .Select(kv => kv.Key)
        .FirstOrDefault();

    public CoreFunctionsManager() {
        internalCoreFunctions = InternalCoreFunctions.Create();

        RegisterFunctions("Default", internalCoreFunctions);

        CurrentFunctions = internalCoreFunctions;
    }

    #region IDisposable Support

    public void Dispose() {
        internalCoreFunctions.Dispose();
    }

    #endregion

    public void RegisterFunctions(string provider, ICoreProcessFunctions functions) {
        functionsRegistry.Add(provider, functions);
    }

    public void SetActiveFunctionsProvider(string provider) {
        if (!functionsRegistry.TryGetValue(provider, out var functions)) {
            throw new ArgumentException();
        }

        CurrentFunctions = functions;
    }

    #region Plugin Functions

    public void EnumerateProcesses(Action<ProcessInfo> callbackProcess) {
        var c = callbackProcess == null ? null : (EnumerateProcessCallback)delegate (ref EnumerateProcessData data) { callbackProcess(new ProcessInfo(data.Id, data.Name, data.Path)); };

        CurrentFunctions.EnumerateProcesses(c);
    }

    public IList<ProcessInfo> EnumerateProcesses() {
        var processes = new List<ProcessInfo>();
        EnumerateProcesses(processes.Add);
        return processes;
    }

    public void EnumerateRemoteSectionsAndModules(IntPtr process, Action<Section> callbackSection, Action<Module> callbackModule) {
        var c1 = callbackSection == null
            ? null
            : (EnumerateRemoteSectionCallback)delegate (ref EnumerateRemoteSectionData data) {
                callbackSection(new Section {
                    Start = data.BaseAddress,
                    End = data.BaseAddress.Add(data.Size),
                    Size = data.Size,
                    Name = data.Name,
                    Protection = data.Protection,
                    Type = data.Type,
                    ModulePath = data.ModulePath,
                    ModuleName = Path.GetFileName(data.ModulePath),
                    Category = data.Category
                });
            };

        var c2 = callbackModule == null
            ? null
            : (EnumerateRemoteModuleCallback)delegate (ref EnumerateRemoteModuleData data) {
                callbackModule(new Module {
                    Start = data.BaseAddress,
                    End = data.BaseAddress.Add(data.Size),
                    Size = data.Size,
                    Path = data.Path,
                    Name = Path.GetFileName(data.Path)
                });
            };

        CurrentFunctions.EnumerateRemoteSectionsAndModules(process, c1, c2);
    }

    public void EnumerateRemoteSectionsAndModules(IntPtr process, out List<Section> sections, out List<Module> modules) {
        sections = [];
        modules = [];

        EnumerateRemoteSectionsAndModules(process, sections.Add, modules.Add);
    }

    public IntPtr OpenRemoteProcess(IntPtr pid, ProcessAccess desiredAccess) => CurrentFunctions.OpenRemoteProcess(pid, desiredAccess);

    public bool IsProcessValid(IntPtr process) => CurrentFunctions.IsProcessValid(process);

    public void CloseRemoteProcess(IntPtr process) {
        CurrentFunctions.CloseRemoteProcess(process);
    }

    public bool ReadRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size) => CurrentFunctions.ReadRemoteMemory(process, address, ref buffer, offset, size);

    public bool WriteRemoteMemory(IntPtr process, IntPtr address, ref byte[] buffer, int offset, int size) => CurrentFunctions.WriteRemoteMemory(process, address, ref buffer, offset, size);

    public void ControlRemoteProcess(IntPtr process, ControlRemoteProcessAction action) {
        CurrentFunctions.ControlRemoteProcess(process, action);
    }

    public bool AttachDebuggerToProcess(IntPtr id) => CurrentFunctions.AttachDebuggerToProcess(id);

    public void DetachDebuggerFromProcess(IntPtr id) {
        CurrentFunctions.DetachDebuggerFromProcess(id);
    }

    #endregion

    #region Internal Core Functions

    public bool DisassembleCode(IntPtr address, int length, IntPtr virtualAddress, bool determineStaticInstructionBytes, EnumerateInstructionCallback callback) => internalCoreFunctions.DisassembleCode(address, length, virtualAddress, determineStaticInstructionBytes, callback);

    public IntPtr InitializeInput() => internalCoreFunctions.InitializeInput();

    public Keys[] GetPressedKeys(IntPtr handle) => internalCoreFunctions.GetPressedKeys(handle);

    public void ReleaseInput(IntPtr handle) {
        internalCoreFunctions.ReleaseInput(handle);
    }

    #endregion

}
