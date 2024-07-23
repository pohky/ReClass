using ReClass.Memory;

namespace ReClass.Nodes;

public interface INodeInfoReader {
    /// <summary>
    ///     Used to print custom informations about a node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="reader">The current <see cref="IRemoteMemoryReader" />.</param>
    /// <param name="memory">The current <see cref="MemoryBuffer" />.</param>
    /// <param name="nodeAddress">The absolute address of the node.</param>
    /// <param name="nodeValue">The memory value of the node as <see cref="nint" />.</param>
    /// <returns>Custom informations about the node or null.</returns>
    string ReadNodeInfo(BaseHexCommentNode node, IRemoteMemoryReader reader, MemoryBuffer memory, nint nodeAddress, nint nodeValue);
}
