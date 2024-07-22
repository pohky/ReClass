namespace ReClassNET.Util;

public static class Utils {
    public static T Min<T, U>(T item1, T item2, Func<T, U> keySelector) where U : IComparable {
        return Min(item1, item2, keySelector, Comparer<U>.Default);
    }

    public static T Min<T, U>(T item1, T item2, Func<T, U> keySelector, IComparer<U> comparer) {
        if (comparer.Compare(keySelector(item1), keySelector(item2)) < 0) {
            return item1;
        }
        return item2;
    }

    public static T Max<T, U>(T item1, T item2, Func<T, U> keySelector) where U : IComparable {
        return Max(item1, item2, keySelector, Comparer<U>.Default);
    }

    public static T1 Max<T1, T2>(T1 item1, T1 item2, Func<T1, T2> keySelector, IComparer<T2> comparer) {
        if (comparer.Compare(keySelector(item1), keySelector(item2)) > 0) {
            return item1;
        }
        return item2;
    }

    //thx again stack overflow https://stackoverflow.com/a/1344242
    public static string RandomString(int length) {
        const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(Chars, length)
            .Select(s => s[Program.GlobalRandom.Next(s.Length)]).ToArray());
    }
}
