namespace ReClass.UI;

public class BindingDisplayWrapper<T> {
    private readonly Func<T, string> toString;
    public T Value { get; }

    public BindingDisplayWrapper(T value, Func<T, string> toString) {
        Value = value;
        this.toString = toString;
    }

    public override string ToString() => toString(Value);
}
