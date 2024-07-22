using System.Diagnostics;
using System.Globalization;
using ReClassNET.Core;
using ReClassNET.DataExchange.ReClass;
using ReClassNET.Forms;
using ReClassNET.Logger;
using ReClassNET.Memory;
using ReClassNET.Native;
using ReClassNET.UI;
using ReClassNET.Util;

namespace ReClassNET;

public static class Program {
    public static CommandLineArgs CommandLineArgs { get; private set; }

    public static Settings Settings { get; private set; }

    public static ILogger Logger { get; private set; }

    public static Random GlobalRandom { get; } = new();

    public static RemoteProcess RemoteProcess { get; private set; }

    public static CoreFunctionsManager CoreFunctions => RemoteProcess.CoreFunctions;

    public static MainForm MainForm { get; private set; }

    public static bool DesignMode { get; private set; } = true;

    public static FontEx MonoSpaceFont { get; private set; }

    public static List<FileInfo> TempFiles { get; } = [];

    [STAThread]
    private static void Main(string[] args) {
        DesignMode = false; // The designer doesn't call Main()
        CommandLineArgs = new CommandLineArgs(args);

        if (CommandLineArgs[Constants.CommandLineOptions.FileExtRegister] != null) {
            NativeMethods.RegisterExtension(ReClassNetFile.FileExtension, ReClassNetFile.FileExtensionId, PathUtil.ExecutablePath, Constants.ApplicationName);
            return;
        }

        if (CommandLineArgs[Constants.CommandLineOptions.FileExtUnregister] != null) {
            NativeMethods.UnregisterExtension(ReClassNetFile.FileExtension, ReClassNetFile.FileExtensionId);
            return;
        }

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        try {
            DpiUtil.TrySetDpiFromCurrentDesktop();
        } catch {
            // ignored
        }

        MonoSpaceFont = new FontEx {
            Font = new Font("Courier New", DpiUtil.ScaleIntX(13), GraphicsUnit.Pixel),
            Width = DpiUtil.ScaleIntX(8),
            Height = DpiUtil.ScaleIntY(16)
        };

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        Settings = SettingsSerializer.Load();
        Logger = new GuiLogger();

        if (Settings.RunAsAdmin && !WinUtil.IsAdministrator) {
            WinUtil.RunElevated(Process.GetCurrentProcess().MainModule!.FileName, args.Length > 0 ? string.Join(" ", args) : string.Empty);
            return;
        }

#if !DEBUG
            try
            {
#endif
        using (var coreFunctions = new CoreFunctionsManager()) {
            RemoteProcess = new RemoteProcess(coreFunctions);

            MainForm = new MainForm();

            Application.Run(MainForm);

            RemoteProcess.Dispose();
        }
#if !DEBUG
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
#endif

        SettingsSerializer.Save(Settings);

        // clean up temp files
        foreach (var fileInfo in TempFiles) {
            try {
                fileInfo.Delete();
            } catch { }
        }
    }

    /// <summary>Shows the exception in a special form.</summary>
    /// <param name="ex">The exception.</param>
    public static void ShowException(Exception ex) {
        var closebutton = new TaskDialogButton("Close");

        TaskDialog.ShowDialog(new() {
            Caption = "ReClass Exception",
            SizeToContent = true,
            DefaultButton = closebutton,
            Heading = ex.GetType().Name,
            Text = ex.Message + "\n" + ex.StackTrace ?? "",
            //Icon = TaskDialogIcon.Information, -- will invoke a beep
            Buttons = [closebutton]
        });
    }
}
