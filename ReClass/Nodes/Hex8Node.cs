using ReClass.Controls;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Hex8Node : BaseHexNode {
    public override int MemorySize => 1;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Hex8";
        icon = Resources.B16x16_Button_Hex_8;
    }

    public override string GetToolTipText(HotSpot spot) {
        var b = spot.Memory.ReadUInt8(Offset);

        return $"Int8: {(int)b}\nUInt8: 0x{b:X02}";
    }

    public override Size Draw(DrawContext context, int x, int y) => Draw(context, x, y, context.Settings.ShowNodeText ? context.Memory.ReadString(context.Settings.RawDataEncoding, Offset, 1) + "        " : null, 1);

    public override void Update(HotSpot spot) {
        Update(spot, 1);
    }
}
