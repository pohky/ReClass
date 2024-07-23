using System.Diagnostics;
using System.Runtime.InteropServices;
using ReClass.AddressParser;
using ReClass.Extensions;
using ReClass.Native;
using ReClass.Util.Conversion;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.ToolHelp;
using Windows.Win32.System.Threading;

namespace ReClass.Memory;

public delegate void RemoteProcessEvent(RemoteProcess sender);

public enum ControlRemoteProcessAction {
    Suspend,
    Resume,
    Terminate
}

public class RemoteProcess : IDisposable, IRemoteMemoryReader, IRemoteMemoryWriter, IProcessReader {
    private readonly Dictionary<string, Func<RemoteProcess, IntPtr>> formulaCache = [];
    private readonly System.Timers.Timer updateTimer = new();

    public Process? UnderlayingProcess { get; private set; }
    public HANDLE Handle { get; private set; } = HANDLE.Null;
    public SectionInfo[] Sections { get; private set; } = [];
    public ModuleInfo[] Modules { get; private set; } = [];

    /// <summary>A map of named addresses.</summary>
    public Dictionary<IntPtr, string> NamedAddresses { get; } = [];

    public bool IsValid => NativeMethods.IsValidProcess(Handle);

    public void Dispose() {
        Close();
    }

    public unsafe SectionInfo? GetSectionToPointer(nint address) {
        var index = Sections.BinarySearch(s => address.CompareToRange(s.Start, s.End));
        return index < 0 ? null : Sections[index];
    }

    public ModuleInfo? GetModuleToPointer(nint address) {
        var index = Modules.BinarySearch(m => address.CompareToRange(m.BaseAddress, m.BaseAddress + (nint)m.Size));
        return index < 0 ? null : Modules[index];
    }

    public ModuleInfo? GetModuleByName(string name) {
        return Modules.FirstOrDefault(m => m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    public EndianBitConverter BitConverter { get; set; } = EndianBitConverter.System;

    #region WriteMemory

    public unsafe bool WriteRemoteMemory(nint address, byte[] data) {
        if (!IsValid) {
            return false;
        }

        fixed (byte* dataPtr = data)
            return PInvoke.WriteProcessMemory(Handle, (void*)address, dataPtr, 0);
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
    public void Open(Process process) {
        if (UnderlayingProcess == process) {
            return;
        }

        Close();

        UnderlayingProcess = process;
        Handle = NativeMethods.OpenProcess((uint)process.Id, ProcessAccess.Full);
        UpdateProcessInformation();

        updateTimer.Elapsed += (_, _) => UpdateProcessInformation();
        updateTimer.Interval = 5000;
        updateTimer.Start();

        ProcessAttached?.Invoke(this);
    }

    /// <summary>Closes the underlaying process. If the debugger is attached, it will automaticly detached.</summary>
    public void Close() {
        if (UnderlayingProcess == null)
            return;

        ProcessClosing?.Invoke(this);

        updateTimer.Stop();
        PInvoke.CloseHandle(Handle);
        UnderlayingProcess = null;
        Handle = HANDLE.Null;
        Sections = [];
        Modules = [];

        ProcessClosed?.Invoke(this);
    }

    public void UpdateProcessInformation() {
        Sections = NativeMethods.GetSections(Handle);
        Modules = NativeMethods.GetModules(Handle);
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

        return string.Empty;
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
        if (!IsValid || UnderlayingProcess == null) {
            return;
        }

        if (action == ControlRemoteProcessAction.Terminate) {
            PInvoke.TerminateProcess(Handle, 0);
            Close();
            return;
        }

        var snapshotHandle = PInvoke.CreateToolhelp32Snapshot(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPTHREAD, 0);
        if (snapshotHandle.IsNull)
            return;

        var te32 = new THREADENTRY32();
        te32.dwSize = (uint)Marshal.SizeOf(te32);

        if (PInvoke.Thread32First(snapshotHandle, ref te32)) {
            do {
                if (te32.th32OwnerProcessID == UnderlayingProcess.Id) {
                    var threadHandle = PInvoke.OpenThread(THREAD_ACCESS_RIGHTS.THREAD_SUSPEND_RESUME, false, te32.th32ThreadID);
                    if (!threadHandle.IsNull) {
                        switch (action) {
                            case ControlRemoteProcessAction.Suspend:
                                PInvoke.SuspendThread(threadHandle);
                                break;
                            case ControlRemoteProcessAction.Resume:
                                PInvoke.ResumeThread(threadHandle);
                                break;
                        }

                        PInvoke.CloseHandle(threadHandle);
                    }
                }
            } while (PInvoke.Thread32Next(snapshotHandle, ref te32));
        }

        PInvoke.CloseHandle(snapshotHandle);
    }

    #region ReadMemory

    public bool ReadRemoteMemoryIntoBuffer(IntPtr address, ref byte[] buffer) {
        return ReadRemoteMemoryIntoBuffer(address, ref buffer, 0, buffer.Length);
    }

    public unsafe bool ReadRemoteMemoryIntoBuffer(IntPtr address, ref byte[] buffer, int offset, int length) {
        buffer.FillWithZero();

        if (!IsValid) {
            Close();
            return false;
        }

        if (offset + length > buffer.Length) {
            return false;
        }

        var size = (nuint)length;
        nuint numberOfBytesRead;
        fixed (byte* bufferPtr = buffer) {
            if (PInvoke.ReadProcessMemory(Handle, (void*)address, bufferPtr + offset, size, &numberOfBytesRead) && size == numberOfBytesRead) {
                return true;
            }
        }

        return false;
    }

    public byte[] ReadRemoteMemory(IntPtr address, int size) {
        var data = new byte[size];
        ReadRemoteMemoryIntoBuffer(address, ref data);
        return data;
    }

    #endregion

}
