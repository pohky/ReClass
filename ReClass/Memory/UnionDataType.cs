using System.Runtime.InteropServices;

namespace ReClass.Memory;

[StructLayout(LayoutKind.Explicit)]
public struct UInt16Data {
    [FieldOffset(0)]
    public short ShortValue;

    [FieldOffset(0)]
    public ushort UShortValue;
}

[StructLayout(LayoutKind.Explicit)]
public struct UInt32FloatData {
    [FieldOffset(0)]
    public int Raw;

    [FieldOffset(0)]
    public int IntValue;

    [FieldOffset(0)]
    public uint UIntValue;

    [FieldOffset(0)]
    public float FloatValue;
}

[StructLayout(LayoutKind.Explicit)]
public struct UInt64FloatDoubleData {
    [FieldOffset(0)]
    public int Raw1;

    [FieldOffset(4)]
    public int Raw2;

    [FieldOffset(0)]
    public long LongValue;

    [FieldOffset(0)]
    public nint NIntValue;

    [FieldOffset(0)]
    public ulong ULongValue;

    [FieldOffset(0)]
    public nuint NUIntValue;

    [FieldOffset(0)]
    public float FloatValue;

    [FieldOffset(0)]
    public double DoubleValue;
}
