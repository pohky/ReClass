using System.Text;
using ReClass.Memory;

namespace ReClass.Extensions;

public static class IRemoteMemoryReaderExtension {
    public static sbyte ReadRemoteInt8(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(sbyte));

        return (sbyte)data[0];
    }

    public static byte ReadRemoteUInt8(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(byte));

        return data[0];
    }

    public static short ReadRemoteInt16(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(short));

        return reader.BitConverter.ToInt16(data, 0);
    }

    public static ushort ReadRemoteUInt16(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(ushort));

        return reader.BitConverter.ToUInt16(data, 0);
    }

    public static int ReadRemoteInt32(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(int));

        return reader.BitConverter.ToInt32(data, 0);
    }

    public static uint ReadRemoteUInt32(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(uint));

        return reader.BitConverter.ToUInt32(data, 0);
    }

    public static long ReadRemoteInt64(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(long));

        return reader.BitConverter.ToInt64(data, 0);
    }

    public static ulong ReadRemoteUInt64(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(ulong));

        return reader.BitConverter.ToUInt64(data, 0);
    }

    public static float ReadRemoteFloat(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(float));

        return reader.BitConverter.ToSingle(data, 0);
    }

    public static double ReadRemoteDouble(this IRemoteMemoryReader reader, nint address) {
        var data = reader.ReadRemoteMemory(address, sizeof(double));

        return reader.BitConverter.ToDouble(data, 0);
    }

    public static nint ReadRemoteNInt(this IRemoteMemoryReader reader, nint address) {
        return (nint)reader.ReadRemoteInt64(address);
    }

    public static string ReadRemoteString(this IRemoteMemoryReader reader, nint address, Encoding encoding, int length) {
        var data = reader.ReadRemoteMemory(address, length * encoding.GuessByteCountPerChar());

        try {
            var sb = new StringBuilder(encoding.GetString(data));
            for (var i = 0; i < sb.Length; ++i) {
                if (sb[i] == '\0') {
                    sb.Length = i;
                    break;
                }
                if (!sb[i].IsPrintable()) {
                    sb[i] = '.';
                }
            }
            return sb.ToString();
        } catch {
            return string.Empty;
        }
    }
}
