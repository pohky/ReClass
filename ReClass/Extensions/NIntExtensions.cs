using System.Diagnostics;

namespace ReClass.Extensions;

public static class NIntExtensions {
    [DebuggerStepThrough]
    public static nint Add(this nint lhs, nint rhs) => lhs + rhs;

    [DebuggerStepThrough]
    public static nint Sub(this nint lhs, nint rhs) => lhs - rhs;

    [DebuggerStepThrough]
    public static nint Mul(this nint lhs, nint rhs) => lhs * rhs;

    [DebuggerStepThrough]
    public static nint Div(this nint lhs, nint rhs) => lhs / rhs;

    [DebuggerStepThrough]
    public static int Mod(this nint lhs, int mod) => (int)(lhs % mod);

    [DebuggerStepThrough]
    public static nint Negate(this nint ptr) => -ptr;

    [DebuggerStepThrough]
    public static bool IsInRange(this nint address, nint start, nint end) {
        var val = (nuint)address;
        return (nuint)start <= val && val <= (nuint)end;
    }

    [DebuggerStepThrough]
    public static int CompareTo(this nint lhs, nint rhs) => ((nuint)lhs).CompareTo((nuint)rhs);

    [DebuggerStepThrough]
    public static int CompareToRange(this nint address, nint start, nint end) {
        if (IsInRange(address, start, end)) {
            return 0;
        }
        return CompareTo(address, start);
    }

    [DebuggerStepThrough]
    public static nint From(int value) => value;

    [DebuggerStepThrough]
    public static nint From(long value) => (nint)value;
}
