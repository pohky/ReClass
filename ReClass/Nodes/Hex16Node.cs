using ReClass.Controls;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Hex16Node : BaseHexNode {
    public override int MemorySize => 2;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Hex16";
        icon = Resources.B16x16_Button_Hex_16;
    }

    public override string GetToolTipText(HotSpot spot) {
        var value = new UInt16Data { ShortValue = spot.Memory.ReadInt16(Offset) };

        return $"Int16: {value.ShortValue}\nUInt16: 0x{value.UShortValue:X04}";
    }

    public override Size Draw(DrawContext context, int x, int y) => Draw(context, x, y, context.Settings.ShowNodeText ? context.Memory.ReadString(context.Settings.RawDataEncoding, Offset, 2) + "       " : null, 2);

    public override void Update(HotSpot spot) {
        Update(spot, 2);
    }
}
