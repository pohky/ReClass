using Iced.Intel;

namespace ReClass.Memory;

public record DisassembledInstruction(nint Address, int Length, byte[] Data, string Instruction);

public class Disassembler {
    /// <summary>
    ///     Disassembles the code in the given range (<paramref name="address" />, <paramref name="maxLength" />) in the
    ///     remote process until the first 0xCC instruction.
    /// </summary>
    /// <param name="process">The process to read from.</param>
    /// <param name="address">The address of the code.</param>
    /// <param name="maxLength">The maximum maxLength of the code.</param>
    /// <returns>A list of <see cref="DisassembledInstruction" /> which belong to the function.</returns>
    public IReadOnlyList<DisassembledInstruction> RemoteDisassembleFunction(IRemoteMemoryReader process, nint address, int maxLength) {
        var buffer = process.ReadRemoteMemory(address, maxLength);
        return DisassembleFunction(process.Is64Bit ? 64 : 32, buffer, address);
    }

    /// <summary>Disassembles the code in the given data.</summary>
    /// <param name="data">The data to disassemble.</param>
    /// <param name="virtualAddress">
    ///     The virtual address of the code. This allows to decode instructions located anywhere in
    ///     memory even if they are not at their original place.
    /// </param>
    /// <returns>A list of <see cref="DisassembledInstruction" /> which belong to the function.</returns>
    public IReadOnlyList<DisassembledInstruction> DisassembleFunction(int bitness, byte[] data, nint virtualAddress) {
        var decoder = Decoder.Create(bitness, data);
        decoder.IP = (ulong)virtualAddress;
        var endRip = decoder.IP + (uint)data.Length;

        var formatter = new NasmFormatter();
        formatter.Options.HexPrefix = "0x";
        formatter.Options.HexSuffix = null;
        formatter.Options.UppercaseHex = true;
        formatter.Options.FirstOperandCharIndex = 10;
        formatter.Options.SpaceAfterOperandSeparator = true;

        var instructions = new List<DisassembledInstruction>();
        var offset = 0;
        var output = new StringOutput();
        while (decoder.IP < endRip) {
            var instr = decoder.Decode();
            if (instr.IsInvalid)
                break;

            formatter.Format(instr, output);

            instructions.Add(new(
                virtualAddress + offset,
                instr.Length,
                data.Skip(offset).Take(instr.Length).ToArray(),
                output.ToStringAndReset()
            ));

            offset += instr.Length;
        }

        return instructions;
    }
}
