using System.Diagnostics;

namespace ReClassNET.Extensions;

public static class ByteExtension {
    /// <summary>
    ///     Sets every element in the array to zero.
    /// </summary>
    /// <param name="array"></param>
    [DebuggerStepThrough]
    public static void FillWithZero(this byte[] array) {
        ((Span<byte>)array).Clear();
    }
}
