using System.Runtime.InteropServices;

namespace ReClass.Native.Imports;

internal static unsafe class Kernel32 {
    [DllImport("kernel32")]
    public static extern nint OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, nint dwProcessId);

    [DllImport("kernel32")]
    public static extern void CloseHandle(nint hObject);

    [DllImport("kernel32")]
    public static extern int GetProcessId(nint hProcess);

    [DllImport("kernel32")]
    public static extern uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    public static extern nint LoadLibrary(string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Ansi)]
    public static extern nint GetProcAddress(nint hModule, string lpProcName);

    [DllImport("kernel32")]
    public static extern bool FreeLibrary(nint hModule);

    [DllImport("kernel32")]
    public static extern bool IsWow64Process(nint hProcess, out bool wow64Process);
}
