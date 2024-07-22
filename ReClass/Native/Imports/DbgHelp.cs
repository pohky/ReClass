using System.Runtime.InteropServices;
using System.Text;

namespace ReClass.Native.Imports;

internal static class DbgHelp {
    public const int UNDNAME_NAME_ONLY = 0x1000;

    [DllImport("dbghelp", CharSet = CharSet.Unicode)]
    public static extern int UnDecorateSymbolName(string decoratedName, StringBuilder unDecoratedName, int undecoratedLength, int flags);
}
