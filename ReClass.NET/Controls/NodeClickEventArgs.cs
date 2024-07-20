using ReClassNET.Memory;
using ReClassNET.Nodes;

namespace ReClassNET.Controls;

public class NodeClickEventArgs : EventArgs {
    public BaseNode Node { get; }

    public IntPtr Address { get; }

    public MemoryBuffer Memory { get; }

    public MouseButtons Button { get; }

    public Point Location { get; }

    public NodeClickEventArgs(BaseNode node, IntPtr address, MemoryBuffer memory, MouseButtons button, Point location) {
        Node = node;
        Address = address;
        Memory = memory;
        Button = button;
        Location = location;
    }
}

public delegate void NodeClickEventHandler(object sender, NodeClickEventArgs args);
