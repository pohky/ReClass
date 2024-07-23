using System.Globalization;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class UInt8Node : BaseNumericNode {
    public override int MemorySize => 1;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "UInt8 / BYTE";
        icon = Resources.B16x16_Button_UInt_8;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        var value = ReadValueFromMemory(context.Memory);
        return DrawNumeric(context, x, y, IconProvider.Unsigned, "UInt8", value.ToString(), $"0x{value:X}");
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0 || spot.Id == 1) {
            if (byte.TryParse(spot.Text, out var val) || (spot.Text.TryGetHexString(out var hexValue) && byte.TryParse(hexValue, NumberStyles.HexNumber, null, out val))) {
                spot.Process.WriteRemoteMemory(spot.Address, val);
            }
        }
    }

    public byte ReadValueFromMemory(MemoryBuffer memory) => memory.ReadUInt8(Offset);
}
