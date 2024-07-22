using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.UI;

namespace ReClass.Nodes;

public abstract class BaseFunctionPtrNode : BaseFunctionNode {
    public override int MemorySize => IntPtr.Size;

    public override string GetToolTipText(HotSpot spot) {
        var ptr = spot.Memory.ReadIntPtr(Offset);

        DisassembleRemoteCode(spot.Process, ptr);

        return string.Join("\n", Instructions.Select(i => i.Instruction));
    }

    protected Size Draw(DrawContext context, int x, int y, string type, string name) {
        if (IsHidden && !IsWrapped) {
            return DrawHidden(context, x, y);
        }

        var origX = x;

        AddSelection(context, x, y, context.Font.Height);

        x = AddIconPadding(context, x);

        x = AddIcon(context, x, y, context.IconProvider.Function, HotSpot.NoneId, HotSpotType.None);

        var tx = x;

        x = AddAddressOffset(context, x, y);

        x = AddText(context, x, y, context.Settings.TypeColor, HotSpot.NoneId, type) + context.Font.Width;
        if (!IsWrapped) {
            x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, name) + context.Font.Width;
        }

        x = AddOpenCloseIcon(context, x, y) + context.Font.Width;

        x = AddComment(context, x, y);

        DrawInvalidMemoryIndicatorIcon(context, y);
        AddContextDropDownIcon(context, y);
        AddDeleteIcon(context, y);

        var size = new Size(x - origX, context.Font.Height);

        if (LevelsOpen[context.Level]) {
            var ptr = context.Memory.ReadIntPtr(Offset);

            DisassembleRemoteCode(context.Process, ptr);

            var instructionSize = DrawInstructions(context, tx, y);

            size.Width = Math.Max(size.Width, instructionSize.Width + tx - origX);
            size.Height += instructionSize.Height;
        }

        return size;
    }

    public override int CalculateDrawnHeight(DrawContext context) {
        if (IsHidden) {
            return HiddenHeight;
        }

        var height = context.Font.Height;
        if (LevelsOpen[context.Level]) {
            height += Instructions.Count * context.Font.Height;
        }
        return height;
    }

    private void DisassembleRemoteCode(RemoteProcess process, IntPtr address) {
        if (Address != address) {
            Instructions.Clear();

            Address = address;

            if (!address.IsNull() && process.IsValid) {
                DisassembleRemoteCode(process, address, out _);
            }
        }
    }
}
