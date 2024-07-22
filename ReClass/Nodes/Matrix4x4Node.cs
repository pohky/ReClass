using ReClass.Controls;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Matrix4x4Node : BaseMatrixNode {
    public override int ValueTypeSize => sizeof(float);

    public override int MemorySize => 16 * ValueTypeSize;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Matrix 4x4";
        icon = Resources.B16x16_Button_Matrix_4x4;
    }

    public override Size Draw(DrawContext context, int x2, int y2) => DrawMatrixType(context, x2, y2, "Matrix (4x4)", 4, 4);

    protected override int CalculateValuesHeight(DrawContext context) => 4 * context.Font.Height;

    public override void Update(HotSpot spot) {
        base.Update(spot);

        Update(spot, 16);
    }
}
