using Microsoft.SqlServer.MessageBox;

namespace ReClassNET.Logger;

public abstract class BaseLogger : ILogger {
    private readonly object sync = new();

    public event NewLogEntryEventHandler NewLogEntry;

    public void Log(Exception ex) {
        Log(LogLevel.Error, ExceptionMessageBox.GetMessageText(ex), ex);
    }

    public void Log(LogLevel level, string message) {
        Log(level, message, null);
    }

    private void Log(LogLevel level, string message, Exception ex) {
        lock (sync) {
            NewLogEntry?.Invoke(level, message, ex);
        }
    }
}
