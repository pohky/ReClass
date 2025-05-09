namespace ReClass.Nodes;

public abstract class BaseClassWrapperNode : BaseWrapperNode {
    public override void Initialize() {
        var node = ClassNode.Create();
        node.Initialize();

        ChangeInnerNode(node);
    }

    public override bool CanChangeInnerNodeTo(BaseNode node) => node is ClassNode;
}
