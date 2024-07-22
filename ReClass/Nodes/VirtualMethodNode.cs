using ReClass.Controls;

namespace ReClass.Nodes;

public class VirtualMethodNode : BaseFunctionPtrNode {
    public string MethodName => string.IsNullOrEmpty(Name) ? $"Function{Offset / IntPtr.Size}" : Name;

    public VirtualMethodNode() {
        Name = string.Empty;
    }

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        throw new InvalidOperationException($"The '{nameof(VirtualMethodNode)}' node should not be accessible from the ui.");
    }

    public override Size Draw(DrawContext context, int x, int y) => Draw(context, x, y, $"({Offset / IntPtr.Size})", MethodName);
}
