using ReClass.Extensions;
using ReClass.Nodes;

namespace ReClass.Memory;

public class NodeDissector {
    public static void DissectNodes(IEnumerable<BaseHexNode> nodes, IProcessReader reader, MemoryBuffer memory) {
        foreach (var node in nodes) {
            if (GuessNode(node, reader, memory, out var guessedNode)) {
                node.GetParentContainer()?.ReplaceChildNode(node, guessedNode);
            }
        }
    }

    public static bool GuessNode(BaseHexNode node, IProcessReader reader, MemoryBuffer memory, out BaseNode guessedNode) {
        guessedNode = null;

        var offset = node.Offset;
        var is4ByteAligned = offset % 4 == 0;
        var is8ByteAligned = offset % 8 == 0;

        // The node is not aligned, skip it.
        if (!is4ByteAligned) {
            return false;
        }

        var data64 = new UInt64FloatDoubleData {
            Raw1 = memory.ReadInt32(offset),
            Raw2 = memory.ReadInt32(offset + sizeof(int))
        };
        ;
        var data32 = new UInt32FloatData {
            Raw = memory.ReadInt32(offset)
        };

        var raw = memory.ReadBytes(offset, node.MemorySize);
        if (raw.InterpretAsSingleByteCharacter().IsLikelyPrintableData()) {
            guessedNode = new Utf8TextNode();

            return true;
        }
        if (raw.InterpretAsDoubleByteCharacter().IsLikelyPrintableData()) {
            guessedNode = new Utf16TextNode();

            return true;
        }

        if (is8ByteAligned) {
            if (GuessPointerNode(data64.IntPtr, reader, out guessedNode)) {
                return true;
            }
        }

        // 0 could be anything.
        if (data32.IntValue != 0) {
            // If the data represents a reasonable range, it could be a float.
            if (-999999.0f <= data32.FloatValue && data32.FloatValue <= 999999.0f && !data32.FloatValue.IsNearlyEqual(0.0f, 0.001f)) {
                guessedNode = new FloatNode();

                return true;
            }

            if (-999999 <= data32.IntValue && data32.IntValue <= 999999) {
                guessedNode = new Int32Node();

                return true;
            }
        }

        if (is8ByteAligned) {
            if (data64.LongValue != 0) {
                // If the data represents a reasonable range, it could be a double.
                if (-999999.0 <= data64.DoubleValue && data64.DoubleValue <= 999999.0 && !data64.DoubleValue.IsNearlyEqual(0.0, 0.001)) {
                    guessedNode = new DoubleNode();

                    return true;
                }
            }
        }

        return false;
    }

    private static bool GuessPointerNode(IntPtr address, IProcessReader process, out BaseNode node) {
        node = null;

        if (address.IsNull()) {
            return false;
        }

        var section = process.GetSectionToPointer(address);
        if (section == null) {
            return false;
        }

        if (section.Category == SectionCategory.CODE) // If the section contains code, it should be a function pointer.
        {
            node = new FunctionPtrNode();

            return true;
        }
        if (section.Category == SectionCategory.DATA || section.Category == SectionCategory.HEAP) // If the section contains data, it is at least a pointer to a class or something.
        {
            // Check if it is a vtable. Check if the first 3 values are pointers to a code section.
            if (process.GetSectionToPointer(process.ReadRemoteIntPtr(address))?.Category == SectionCategory.CODE
                && process.GetSectionToPointer(process.ReadRemoteIntPtr(address + IntPtr.Size))?.Category == SectionCategory.CODE
                && process.GetSectionToPointer(process.ReadRemoteIntPtr(address + 2 * IntPtr.Size))?.Category == SectionCategory.CODE) {
                node = new VirtualMethodTableNode();

                return true;
            }

            // Check if it is a string.
            var data = process.ReadRemoteMemory(address, IntPtr.Size * 2);
            if (data.Take(IntPtr.Size).InterpretAsSingleByteCharacter().IsLikelyPrintableData()) {
                node = new Utf8TextPtrNode();

                return true;
            }
            if (data.InterpretAsDoubleByteCharacter().IsLikelyPrintableData()) {
                node = new Utf16TextPtrNode();

                return true;
            }

            // Now it could be a pointer to something else but we can't tell. :(
            node = new PointerNode();

            return true;
        }

        return false;
    }
}
