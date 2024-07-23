using ReClass.Controls;
using ReClass.Memory;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class FunctionNode : BaseFunctionNode {
    private int memorySize = nint.Size;
    public string Signature { get; set; } = "void function()";

    public ClassNode? BelongsToClass { get; set; }
    public override int MemorySize => memorySize;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Function";
        icon = Resources.B16x16_Button_Function;
    }

    public override string GetToolTipText(HotSpot spot) {
        DisassembleRemoteCode(spot.Process, spot.Address);

        return string.Join("\n", Instructions.Select(i => i.Instruction));
    }

    public override Size Draw(DrawContext context, int x, int y) {
        if (IsHidden && !IsWrapped) {
            return DrawHidden(context, x, y);
        }

        var origX = x;

        AddSelection(context, x, y, context.Font.Height);

        x = AddIconPadding(context, x);

        x = AddIcon(context, x, y, IconProvider.Function, HotSpot.NoneId, HotSpotType.None);

        var tx = x;

        x = AddAddressOffset(context, x, y);

        x = AddText(context, x, y, context.Settings.TypeColor, HotSpot.NoneId, "Function") + context.Font.Width;
        if (!IsWrapped) {
            x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, Name) + context.Font.Width;
        }

        x = AddOpenCloseIcon(context, x, y) + context.Font.Width;

        x = AddComment(context, x, y);

        DrawInvalidMemoryIndicatorIcon(context, y);
        AddContextDropDownIcon(context, y);
        AddDeleteIcon(context, y);

        var size = new Size(x - origX, context.Font.Height);

        var ptr = context.Address + Offset;
        DisassembleRemoteCode(context.Process, ptr);

        if (LevelsOpen[context.Level]) {
            y += context.Font.Height;
            x = AddText(context, tx, y, context.Settings.TypeColor, HotSpot.NoneId, "Signature:") + context.Font.Width;
            x = AddText(context, x, y, context.Settings.ValueColor, 0, Signature);
            size.Width = Math.Max(size.Width, x - origX);
            size.Height += context.Font.Height;

            y += context.Font.Height;
            x = AddText(context, tx, y, context.Settings.TextColor, HotSpot.NoneId, "Belongs to: ");
            x = AddText(context, x, y, context.Settings.ValueColor, HotSpot.NoneId, BelongsToClass == null ? "<None>" : $"<{BelongsToClass.Name}>") + context.Font.Width;
            x = AddIcon(context, x, y, IconProvider.Change, 1, HotSpotType.ChangeClassType);
            size.Width = Math.Max(size.Width, x - origX);
            size.Height += context.Font.Height;

            var instructionSize = DrawInstructions(context, tx, y + 4);
            size.Width = Math.Max(size.Width, instructionSize.Width + tx - origX);
            size.Height += instructionSize.Height + 4;
        }

        return size;
    }

    public override int CalculateDrawnHeight(DrawContext context) {
        if (IsHidden && !IsWrapped) {
            return HiddenHeight;
        }

        var height = context.Font.Height;
        if (LevelsOpen[context.Level]) {
            height += Instructions.Count * context.Font.Height;
        }
        return height;
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0) // Signature
        {
            Signature = spot.Text;
        }
    }

    private void DisassembleRemoteCode(RemoteProcess process, nint address) {
        if (Address != address) {
            Instructions.Clear();

            Address = address;

            if (address != 0 && process.IsValid) {
                DisassembleRemoteCode(process, address, out memorySize);
            }

            GetParentContainer()?.ChildHasChanged(this);
        }
    }
}
