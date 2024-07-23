using System.Globalization;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class NIntNode : BaseNumericNode {
    public override int MemorySize => nint.Size;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "NInt";
        icon = Resources.B16x16_Button_NInt;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        var value = ReadValueFromMemory(context.Memory);
        return DrawNumeric(context, x, y, IconProvider.Signed, "NInt", value.ToString(), $"0x{value:X}");
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0 || spot.Id == 1) {
            if (long.TryParse(spot.Text, out var val) || (spot.Text.TryGetHexString(out var hexValue) && long.TryParse(hexValue, NumberStyles.HexNumber, null, out val))) {
                spot.Process.WriteRemoteMemory(spot.Address, val);
            }
        }
    }

    public IntPtr ReadValueFromMemory(MemoryBuffer memory) => memory.ReadNInt(Offset);
}
