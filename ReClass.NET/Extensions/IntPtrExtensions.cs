using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace ReClassNET.Extensions;

public static class IntPtrExtension {
    [Pure]
    [DebuggerStepThrough]
    public static bool IsNull(this IntPtr ptr) => ptr == IntPtr.Zero;

    [Pure]
    [DebuggerStepThrough]
    public static bool MayBeValid(this IntPtr ptr) {
        return ptr.IsInRange(0x10000, (IntPtr)long.MaxValue);
    }

    [Pure]
    [DebuggerStepThrough]
    public static IntPtr Add(this IntPtr lhs, IntPtr rhs) {
        return new IntPtr(lhs.ToInt64() + rhs.ToInt64());
    }

    [Pure]
    [DebuggerStepThrough]
    public static IntPtr Sub(this IntPtr lhs, IntPtr rhs) {
        return new IntPtr(lhs.ToInt64() - rhs.ToInt64());
    }

    [Pure]
    [DebuggerStepThrough]
    public static IntPtr Mul(this IntPtr lhs, IntPtr rhs) {
        return new IntPtr(lhs.ToInt64() * rhs.ToInt64());
    }

    [Pure]
    [DebuggerStepThrough]
    public static IntPtr Div(this IntPtr lhs, IntPtr rhs) {
        return new IntPtr(lhs.ToInt64() / rhs.ToInt64());
    }

    [Pure]
    [DebuggerStepThrough]
    public static int Mod(this IntPtr lhs, int mod) {
        return (int)(lhs.ToInt64() % mod);
    }

    [Pure]
    [DebuggerStepThrough]
    public static IntPtr Negate(this IntPtr ptr) {
        return new IntPtr(-ptr.ToInt64());
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool IsInRange(this IntPtr address, IntPtr start, IntPtr end) {
        var val = (ulong)address.ToInt64();
        return (ulong)start.ToInt64() <= val && val <= (ulong)end.ToInt64();
    }

    [Pure]
    [DebuggerStepThrough]
    public static int CompareTo(this IntPtr lhs, IntPtr rhs) {
        return ((ulong)lhs.ToInt64()).CompareTo((ulong)rhs.ToInt64());
    }

    [Pure]
    [DebuggerStepThrough]
    public static int CompareToRange(this IntPtr address, IntPtr start, IntPtr end) {
        if (IsInRange(address, start, end)) {
            return 0;
        }
        return CompareTo(address, start);
    }

    /// <summary>
    ///     Changes the behaviour of ToInt64 in x86 mode.
    ///     IntPtr(int.MaxValue + 1) = (int)0x80000000 (-2147483648) = (long)0xFFFFFFFF80000000
    ///     This method converts the value to (long)0x0000000080000000 (2147483648).
    /// </summary>
    /// <param name="ptr"></param>
    /// <returns></returns>
    [Pure]
    [DebuggerStepThrough]
    public static long ToInt64Bits(this IntPtr ptr) {
        return ptr.ToInt64();
    }

    [Pure]
    [DebuggerStepThrough]
    public static IntPtr From(int value) => value;

    [Pure]
    [DebuggerStepThrough]
    public static IntPtr From(long value) {
        return (IntPtr)value;
    }
}
