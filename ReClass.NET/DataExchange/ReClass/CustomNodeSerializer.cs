using System.Xml.Linq;
using ReClassNET.Logger;
using ReClassNET.Nodes;

namespace ReClassNET.DataExchange.ReClass;

public delegate BaseNode CreateNodeFromElementHandler(XElement element, BaseNode parent, ILogger logger);

public delegate XElement CreateElementFromNodeHandler(BaseNode node, ILogger logger);

public interface ICustomNodeSerializer {
    /// <summary>Determine if the instance can handle the xml element.</summary>
    /// <param name="element">The xml element to check.</param>
    /// <returns>True if the instance can handle the element, false if not.</returns>
    bool CanHandleElement(XElement element);

    /// <summary>Determine if the instance can handle the node.</summary>
    /// <param name="node">The node to check.</param>
    /// <returns>True if the instance can handle the node, false if not.</returns>
    bool CanHandleNode(BaseNode node);

    /// <summary>
    ///     Creates a node from the xml element. This method gets only called if <see cref="CanHandleElement(XElement)" />
    ///     returned true.
    /// </summary>
    /// <param name="element">The element to create the node from.</param>
    /// <param name="parent">The parent of the node.</param>
    /// <param name="classes">The list of classes which correspond to the node.</param>
    /// <param name="logger">The logger used to output messages.</param>
    /// <param name="defaultHandler">
    ///     The default method which creates a node from an element. Should be called to resolve nodes
    ///     for wrapped inner nodes.
    /// </param>
    /// <returns>The node for the xml element.</returns>
    BaseNode CreateNodeFromElement(XElement element, BaseNode parent, IEnumerable<ClassNode> classes, ILogger logger, CreateNodeFromElementHandler defaultHandler);

    /// <summary>
    ///     Creates a xml element from the node. This method gets only called if <see cref="CanHandleNode(BaseNode)" />
    ///     returned true.
    /// </summary>
    /// <param name="node">The node to create the xml element from.</param>
    /// <param name="logger">The logger used to output messages.</param>
    /// <param name="defaultHandler">
    ///     The default method which creates an element for a node. Should be called to resolve
    ///     elements for wrapped inner nodes.
    /// </param>
    /// <returns>The xml element for the node.</returns>
    XElement CreateElementFromNode(BaseNode node, ILogger logger, CreateElementFromNodeHandler defaultHandler);
}

internal class CustomNodeSerializer {
    private static readonly List<ICustomNodeSerializer> converters = [];

    public static void Add(ICustomNodeSerializer serializer) {
        converters.Add(serializer);
    }

    public static void Remove(ICustomNodeSerializer serializer) {
        converters.Remove(serializer);
    }

    public static ICustomNodeSerializer GetReadConverter(XElement element) {
        return converters.FirstOrDefault(c => c.CanHandleElement(element));
    }

    public static ICustomNodeSerializer GetWriteConverter(BaseNode node) {
        return converters.FirstOrDefault(c => c.CanHandleNode(node));
    }
}
