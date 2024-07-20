using System.Text;
using ReClassNET.Controls;
using ReClassNET.Properties;

namespace ReClassNET.Nodes;

public class Utf8TextNode : BaseTextNode {
    public override Encoding Encoding => Encoding.UTF8;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "UTF8 / ASCII Text";
        icon = Resources.B16x16_Button_Text;
    }

    public override Size Draw(DrawContext context, int x, int y) => DrawText(context, x, y, "Text8");
}
