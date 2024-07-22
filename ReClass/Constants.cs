namespace ReClass;

public class Constants {
    public static string ApplicationName { get; } = "ReClass";

    public static string ApplicationVersion { get; } = "1.2";

    public static string Author { get; } = "KN4CK3R";

    public static string HomepageUrl { get; } = "https://github.com/ReClassNET/ReClass.NET";

    public static string PluginUrl { get; } = "https://github.com/ReClassNET/ReClass.NET#plugins";

    public static string Platform { get; } = "x64";

    public static string AddressHexFormat { get; } = "X016";

    public static string SettingsFile { get; } = "settings.xml";

    public static string PluginsFolder { get; } = "Plugins";

    public static class CommandLineOptions {
        public static string AttachTo { get; } = "attachto";

        public static string FileExtRegister { get; } = "registerfileext";
        public static string FileExtUnregister { get; } = "unregisterfileext";
    }
}
