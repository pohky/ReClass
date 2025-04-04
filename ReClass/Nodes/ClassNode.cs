using ReClass.Controls;
using ReClass.UI;

namespace ReClass.Nodes;

public delegate void ClassCreatedEventHandler(ClassNode node);

public class ClassNode : BaseContainerNode {
    public static event ClassCreatedEventHandler? ClassCreated;

    public static nint DefaultAddress { get; } = (nint)0x140000000;
    public static string DefaultAddressFormula { get; } = "140000000";

    public override int MemorySize => Nodes.Sum(n => n.MemorySize);

    protected override bool ShouldCompensateSizeChanges => true;

    public Guid Uuid { get; set; }

    public string AddressFormula { get; set; } = DefaultAddressFormula;

    public event NodeEventHandler? NodesChanged;

    internal ClassNode(bool notifyClassCreated) {
        LevelsOpen.DefaultValue = true;

        Uuid = Guid.NewGuid();

        if (notifyClassCreated) {
            ClassCreated?.Invoke(this);
        }
    }

    public static ClassNode Create() {
        return new ClassNode(true);
    }

    public override void GetUserInterfaceInfo(out string name, out Image icon) {
        throw new InvalidOperationException($"The '{nameof(ClassNode)}' node should not be accessible from the ui.");
    }

    public override bool CanHandleChildNode(BaseNode? node) {
        switch (node) {
            case null:
            case ClassNode _:
            case VirtualMethodNode _:
                return false;
        }

        return true;
    }

    public override void Initialize() {
        AddBytes(IntPtr.Size);
    }

    public override Size Draw(DrawContext context, int x, int y) {
        AddSelection(context, 0, y, context.Font.Height);

        var origX = x;
        var origY = y;

        x = AddOpenCloseIcon(context, x, y);

        var tx = x;

        x = AddIcon(context, x, y, IconProvider.Class, HotSpot.NoneId, HotSpotType.None);
        x = AddText(context, x, y, context.Settings.OffsetColor, 0, AddressFormula) + context.Font.Width;

        x = AddText(context, x, y, context.Settings.TypeColor, HotSpot.NoneId, "Class") + context.Font.Width;
        x = AddText(context, x, y, context.Settings.NameColor, HotSpot.NameId, Name) + context.Font.Width;
        x = AddText(context, x, y, context.Settings.ValueColor, HotSpot.NoneId, $"[0x{MemorySize:X}]") + context.Font.Width;
        x = AddComment(context, x, y);

        y += context.Font.Height;

        var size = new Size(x - origX, y - origY);

        if (LevelsOpen[context.Level]) {
            var childOffset = tx - origX;

            var innerContext = context.Clone();
            innerContext.Level++;
            foreach (var node in Nodes) {
                Size AggregateNodeSizes(Size baseSize, Size newSize) {
                    return new Size(Math.Max(baseSize.Width, newSize.Width), baseSize.Height + newSize.Height);
                }

                Size ExtendWidth(Size baseSize, int width) {
                    return new Size(baseSize.Width + width, baseSize.Height);
                }

                // Draw the node if it is in the visible area.
                if (context.ClientArea.Contains(tx, y)) {
                    var innerSize = node.Draw(innerContext, tx, y);

                    size = AggregateNodeSizes(size, ExtendWidth(innerSize, childOffset));

                    y += innerSize.Height;
                } else {
                    // Otherwise calculate the height...
                    var calculatedHeight = node.CalculateDrawnHeight(innerContext);

                    // and check if the node area overlaps with the visible area...
                    if (new Rectangle(tx, y, 9999999, calculatedHeight).IntersectsWith(context.ClientArea)) {
                        // then draw the node...
                        var innerSize = node.Draw(innerContext, tx, y);

                        size = AggregateNodeSizes(size, ExtendWidth(innerSize, childOffset));

                        y += innerSize.Height;
                    } else {
                        // or skip drawing and just use the calculated height.
                        size = AggregateNodeSizes(size, new Size(0, calculatedHeight));

                        y += calculatedHeight;
                    }
                }
            }
        }

        return size;
    }

    public override int CalculateDrawnHeight(DrawContext context) {
        if (IsHidden) {
            return HiddenHeight;
        }

        var height = context.Font.Height;
        if (LevelsOpen[context.Level]) {
            var nv = context.Clone();
            nv.Level++;
            height += Nodes.Sum(n => n.CalculateDrawnHeight(nv));
        }
        return height;
    }

    public override void Update(HotSpot spot) {
        base.Update(spot);

        if (spot.Id == 0) {
            AddressFormula = spot.Text;
        }
    }

    protected internal override void ChildHasChanged(BaseNode child) {
        NodesChanged?.Invoke(this);
    }
}
