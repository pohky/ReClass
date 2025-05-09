using System.Globalization;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Int32Node : BaseNumericNode {
    public override int MemorySize => 4;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Int32";
        icon = Resources.B16x16_Button_Int_32;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        var value = ReadValueFromMemory(context.Memory);
        return DrawNumeric(context, x, y, IconProvider.Signed, "Int32", value.ToString(), $"0x{value:X}");
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0 || spot.Id == 1) {
            if (int.TryParse(spot.Text, out var val) || (spot.Text.TryGetHexString(out var hexValue) && int.TryParse(hexValue, NumberStyles.HexNumber, null, out val))) {
                spot.Process.WriteRemoteMemory(spot.Address, val);
            }
        }
    }

    public int ReadValueFromMemory(MemoryBuffer memory) => memory.ReadInt32(Offset);
}
