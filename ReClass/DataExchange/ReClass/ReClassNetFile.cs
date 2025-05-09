using ReClass.Nodes;
using ReClass.Project;

namespace ReClass.DataExchange.ReClass;

public partial class ReClassNetFile(ReClassNetProject project) {
    private static readonly Dictionary<string, Type> buildInStringToTypeMap = new[] {
        typeof(BoolNode),
        typeof(BitFieldNode),
        typeof(EnumNode),
        typeof(ClassInstanceNode),
        typeof(DoubleNode),
        typeof(FloatNode),
        typeof(FunctionNode),
        typeof(FunctionPtrNode),
        typeof(Hex8Node),
        typeof(Hex16Node),
        typeof(Hex32Node),
        typeof(Hex64Node),
        typeof(Int8Node),
        typeof(Int16Node),
        typeof(Int32Node),
        typeof(Int64Node),
        typeof(NIntNode),
        typeof(Matrix3x3Node),
        typeof(Matrix3x4Node),
        typeof(Matrix4x4Node),
        typeof(UInt8Node),
        typeof(UInt16Node),
        typeof(UInt32Node),
        typeof(UInt64Node),
        typeof(NUIntNode),
        typeof(Utf8TextNode),
        typeof(Utf8TextPtrNode),
        typeof(Utf16TextNode),
        typeof(Utf16TextPtrNode),
        typeof(Utf32TextNode),
        typeof(Utf32TextPtrNode),
        typeof(Vector2Node),
        typeof(Vector3Node),
        typeof(Vector4Node),
        typeof(VirtualMethodTableNode),
        typeof(ArrayNode),
        typeof(PointerNode),
        typeof(UnionNode)
    }.ToDictionary(t => t.Name, t => t);

    private static readonly Dictionary<Type, string> buildInTypeToStringMap = buildInStringToTypeMap.ToDictionary(kv => kv.Value, kv => kv.Key);
}
