using System.Text;
using ReClass.Controls;
using ReClass.Properties;

namespace ReClass.Nodes;

public class Utf16TextNode : BaseTextNode {
    public override Encoding Encoding => Encoding.Unicode;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "UTF16 / Unicode Text";
        icon = Resources.B16x16_Button_UText;
    }

    public override Size Draw(DrawContext context, int x, int y) => DrawText(context, x, y, "Text16");
}
