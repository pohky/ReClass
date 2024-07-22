namespace ReClass.Extensions;

public static class BinaryReaderWriterExtension {
    public static IntPtr ReadIntPtr(this BinaryReader br) {
        return (IntPtr)br.ReadInt64();
    }

    public static void Write(this BinaryWriter bw, IntPtr value) {
        bw.Write(value.ToInt64());
    }
}
