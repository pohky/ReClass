using System.Text;
using ReClass.Controls;
using ReClass.Properties;

namespace ReClass.Nodes;

public class Utf8TextPtrNode : BaseTextPtrNode {
    public override Encoding Encoding => Encoding.UTF8;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "UTF8 / ASCII Text Pointer";
        icon = Resources.B16x16_Button_Text_Pointer;
    }

    public override Size Draw(DrawContext context, int x, int y) => DrawText(context, x, y, "Text8Ptr");
}
