using ReClass.Memory;

namespace ReClass.AddressParser;

public interface IExecutor {
    IntPtr Execute(IExpression expression, IProcessReader processReader);
}
