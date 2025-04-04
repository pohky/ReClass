using System.Windows.Forms.Design;

namespace ReClass.Controls;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
public class IntegerToolStripMenuItem : ToolStripMenuItem {
    public int Value { get; set; }
}

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
public class TypeToolStripMenuItem : ToolStripMenuItem {
    public Type Value { get; set; }
}

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
public class TypeToolStripButton : ToolStripButton {
    public Type Value { get; set; }
}
