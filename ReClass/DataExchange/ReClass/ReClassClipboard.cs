using ReClass.Logger;
using ReClass.Nodes;
using ReClass.Project;

namespace ReClass.DataExchange.ReClass;

public class ReClassClipboard {
    /// <summary>The clipboard format string.</summary>
    private const string ClipboardFormat = "ReClass.NET::Nodes";

    /// <summary>Checks if ReClass.NET nodes are present in the clipboard.</summary>
    public static bool ContainsNodes => Clipboard.ContainsData(ClipboardFormat);

    /// <summary>Copies the nodes to the clipboard.</summary>
    /// <param name="nodes">The nodes to copy.</param>
    /// <param name="logger">The logger.</param>
    public static void Copy(IEnumerable<BaseNode> nodes, ILogger logger) {
        using var ms = new MemoryStream();

        ReClassNetFile.SerializeNodesToStream(ms, nodes, logger);

        Clipboard.SetData(ClipboardFormat, ms.ToArray());
    }

    /// <summary>Pastes nodes from the clipboard.</summary>
    /// <param name="templateProject">The project to resolve class references.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>A list of <see cref="ClassNode" /> and <see cref="BaseNode" />. If no data was present, both lists are empty.</returns>
    public static Tuple<List<ClassNode>, List<BaseNode>> Paste(ReClassNetProject templateProject, ILogger logger) {
        if (ContainsNodes && Clipboard.GetData(ClipboardFormat) is byte[] data) {
            using var ms = new MemoryStream(data);

            return ReClassNetFile.DeserializeNodesFromStream(ms, templateProject, logger);
        }

        return Tuple.Create(new List<ClassNode>(), new List<BaseNode>());
    }
}
