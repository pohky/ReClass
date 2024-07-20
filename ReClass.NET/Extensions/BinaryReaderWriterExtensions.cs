using System.Diagnostics.Contracts;

namespace ReClassNET.Extensions;

public static class BinaryReaderWriterExtension {
    public static IntPtr ReadIntPtr(this BinaryReader br) {
        Contract.Requires(br != null);

        return (IntPtr)br.ReadInt64();
    }

    public static void Write(this BinaryWriter bw, IntPtr value) {
        Contract.Requires(bw != null);

        bw.Write(value.ToInt64());
    }
}
