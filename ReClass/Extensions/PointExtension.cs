using System.Diagnostics;

namespace ReClass.Extensions;

public static class PointExtension {
    /// <summary>
    ///     Creates a new point which is relocated with the given offsets.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="offsetX">The offset in x direction.</param>
    /// <param name="offsetY">The offset in y direction.</param>
    /// <returns>The relocated point.</returns>
    [DebuggerStepThrough]
    public static Point Relocate(this Point point, int offsetX, int offsetY) => new(point.X + offsetX, point.Y + offsetY);
}
