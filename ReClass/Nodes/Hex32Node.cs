using ReClass.Controls;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Hex32Node : BaseHexCommentNode {
    public override int MemorySize => 4;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Hex32";
        icon = Resources.B16x16_Button_Hex_32;
    }

    public override string GetToolTipText(HotSpot spot) {
        var value = ReadFromBuffer(spot.Memory, Offset);

        return $"Int32: {value.IntValue}\nUInt32: 0x{value.UIntValue:X08}\nFloat: {value.FloatValue:0.000}";
    }

    public override Size Draw(DrawContext context, int x, int y) => Draw(context, x, y, context.Settings.ShowNodeText ? context.Memory.ReadString(context.Settings.RawDataEncoding, Offset, 4) + "     " : null, 4);

    public override void Update(HotSpot spot) {
        Update(spot, 4);
    }

    protected override int AddComment(DrawContext context, int x, int y) {
        x = base.AddComment(context, x, y);

        var value = ReadFromBuffer(context.Memory, Offset);

        x = AddComment(context, x, y, value.FloatValue, 0, 0);

        return x;
    }

    private static UInt32FloatData ReadFromBuffer(MemoryBuffer memory, int offset) => new() {
        Raw = memory.ReadInt32(offset)
    };
}
