using ReClassNET.Controls;
using ReClassNET.Properties;

namespace ReClassNET.Nodes;

public class FunctionPtrNode : BaseFunctionPtrNode {
    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Function Pointer";
        icon = Resources.B16x16_Button_Function_Pointer;
    }

    public override Size Draw(DrawContext context, int x, int y) => Draw(context, x, y, "FunctionPtr", Name);
}
