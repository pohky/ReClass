using System.Text;
using ReClassNET.Controls;
using ReClassNET.Properties;

namespace ReClassNET.Nodes;

public class Utf16TextPtrNode : BaseTextPtrNode {
    public override Encoding Encoding => Encoding.Unicode;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "UTF16 / Unicode Text Pointer";
        icon = Resources.B16x16_Button_UText_Pointer;
    }

    public override Size Draw(DrawContext context, int x, int y) => DrawText(context, x, y, "Text16Ptr");
}
