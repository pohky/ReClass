using System.Globalization;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Int16Node : BaseNumericNode {
    public override int MemorySize => 2;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Int16";
        icon = Resources.B16x16_Button_Int_16;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        var value = ReadValueFromMemory(context.Memory);
        return DrawNumeric(context, x, y, IconProvider.Signed, "Int16", value.ToString(), $"0x{value:X}");
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0 || spot.Id == 1) {
            if (short.TryParse(spot.Text, out var val) || (spot.Text.TryGetHexString(out var hexValue) && short.TryParse(hexValue, NumberStyles.HexNumber, null, out val))) {
                spot.Process.WriteRemoteMemory(spot.Address, val);
            }
        }
    }

    public short ReadValueFromMemory(MemoryBuffer memory) => memory.ReadInt16(Offset);
}
