using ReClassNET.AddressParser;
using ReClassNET.Core;
using ReClassNET.Extensions;
using ReClassNET.Util.Conversion;

namespace ReClassNET.Memory;

public delegate void RemoteProcessEvent(RemoteProcess sender);

public class RemoteProcess : IDisposable, IRemoteMemoryReader, IRemoteMemoryWriter, IProcessReader {
    private readonly Dictionary<string, Func<RemoteProcess, IntPtr>> formulaCache = [];

    private readonly List<Module> modules = [];
    private readonly object processSync = new();

    private readonly Dictionary<IntPtr, string> rttiCache = [];

    private readonly List<Section> sections = [];

    private IntPtr handle;

    public CoreFunctionsManager CoreFunctions { get; }

    public ProcessInfo? UnderlayingProcess { get; private set; }

    /// <summary>Gets a copy of the current modules list. This list may change if the remote process (un)loads a module.</summary>
    public IEnumerable<Module> Modules {
        get {
            lock (modules) {
                return new List<Module>(modules);
            }
        }
    }

    /// <summary>Gets a copy of the current sections list. This list may change if the remote process (un)loads a section.</summary>
    public IEnumerable<Section> Sections {
        get {
            lock (sections) {
                return new List<Section>(sections);
            }
        }
    }

    /// <summary>A map of named addresses.</summary>
    public Dictionary<IntPtr, string> NamedAddresses { get; } = [];

    public bool IsValid => UnderlayingProcess != null && CoreFunctions.IsProcessValid(handle);

    public RemoteProcess(CoreFunctionsManager coreFunctions) {
        this.CoreFunctions = coreFunctions;
    }

    public void Dispose() {
        Close();
    }

    public Section? GetSectionToPointer(IntPtr address) {
        lock (sections) {
            var index = sections.BinarySearch(s => address.CompareToRange(s.Start, s.End));
            return index < 0 ? null : sections[index];
        }
    }

    public Module? GetModuleToPointer(IntPtr address) {
        lock (modules) {
            var index = modules.BinarySearch(m => address.CompareToRange(m.Start, m.End));
            return index < 0 ? null : modules[index];
        }
    }

    public Module? GetModuleByName(string name) {
        lock (modules) {
            return modules
                .FirstOrDefault(m => m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public bool EnumerateRemoteSectionsAndModules(out List<Section> _sections, out List<Module> _modules) {
        if (!IsValid) {
            _sections = null;
            _modules = null;

            return false;
        }

        _sections = [];
        _modules = [];

        CoreFunctions.EnumerateRemoteSectionsAndModules(handle, _sections.Add, _modules.Add);

        return true;
    }

    public EndianBitConverter BitConverter { get; set; } = EndianBitConverter.System;

    #region WriteMemory

    public bool WriteRemoteMemory(IntPtr address, byte[] data) {
        if (!IsValid) {
            return false;
        }

        return CoreFunctions.WriteRemoteMemory(handle, address, ref data, 0, data.Length);
    }

    #endregion

    /// <summary>Event which gets invoked when a process was opened.</summary>
    public event RemoteProcessEvent? ProcessAttached;

    /// <summary>Event which gets invoked before a process gets closed.</summary>
    public event RemoteProcessEvent? ProcessClosing;

    /// <summary>Event which gets invoked after a process was closed.</summary>
    public event RemoteProcessEvent? ProcessClosed;

    /// <summary>Opens the given process to gather informations from.</summary>
    /// <param name="info">The process information.</param>
    public void Open(ProcessInfo info) {
        if (UnderlayingProcess != info) {
            lock (processSync) {
                Close();

                rttiCache.Clear();

                UnderlayingProcess = info;

                handle = CoreFunctions.OpenRemoteProcess(UnderlayingProcess.Id, ProcessAccess.Full);
            }

            ProcessAttached?.Invoke(this);
        }
    }

    /// <summary>Closes the underlaying process. If the debugger is attached, it will automaticly detached.</summary>
    public void Close() {
        if (UnderlayingProcess != null) {
            ProcessClosing?.Invoke(this);

            lock (processSync) {
                CoreFunctions.CloseRemoteProcess(handle);

                handle = IntPtr.Zero;

                UnderlayingProcess = null;
            }

            ProcessClosed?.Invoke(this);
        }
    }

    /// <summary>Tries to map the given address to a section or a module of the process.</summary>
    /// <param name="address">The address to map.</param>
    /// <returns>The named address or null if no mapping exists.</returns>
    public string GetNamedAddress(IntPtr address) {
        if (NamedAddresses.TryGetValue(address, out var namedAddress)) {
            return namedAddress;
        }

        var section = GetSectionToPointer(address);
        if (section != null) {
            if (section.Category == SectionCategory.CODE || section.Category == SectionCategory.DATA) {
                // Code and Data sections belong to a module.
                return $"<{section.Category}>{section.ModuleName}.{address:X}";
            }
            if (section.Category == SectionCategory.HEAP) {
                return $"<HEAP>{address:X}";
            }
        }
        var module = GetModuleToPointer(address);
        if (module != null) {
            return $"{module.Name}.{address:X}";
        }
        return null;
    }

    /// <summary>Updates the process informations.</summary>
    public void UpdateProcessInformations() {
        UpdateProcessInformationsAsync().Wait();
    }

    /// <summary>Updates the process informations asynchronous.</summary>
    /// <returns>The Task.</returns>
    public Task UpdateProcessInformationsAsync() {
        if (!IsValid) {
            lock (modules) {
                modules.Clear();
            }
            lock (sections) {
                sections.Clear();
            }

            // TODO: Mono doesn't support Task.CompletedTask at the moment.
            //return Task.CompletedTask;
            return Task.FromResult(true);
        }

        return Task.Run(() => {
            EnumerateRemoteSectionsAndModules(out var newSections, out var newModules);

            newModules.Sort((m1, m2) => m1.Start.CompareTo(m2.Start));
            newSections.Sort((s1, s2) => s1.Start.CompareTo(s2.Start));

            lock (modules) {
                modules.Clear();
                modules.AddRange(newModules);
            }
            lock (sections) {
                sections.Clear();
                sections.AddRange(newSections);
            }
        });
    }

    /// <summary>Parse the address formula.</summary>
    /// <param name="addressFormula">The address formula.</param>
    /// <returns>The result of the parsed address or <see cref="IntPtr.Zero" />.</returns>
    public IntPtr ParseAddress(string addressFormula) {
        if (!formulaCache.TryGetValue(addressFormula, out var func)) {
            var expression = Parser.Parse(addressFormula);

            func = DynamicCompiler.CompileExpression(expression);

            formulaCache.Add(addressFormula, func);
        }

        return func(this);
    }

    public void ControlRemoteProcess(ControlRemoteProcessAction action) {
        if (!IsValid) {
            return;
        }

        CoreFunctions.ControlRemoteProcess(handle, action);
    }

    #region ReadMemory

    public bool ReadRemoteMemoryIntoBuffer(IntPtr address, ref byte[] buffer) {
        return ReadRemoteMemoryIntoBuffer(address, ref buffer, 0, buffer.Length);
    }

    public bool ReadRemoteMemoryIntoBuffer(IntPtr address, ref byte[] buffer, int offset, int length) {
        if (!IsValid) {
            Close();

            buffer.FillWithZero();

            return false;
        }

        return CoreFunctions.ReadRemoteMemory(handle, address, ref buffer, offset, length);
    }

    public byte[] ReadRemoteMemory(IntPtr address, int size) {
        var data = new byte[size];
        ReadRemoteMemoryIntoBuffer(address, ref data);
        return data;
    }

    #endregion

}
