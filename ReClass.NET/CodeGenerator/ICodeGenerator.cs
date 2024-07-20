using ReClassNET.Logger;
using ReClassNET.Nodes;
using ReClassNET.Project;

namespace ReClassNET.CodeGenerator;

public interface ICodeGenerator {
    /// <summary>The language this generator produces.</summary>
    Language Language { get; }

    /// <summary>Generates code for the classes.</summary>
    /// <param name="classes">The classes to generate code from.</param>
    /// <param name="logger">The logger used to output messages.</param>
    /// <returns>The code for the classes.</returns>
    string GenerateCode(IReadOnlyList<ClassNode> classes, IReadOnlyList<EnumDescription> enums, ILogger logger);
}
