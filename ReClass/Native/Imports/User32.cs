using System.Runtime.InteropServices;

namespace ReClass.Native.Imports;

internal static class User32 {
    public const int BCM_SETSHIELD = 0x160C;

    [DllImport("user32")]
    public static extern nint SendMessage(nint hWnd, int nMsg, nint wParam, nint lParam);
}
