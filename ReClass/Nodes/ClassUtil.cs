using ReClass.Util;

namespace ReClass.Nodes;

public class ClassUtil {
    /// <summary>
    ///     Tests if the class to check can be inserted into the parent class without creating a cycle.
    /// </summary>
    /// <param name="parent">The class into which </param>
    /// <param name="classToCheck">The class which should get inserted.</param>
    /// <param name="classes">An enumeration of all available classes.</param>
    /// <returns>True if a cycle is detected, false otherwise.</returns>
    public static bool IsCyclicIfClassIsAccessibleFromParent(ClassNode parent, ClassNode classToCheck, IEnumerable<ClassNode> classes) {
        var graph = new DirectedGraph<ClassNode>();
        graph.AddVertices(classes);

        graph.AddEdge(parent, classToCheck);

        foreach (var c in graph.Vertices) {
            foreach (var wrapperNode in c.Nodes.OfType<BaseWrapperNode>()) {
                if (wrapperNode.ShouldPerformCycleCheckForInnerNode() && wrapperNode.ResolveMostInnerNode() is ClassNode classNode) {
                    graph.AddEdge(c, classNode);
                }
            }
        }

        return graph.ContainsCycle();
    }
}
