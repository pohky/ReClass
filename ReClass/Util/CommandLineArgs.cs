namespace ReClass.Util;

public class CommandLineArgs {
    private readonly List<string> fileNames = [];
    private readonly SortedDictionary<string, string> parms = [];

    /// <summary>
    ///     Get the primary file name.
    /// </summary>
    public string? FileName => fileNames.Count < 1 ? null : fileNames[0];

    /// <summary>
    ///     Gets all file names.
    /// </summary>
    public IEnumerable<string> FileNames => fileNames;

    /// <summary>
    ///     Gets all provided parameters.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> Parameters => parms;

    /// <summary>
    ///     Get the value of a command line parameter.
    /// </summary>
    /// <returns>
    ///     Returns <c>null</c> if no parameter with the specified key exists.
    /// </returns>
    public string? this[string strKey] => parms.GetValueOrDefault(strKey.ToLower());

    public CommandLineArgs(string[]? args) {
        if (args == null) {
            return;
        }

        foreach (var str in args) {
            if (string.IsNullOrEmpty(str)) {
                continue;
            }

            var kv = GetParameter(str);
            if (kv.Key.Length == 0) {
                fileNames.Add(kv.Value);
            } else {
                parms[kv.Key] = kv.Value;
            }
        }
    }

    /// <summary>
    ///     Parses the parameter and extracts the key and value.
    /// </summary>
    /// <param name="str">The parameter string to parse.</param>
    /// <returns>
    ///     The parameter split in key and value. An empty key signals a file name.
    /// </returns>
    internal static KeyValuePair<string, string> GetParameter(string str) {
        if (str.StartsWith("--")) {
            str = str.Remove(0, 2);
        } else if (str.StartsWith("-")) {
            str = str.Remove(0, 1);
        } else {
            return new KeyValuePair<string, string>(string.Empty, str);
        }

        var posColon = str.IndexOf(':');
        var posEqual = str.IndexOf('=');

        if (posColon < 0 && posEqual < 0) {
            return new KeyValuePair<string, string>(str.ToLower(), string.Empty);
        }

        var posMin = Math.Min(posColon, posEqual);
        if (posMin < 0) {
            posMin = posColon < 0 ? posEqual : posColon;
        }

        if (posMin <= 0) {
            return new KeyValuePair<string, string>(str.ToLower(), string.Empty);
        }

        var key = str.Substring(0, posMin).ToLower();
        var value = str.Remove(0, posMin + 1);
        return new KeyValuePair<string, string>(key, value);
    }
}
