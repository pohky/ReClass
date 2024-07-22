using System.Diagnostics;
using System.Security.Principal;

namespace ReClassNET.Util;

public static class WinUtil {
    //from https://stackoverflow.com/a/11660205
    public static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    /// <summary>Executes the a process with elevated permissions.</summary>
    /// <param name="applicationPath"> The executable path.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool RunElevated(string applicationPath, string arguments) {
        try {
            var processStartInfo = new ProcessStartInfo {
                FileName = applicationPath,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };
            if (arguments != null) {
                processStartInfo.Arguments = arguments;
            }

            if (Environment.OSVersion.Version.Major >= 6) {
                processStartInfo.Verb = "runas";
            }

            Process.Start(processStartInfo);
        } catch (Exception) {
            return false;
        }

        return true;
    }
}
