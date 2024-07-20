using System.Globalization;
using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.Properties;
using ReClassNET.UI;

namespace ReClassNET.Nodes;

public class NUIntNode : BaseNumericNode {
    public override int MemorySize => UIntPtr.Size;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "NUInt";
        icon = Resources.B16x16_Button_NUInt;
    }

    public override Size Draw(DrawContext context, int x, int y) {
        var value = ReadValueFromMemory(context.Memory).ToUInt64();
        return DrawNumeric(context, x, y, context.IconProvider.Unsigned, "NUInt", value.ToString(), $"0x{value:X}");
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0 || spot.Id == 1) {
            if (ulong.TryParse(spot.Text, out var val) || (spot.Text.TryGetHexString(out var hexValue) && ulong.TryParse(hexValue, NumberStyles.HexNumber, null, out val))) {
                spot.Process.WriteRemoteMemory(spot.Address, val);
            }
        }
    }

    public UIntPtr ReadValueFromMemory(MemoryBuffer memory) => memory.ReadUIntPtr(Offset);
}
