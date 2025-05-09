using System.Text;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.UI;

namespace ReClass.Nodes;

public abstract class BaseTextPtrNode : BaseNode {
    private const int MaxStringCharacterCount = 256;

    public override int MemorySize => nint.Size;

    /// <summary>The encoding of the string.</summary>
    public abstract Encoding Encoding { get; }

    /// <summary>Draws this node.</summary>
    /// <param name="context">The drawing context.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="type">The name of the type.</param>
    /// <returns>The pixel size the node occupies.</returns>
    public Size DrawText(DrawContext context, int x, int y, string type) {
        if (IsHidden && !IsWrapped) {
            return DrawHidden(context, x, y);
        }

        var ptr = context.Memory.ReadNInt(Offset);
        var text = context.Process.ReadRemoteString(ptr, Encoding, MaxStringCharacterCount);

        var origX = x;

        AddSelection(context, x, y, context.Font.Height);

        x = AddIconPadding(context, x);

        x = AddIcon(context, x, y, IconProvider.Text, HotSpot.NoneId, HotSpotType.None);
        x = AddAddressOffset(context, x, y);

        x = AddText(context, x, y, context.Settings.TypeColor, HotSpot.NoneId, type) + context.Font.Width;
        if (!IsWrapped) {
            x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, Name) + context.Font.Width;
        }

        x = AddText(context, x, y, context.Settings.TextColor, HotSpot.NoneId, "= '");
        x = AddText(context, x, y, context.Settings.TextColor, HotSpot.ReadOnlyId, text);
        x = AddText(context, x, y, context.Settings.TextColor, HotSpot.NoneId, "'") + context.Font.Width;

        x = AddComment(context, x, y);

        DrawInvalidMemoryIndicatorIcon(context, y);
        AddContextDropDownIcon(context, y);
        AddDeleteIcon(context, y);

        return new Size(x - origX, context.Font.Height);
    }

    public override int CalculateDrawnHeight(DrawContext context) => IsHidden && !IsWrapped ? HiddenHeight : context.Font.Height;
}
