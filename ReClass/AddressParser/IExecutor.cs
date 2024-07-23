using ReClass.Memory;

namespace ReClass.AddressParser;

public interface IExecutor {
    nint Execute(IExpression expression, IProcessReader processReader);
}
