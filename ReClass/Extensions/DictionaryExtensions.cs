namespace ReClass.Extensions;

public static class DictionaryExtension {
    public static void RemoveWhere<TKey, TValue>(this IDictionary<TKey, TValue> source, Func<KeyValuePair<TKey, TValue>, bool> selector) {
        foreach (var kv in source.Where(selector).ToList()) {
            source.Remove(kv.Key);
        }
    }
}
