namespace ReClass.Util;

/// <summary>A wrapper for non reference types.</summary>
/// <typeparam name="T">Generic type parameter.</typeparam>
public class ValueTypeWrapper<T> where T : struct {
    public T Value { get; set; }
    public ValueTypeWrapper(T value) {
        Value = value;
    }

    public static implicit operator ValueTypeWrapper<T>(T value) => new(value);
}
