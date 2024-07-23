using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.Debug;
using Windows.Win32.System.Diagnostics.ToolHelp;
using Windows.Win32.System.Memory;
using Windows.Win32.System.SystemServices;
using Windows.Win32.System.Threading;
using Windows.Win32.UI.Shell;

namespace ReClass.Native;

public enum ProcessAccess {
    Read,
    Write,
    Full
}

public static class NativeMethods {
    private static readonly uint ProcessEntry32WSize = (uint)Marshal.SizeOf<PROCESSENTRY32W>();
    private static readonly uint ModuleEntry32WSize = (uint)Marshal.SizeOf<MODULEENTRY32W>();
    private static readonly uint MemoryBasicInformationSize = (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>();
    private static readonly uint ImageDosHeaderSize = (uint)Marshal.SizeOf<IMAGE_DOS_HEADER>();
    private static readonly uint ImageNtHeaders64Size = (uint)Marshal.SizeOf<IMAGE_NT_HEADERS64>();
    private static readonly uint ImageSectionHeaderSize = (uint)Marshal.SizeOf<IMAGE_SECTION_HEADER>();

    private static readonly PROCESS_ACCESS_RIGHTS DefaultProcessAccessRights =
        PROCESS_ACCESS_RIGHTS.PROCESS_STANDARD_RIGHTS_REQUIRED |
        PROCESS_ACCESS_RIGHTS.PROCESS_TERMINATE |
        PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_INFORMATION |
        PROCESS_ACCESS_RIGHTS.PROCESS_SYNCHRONIZE;

    public unsafe static string UndecorateSymbolName(string name) {
        const int capacity = 255;
        var buffer = stackalloc byte[capacity];

        if (PInvoke.UnDecorateSymbolName(name, buffer, capacity, PInvoke.UNDNAME_NAME_ONLY) != 0)
            return Encoding.UTF8.GetString(buffer, capacity);

        return name;
    }

    public static void SetButtonShield(Button button, bool setShield) {
        try {
            if (button.FlatStyle != FlatStyle.System)
                button.FlatStyle = FlatStyle.System;

            var h = button.Handle;
            if (h == 0) return;

            PInvoke.SendMessage((HWND)h, PInvoke.BCM_SETSHIELD, 0, setShield ? 1 : 0);
        } catch {
            // ignored
        }
    }

    public static bool RegisterExtension(string fileExtension, string extensionId, string applicationPath, string applicationName) {
        try {
            using (var fileExtensionKey = Registry.ClassesRoot.CreateSubKey(fileExtension)) {
                fileExtensionKey.SetValue(string.Empty, extensionId, RegistryValueKind.String);
            }

            using (var extensionInfoKey = Registry.ClassesRoot.CreateSubKey(extensionId)) {
                extensionInfoKey.SetValue(string.Empty, applicationName, RegistryValueKind.String);

                using (var icon = extensionInfoKey.CreateSubKey("DefaultIcon")) {
                    icon.SetValue(string.Empty, "\"" + applicationPath + "\",0", RegistryValueKind.String);
                }

                using (var shellKey = extensionInfoKey.CreateSubKey("shell")) {
                    using (var openKey = shellKey.CreateSubKey("open")) {
                        openKey.SetValue(string.Empty, $"&Open with {applicationName}", RegistryValueKind.String);

                        using (var commandKey = openKey.CreateSubKey("command")) {
                            commandKey.SetValue(string.Empty, $"\"{applicationPath}\" \"%1\"", RegistryValueKind.String);
                        }
                    }
                }
            }

            FireAssocChanged();

            return true;
        } catch (Exception) {
            return false;
        }
    }

    public static void UnregisterExtension(string fileExtension, string extensionId) {
        try {
            Registry.ClassesRoot.DeleteSubKeyTree(fileExtension);
            Registry.ClassesRoot.DeleteSubKeyTree(extensionId);

            FireAssocChanged();
        } catch {
            // ignored
        }
    }

    private static unsafe void FireAssocChanged() {
        PInvoke.SHChangeNotify(SHCNE_ID.SHCNE_ASSOCCHANGED, SHCNF_FLAGS.SHCNF_IDLIST);
    }

    public static HANDLE OpenProcess(uint pid, ProcessAccess desiredAccess) {
        var dwDesiredAccess = DefaultProcessAccessRights;

        switch (desiredAccess) {
            case ProcessAccess.Read:
                dwDesiredAccess |= PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ;
                break;
            case ProcessAccess.Write:
                dwDesiredAccess |= PROCESS_ACCESS_RIGHTS.PROCESS_VM_OPERATION | PROCESS_ACCESS_RIGHTS.PROCESS_VM_WRITE;
                break;
            case ProcessAccess.Full:
                dwDesiredAccess |= PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ | PROCESS_ACCESS_RIGHTS.PROCESS_VM_OPERATION | PROCESS_ACCESS_RIGHTS.PROCESS_VM_WRITE;
                break;
        }

        return PInvoke.OpenProcess(dwDesiredAccess, false, pid);
    }

    public static unsafe ProcessInfo[] GetProcesses() {
        var handle = PInvoke.CreateToolhelp32Snapshot(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);
        if (handle == HANDLE.INVALID_HANDLE_VALUE) {
            return [];
        }

        var list = new List<ProcessInfo>();
        var pe32 = new PROCESSENTRY32W {
            dwSize = ProcessEntry32WSize
        };

        const int capacity = 255;
        var buffer = stackalloc char[capacity];

        if (PInvoke.Process32FirstW(handle, ref pe32)) {
            do {
                var processHandle = OpenProcess(pe32.th32ProcessID, ProcessAccess.Read);
                if (!IsValidProcess(processHandle)) {
                    continue;
                }

                if (PInvoke.IsWow64Process(processHandle, out var wow64Process) && wow64Process) {
                    PInvoke.CloseHandle(processHandle);
                    continue;
                }

                PInvoke.GetModuleFileNameEx(processHandle, HMODULE.Null, buffer, capacity);
                var path = new string(buffer);

                list.Add(new() {
                    Id = pe32.th32ProcessID,
                    Name = Path.GetFileName(path),
                    Path = path
                });

                PInvoke.CloseHandle(processHandle);

            } while (PInvoke.Process32NextW(handle, ref pe32));
        }

        PInvoke.CloseHandle(handle);

        return [.. list];
    }

    public static bool IsValidProcess(HANDLE processHandle) {
        if (processHandle.IsNull)
            return false;

        var result = PInvoke.WaitForSingleObject(processHandle, 0);
        if (result == WAIT_EVENT.WAIT_FAILED)
            return false;

        return result == WAIT_EVENT.WAIT_TIMEOUT;
    }

    public static unsafe ModuleInfo[] GetModules(HANDLE processHandle) {
        if (!IsValidProcess(processHandle))
            return [];

        var handle = PInvoke.CreateToolhelp32Snapshot(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPMODULE, PInvoke.GetProcessId(processHandle));
        if (handle == HANDLE.INVALID_HANDLE_VALUE) {
            return [];
        }

        var list = new List<ModuleInfo>();
        var me32 = new MODULEENTRY32W {
            dwSize = ModuleEntry32WSize
        };

        if (PInvoke.Module32FirstW(handle, ref me32)) {
            do {
                list.Add(new() {
                    BaseAddress = (nint)me32.modBaseAddr,
                    Size = me32.modBaseSize,
                    Path = me32.szExePath.ToString(),
                    Name = Path.GetFileName(me32.szExePath.ToString())
                });
            } while (PInvoke.Module32NextW(handle, ref me32));
        }

        PInvoke.CloseHandle(handle);

        return [.. list];
    }

    public static unsafe SectionInfo[] GetSections(HANDLE processHandle) {
        if (!IsValidProcess(processHandle))
            return [];

        var sections = new List<SectionInfo>();

        nuint address = 0;
        while (PInvoke.VirtualQueryEx(processHandle, (void*)address, out var memory, MemoryBasicInformationSize) != 0 && address + memory.RegionSize > address) {
            if (memory.State == VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT) {
                sections.Add(new() {
                    Start = (nint)memory.BaseAddress,
                    End = (nint)memory.BaseAddress + (nint)memory.RegionSize,
                    Size = (nint)memory.RegionSize,
                    Category = memory.Type == PAGE_TYPE.MEM_PRIVATE ? SectionCategory.HEAP : SectionCategory.Unknown
                });
            }

            address = (nuint)memory.BaseAddress + memory.RegionSize;
        }

        var modules = GetModules(processHandle);

        foreach (var module in modules) {
            var imageDosHeader = new IMAGE_DOS_HEADER();
            if (!PInvoke.ReadProcessMemory(processHandle, (void*)module.BaseAddress, &imageDosHeader, ImageDosHeaderSize))
                continue;

            var ntHeaderPtr = (void*)(module.BaseAddress + imageDosHeader.e_lfanew);

            var imageNtHeaders = new IMAGE_NT_HEADERS64();
            if (!PInvoke.ReadProcessMemory(processHandle, ntHeaderPtr, &imageNtHeaders, ImageNtHeaders64Size))
                continue;

            var sectionHeaders = new IMAGE_SECTION_HEADER[imageNtHeaders.FileHeader.NumberOfSections];

            fixed (void* sectionHeadersPtr = sectionHeaders)
                PInvoke.ReadProcessMemory(processHandle, (void*)((nuint)ntHeaderPtr + ImageNtHeaders64Size), sectionHeadersPtr, imageNtHeaders.FileHeader.NumberOfSections * ImageNtHeaders64Size);

            foreach (var sectionHeader in sectionHeaders) {
                var sectionAddress = module.BaseAddress + sectionHeader.VirtualAddress;

                foreach (var section in sections) {
                    if (sectionAddress < section.Start)
                        continue;

                    if (sectionAddress >= section.Start + section.Size)
                        continue;

                    if (sectionHeader.VirtualAddress + sectionHeader.Misc.VirtualSize > module.Size)
                        continue;

                    if (sectionHeader.Characteristics.HasFlag(IMAGE_SECTION_CHARACTERISTICS.IMAGE_SCN_CNT_CODE)) {
                        section.Category = SectionCategory.CODE;
                    } else if (sectionHeader.Characteristics.HasFlag(IMAGE_SECTION_CHARACTERISTICS.IMAGE_SCN_CNT_INITIALIZED_DATA)
                        || sectionHeader.Characteristics.HasFlag(IMAGE_SECTION_CHARACTERISTICS.IMAGE_SCN_CNT_UNINITIALIZED_DATA)) {
                        section.Category = SectionCategory.DATA;
                    }

                    section.Name = Encoding.UTF8.GetString(sectionHeader.Name.AsSpan()).TrimEnd('\0');
                    section.ModulePath = module.Path;
                    section.ModuleName = Path.GetFileName(module.Path);
                    break;
                }
            }
        }

        return [.. sections.OrderBy(section => section.Start)];
    }
}
