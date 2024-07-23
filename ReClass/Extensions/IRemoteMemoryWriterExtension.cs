using System.Text;
using ReClass.Memory;

namespace ReClass.Extensions;

public static class IRemoteMemoryWriterExtension {
    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, sbyte value) => writer.WriteRemoteMemory(address, [(byte)value]);

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, byte value) => writer.WriteRemoteMemory(address, [value]);

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, short value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, ushort value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, int value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, uint value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, long value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, ulong value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, float value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, double value) => writer.WriteRemoteMemory(address, writer.BitConverter.GetBytes(value));

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, nint value) => writer.WriteRemoteMemory(address, value);

    public static void WriteRemoteMemory(this IRemoteMemoryWriter writer, nint address, string value, Encoding encoding) => writer.WriteRemoteMemory(address, encoding.GetBytes(value));
}
