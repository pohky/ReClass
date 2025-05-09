using System.Globalization;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Int64Node : BaseNumericNode {
    public override int MemorySize => 8;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Int64";
        icon = Resources.B16x16_Button_Int_64;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        var value = ReadValueFromMemory(context.Memory);
        return DrawNumeric(context, x, y, IconProvider.Signed, "Int64", value.ToString(), $"0x{value:X}");
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0 || spot.Id == 1) {
            if (long.TryParse(spot.Text, out var val) || (spot.Text.TryGetHexString(out var hexValue) && long.TryParse(hexValue, NumberStyles.HexNumber, null, out val))) {
                spot.Process.WriteRemoteMemory(spot.Address, val);
            }
        }
    }

    public long ReadValueFromMemory(MemoryBuffer memory) => memory.ReadInt64(Offset);
}
