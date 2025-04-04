using ReClass.Forms;

namespace ReClass.Logger;

/// <summary>A logger which displays messages in a form.</summary>
public class GuiLogger : BaseLogger {
    private readonly LogForm form;

    public LogLevel Level { get; set; } = LogLevel.Warning;

    public GuiLogger() {
        form = new LogForm();
        form.FormClosing += delegate (object? sender, FormClosingEventArgs e) {
            form.Clear();

            form.Hide();

            e.Cancel = true;
        };

        NewLogEntry += OnNewLogEntry;
    }

    private void OnNewLogEntry(LogLevel level, string message, Exception? ex) {
        if (level < Level) {
            return;
        }

        ShowForm();

        form.Add(level, message, ex);
    }

    public void ShowForm() {
        if (!form.Visible) {
            form.Show();

            form.BringToFront();
        }
    }
}
