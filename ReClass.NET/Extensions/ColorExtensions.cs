using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace ReClassNET.Extensions;

public static class ExtensionColor {
    [Pure]
    [DebuggerStepThrough]
    public static int ToRgb(this Color color) => 0xFFFFFF & color.ToArgb();

    [Pure]
    [DebuggerStepThrough]
    public static Color Invert(this Color color) => Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
}
