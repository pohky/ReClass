using System.Text;
using Microsoft.Win32;
using ReClass.Native.Imports;

namespace ReClass.Native;

public static class NativeMethods {
    public static nint LoadLibrary(string name) {
        return Kernel32.LoadLibrary(name);
    }

    public static nint GetProcAddress(nint handle, string name) {
        return Kernel32.GetProcAddress(handle, name);
    }

    public static void FreeLibrary(nint handle) {
        Kernel32.FreeLibrary(handle);
    }

    public static Icon? GetIconForFile(string path) {
        return Icon.ExtractAssociatedIcon(path);
    }
    
    public static string UndecorateSymbolName(string name) {
        var sb = new StringBuilder(255);
        if (DbgHelp.UnDecorateSymbolName(name, sb, sb.Capacity, DbgHelp.UNDNAME_NAME_ONLY) != 0)
            return sb.ToString();
        return name;
    }

    public static void SetButtonShield(Button button, bool setShield) {
        try {
            if (button.FlatStyle != FlatStyle.System)
                button.FlatStyle = FlatStyle.System;

            var h = button.Handle;
            if (h == 0) return;

            User32.SendMessage(h, User32.BCM_SETSHIELD, 0, setShield ? 1 : 0);
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

            Shell32.SHChangeNotify(Shell32.SHCNE_ASSOCCHANGED, Shell32.SHCNF_IDLIST, 0, 0);

            return true;
        } catch (Exception) {
            return false;
        }
    }

    public static void UnregisterExtension(string fileExtension, string extensionId) {
        try {
            Registry.ClassesRoot.DeleteSubKeyTree(fileExtension);
            Registry.ClassesRoot.DeleteSubKeyTree(extensionId);

            Shell32.SHChangeNotify(Shell32.SHCNE_ASSOCCHANGED, Shell32.SHCNF_IDLIST, 0, 0);
        } catch {
            // ignored
        }
    }
}
