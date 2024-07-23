using System.Diagnostics;

namespace ReClass.Extensions;

public static class NUIntExtensions {
    [DebuggerStepThrough]
    public static bool IsInRange(this nuint address, nuint start, nuint end) {
        return start <= address && address <= end;
    }

    [DebuggerStepThrough]
    public static int CompareToRange(this nuint address, nuint start, nuint end) {
        if (IsInRange(address, start, end)) {
            return 0;
        }
        return address.CompareTo(start);
    }
}
