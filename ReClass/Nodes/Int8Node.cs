using System.Globalization;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Int8Node : BaseNumericNode {
    public override int MemorySize => 1;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Int8";
        icon = Resources.B16x16_Button_Int_8;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        var value = ReadValueFromMemory(context.Memory);
        return DrawNumeric(context, x, y, context.IconProvider.Signed, "Int8", value.ToString(), $"0x{value:X}");
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0 || spot.Id == 1) {
            if (sbyte.TryParse(spot.Text, out var val) || (spot.Text.TryGetHexString(out var hexValue) && sbyte.TryParse(hexValue, NumberStyles.HexNumber, null, out val))) {
                spot.Process.WriteRemoteMemory(spot.Address, val);
            }
        }
    }

    public sbyte ReadValueFromMemory(MemoryBuffer memory) => memory.ReadInt8(Offset);
}
