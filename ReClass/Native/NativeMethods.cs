using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.ToolHelp;
using Windows.Win32.System.Threading;
using Windows.Win32.UI.Shell;

namespace ReClass.Native;

public static class NativeMethods {
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

    public static unsafe ProcessInfo[] GetProcesses() {
        var handle = PInvoke.CreateToolhelp32Snapshot(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);
        if (handle == HANDLE.INVALID_HANDLE_VALUE) {
            return [];
        }

        var list = new List<ProcessInfo>();
        var pe32 = new PROCESSENTRY32W {
            dwSize = (uint)Marshal.SizeOf<PROCESSENTRY32W>()
        };

        const int capacity = 255;
        var buffer = stackalloc char[capacity];

        if (PInvoke.Process32FirstW(handle, ref pe32)) {
            do {
                var process = PInvoke.OpenProcess(
                    PROCESS_ACCESS_RIGHTS.PROCESS_STANDARD_RIGHTS_REQUIRED |
                    PROCESS_ACCESS_RIGHTS.PROCESS_TERMINATE |
                    PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_INFORMATION |
                    PROCESS_ACCESS_RIGHTS.PROCESS_SYNCHRONIZE |
                    PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ,
                    false,
                    pe32.th32ProcessID);
                if (process.IsNull || process == HANDLE.INVALID_HANDLE_VALUE) {
                    continue;
                }

                if (PInvoke.IsWow64Process(process, out var wow64Process) && wow64Process) {
                    PInvoke.CloseHandle(process);
                    continue;
                }

                var retn = PInvoke.WaitForSingleObject(process, 0);
                if (retn != WAIT_EVENT.WAIT_TIMEOUT) {
                    PInvoke.CloseHandle(process);
                    continue;
                }

                PInvoke.GetModuleFileNameEx(process, HMODULE.Null, buffer, capacity);
                var path = new string(buffer);

                list.Add(new() {
                    Id = pe32.th32ProcessID,
                    Name = Path.GetFileName(path),
                    Path = path
                });

                PInvoke.CloseHandle(process);

            } while (PInvoke.Process32NextW(handle, ref pe32));
        }

        PInvoke.CloseHandle(handle);

        return [.. list];
    }
}
