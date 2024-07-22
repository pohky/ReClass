using System.Diagnostics;

namespace ReClass.Extensions;

public static class ProcessExtensions {
    public static Image? GetIcon(this Process process) {
        try {
            if (process.MainModule == null)
                return null;

            return Icon.ExtractAssociatedIcon(process.MainModule.FileName)?.ToBitmap();
        } catch (Exception) {
            return null;
        }
    }
}
