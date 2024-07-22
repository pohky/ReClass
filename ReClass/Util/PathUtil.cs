using System.Reflection;

namespace ReClass.Util;

public class PathUtil {
    private static readonly Lazy<string> executablePath = new(() => {
        string path = null;
        try {
            path = Assembly.GetExecutingAssembly().Location;
        } catch {
            // ignored
        }

        if (string.IsNullOrEmpty(path)) {
            path = Assembly.GetExecutingAssembly().GetName().CodeBase;
            path = FileUrlToPath(path);
        }

        return path;
    });

    private static readonly Lazy<string> executableFolderPath = new(() => Path.GetDirectoryName(executablePath.Value));

    private static readonly Lazy<string> settingsFolderPath = new(() => {
        string applicationData;
        try {
            applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        } catch (Exception) {
            applicationData = executableFolderPath.Value;
        }

        string localApplicationData;
        try {
            localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        } catch (Exception) {
            localApplicationData = applicationData;
        }

        return Path.Combine(localApplicationData, Constants.ApplicationName);
    });
    
    /// <summary>Gets the full pathname of the executable file.</summary>
    public static string ExecutablePath => executablePath.Value;

    /// <summary>Gets the full pathname of the executable folder.</summary>
    public static string ExecutableFolderPath => executableFolderPath.Value;

    /// <summary>Gets the full pathname of the settings folder.</summary>
    /// <remarks>%localappdata%\ReClass.NET\</remarks>
    public static string SettingsFolderPath => settingsFolderPath.Value;

    /// <summary>Converts a file url to a normal path.</summary>
    /// <param name="url">URL of the file.</param>
    /// <returns>The path part of the URL.</returns>
    public static string FileUrlToPath(string url) {
        if (url.StartsWith("file:///", StringComparison.OrdinalIgnoreCase)) {
            url = url.Substring(8);
        }

        url = url.Replace('/', Path.DirectorySeparatorChar);

        return url;
    }
}
