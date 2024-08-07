using ReClass.Controls;
using ReClass.Properties;
using ReClass.UI;

namespace ReClass.Nodes;

public class Vector4Node : BaseMatrixNode {
    public override int ValueTypeSize => sizeof(float);

    public override int MemorySize => 4 * ValueTypeSize;

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        name = "Vector4";
        icon = Resources.B16x16_Button_Vector_4;
    }

    public override Size Draw(DrawContext context, int x2, int y2) => DrawVectorType(context, x2, y2, "Vector4", 4);

    protected override int CalculateValuesHeight(DrawContext context) => 0;

    public override void Update(HotSpot spot) {
        base.Update(spot);

        Update(spot, 4);
    }
}
