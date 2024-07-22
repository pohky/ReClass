using System.Diagnostics;

namespace ReClass.Extensions;

public static class EnumerableExtension {
    [DebuggerStepThrough]
    public static bool None<TSource>(this IEnumerable<TSource> source) {
        return !source.Any();
    }

    [DebuggerStepThrough]
    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        return !source.Any(predicate);
    }

    [DebuggerStepThrough]
    public static IEnumerable<TSource> WhereNot<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        return source.Where(item => predicate(item) == false);
    }

    [DebuggerStepThrough]
    public static int FindIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        var index = 0;
        foreach (var item in source) {
            if (predicate(item)) {
                return index;
            }
            ++index;
        }
        return -1;
    }

    [DebuggerStepThrough]
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> func) {
        foreach (var item in source) {
            func(item);
        }
    }

    [DebuggerStepThrough]
    public static IEnumerable<TSource> Traverse<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> childSelector) {
        var queue = new Queue<TSource>(source);
        while (queue.Count > 0) {
            var next = queue.Dequeue();

            yield return next;

            foreach (var child in childSelector(next)) {
                queue.Enqueue(child);
            }
        }
    }

    [DebuggerStepThrough]
    public static IEnumerable<TSource> TakeWhileInclusive<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        foreach (var item in source) {
            yield return item;

            if (!predicate(item)) {
                yield break;
            }
        }
    }

    [DebuggerStepThrough]
    public static bool IsEquivalentTo<T>(this IEnumerable<T> source, IEnumerable<T> other) {
        var expected = new List<T>(source);

        if (other.Any(item => !expected.Remove(item))) {
            return false;
        }

        return expected.Count == 0;
    }

    /// <summary>
    ///     Scans the source and returns the first element where the predicate matches.
    ///     If the predicate doesn't match the first element of the source is returned.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static TSource PredicateOrFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        var result = default(TSource);
        var first = true;
        foreach (var element in source) {
            if (predicate(element)) {
                return element;
            }
            if (first) {
                result = element;
                first = false;
            }
        }

        if (first) {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        return result;
    }

    public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> source, Func<T, T, bool> condition) {
        using var it = source.GetEnumerator();
        if (it.MoveNext()) {
            var previous = it.Current;
            var list = new List<T> { previous };

            while (it.MoveNext()) {
                var item = it.Current;

                if (condition(previous, item) == false) {
                    yield return list;

                    list = [];
                }

                list.Add(item);

                previous = item;
            }

            yield return list;
        }
    }
}
