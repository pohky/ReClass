using ReClass.Controls;
using ReClass.Memory;
using ReClass.UI;

namespace ReClass.Nodes;

public abstract class BaseFunctionNode : BaseNode {
    protected readonly List<FunctionNodeInstruction> Instructions = [];

    protected nint Address = 0;

    protected Size DrawInstructions(DrawContext view, int tx, int y) {
        var origY = y;

        var minWidth = 26 * view.Font.Width;
        var maxWidth = 0;

        using (var brush = new SolidBrush(view.Settings.HiddenColor)) {
            foreach (var instruction in Instructions) {
                y += view.Font.Height;

                var x = AddText(view, tx, y, view.Settings.AddressColor, HotSpot.ReadOnlyId, instruction.Address) + 6;

                view.Graphics.FillRectangle(brush, x, y, 1, view.Font.Height);
                x += 6;

                x = Math.Max(AddText(view, x, y, view.Settings.HexColor, HotSpot.ReadOnlyId, instruction.Data) + 6, x + minWidth);

                view.Graphics.FillRectangle(brush, x, y, 1, view.Font.Height);
                x += 6;

                x = AddText(view, x, y, view.Settings.ValueColor, HotSpot.ReadOnlyId, instruction.Instruction);

                maxWidth = Math.Max(x - tx, maxWidth);
            }
        }

        return new Size(maxWidth, y - origY);
    }

    protected void DisassembleRemoteCode(RemoteProcess process, nint address, out int memorySize) {
        memorySize = 0;

        var disassembler = new Disassembler();
        foreach (var instruction in disassembler.RemoteDisassembleFunction(process, address, 8192)) {
            memorySize += instruction.Length;

            Instructions.Add(new FunctionNodeInstruction {
                Address = instruction.Address.ToString(Constants.AddressHexFormat),
                Data = string.Join(" ", instruction.Data.Take(instruction.Length).Select(b => $"{b:X2}")),
                Instruction = instruction.Instruction
            });
        }
    }

    protected class FunctionNodeInstruction {
        public required string Address { get; set; }
        public required string Data { get; set; }
        public required string Instruction { get; set; }
    }
}
