namespace ReClassNET.Util;

public class IntPtrComparer : IComparer<IntPtr> {
    public static IntPtrComparer Instance { get; } = new();

    public int Compare(IntPtr x, IntPtr y) => x.CompareTo(y);
}
