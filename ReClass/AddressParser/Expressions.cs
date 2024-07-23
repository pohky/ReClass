namespace ReClass.AddressParser;

public interface IExpression;

public abstract class BinaryExpression(IExpression lhs, IExpression rhs) : IExpression {
    public IExpression Lhs { get; } = lhs;
    public IExpression Rhs { get; } = rhs;
}

public class AddExpression(IExpression lhs, IExpression rhs) : BinaryExpression(lhs, rhs);
public class SubtractExpression(IExpression lhs, IExpression rhs) : BinaryExpression(lhs, rhs);
public class MultiplyExpression(IExpression lhs, IExpression rhs) : BinaryExpression(lhs, rhs);
public class DivideExpression(IExpression lhs, IExpression rhs) : BinaryExpression(lhs, rhs);

public class ConstantExpression(long value) : IExpression {
    public long Value { get; } = value;
}

public abstract class UnaryExpression(IExpression expression) : IExpression {
    public IExpression Expression { get; } = expression;
}

public class NegateExpression(IExpression expression) : UnaryExpression(expression);

public class ReadMemoryExpression(IExpression expression, int byteCount) : UnaryExpression(expression) {
    public int ByteCount { get; } = byteCount;
}

public class ModuleExpression(string name) : IExpression {
    public string Name { get; } = name;
}
