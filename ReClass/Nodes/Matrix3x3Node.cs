using ReClass.Controls;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Matrix3x3Node : BaseMatrixNode {
    public override int ValueTypeSize => sizeof(float);

    public override int MemorySize => 9 * ValueTypeSize;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Matrix 3x3";
        icon = Resources.B16x16_Button_Matrix_3x3;
    }

    public override Size Draw(DrawContext context, int x2, int y2) => DrawMatrixType(context, x2, y2, "Matrix (3x3)", 3, 3);

    protected override int CalculateValuesHeight(DrawContext context) => 3 * context.Font.Height;

    public override void Update(HotSpot spot) {
        base.Update(spot);

        Update(spot, 9);
    }
}
