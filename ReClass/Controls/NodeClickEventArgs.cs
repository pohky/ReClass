using ReClass.Memory;
using ReClass.Nodes;

namespace ReClass.Controls;

public class NodeClickEventArgs(BaseNode node, nint address, MemoryBuffer memory, MouseButtons button, Point location) : EventArgs {
    public BaseNode Node { get; } = node;

    public nint Address { get; } = address;

    public MemoryBuffer Memory { get; } = memory;

    public MouseButtons Button { get; } = button;

    public Point Location { get; } = location;
}

public delegate void NodeClickEventHandler(object sender, NodeClickEventArgs args);
