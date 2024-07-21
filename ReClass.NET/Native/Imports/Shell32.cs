using System.Runtime.InteropServices;

namespace ReClassNET.Native.Imports;

internal static class Shell32 {
    public const int SHCNE_ASSOCCHANGED = 0x08000000;
    public const uint SHCNF_IDLIST = 0x0000;

    [DllImport("shell32")]
    public static extern void SHChangeNotify(int wEventId, uint uFlags, nint dwItem1, nint dwItem2);
}
