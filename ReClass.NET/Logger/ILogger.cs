namespace ReClassNET.Logger;

public delegate void NewLogEntryEventHandler(LogLevel level, string message, Exception ex);

public interface ILogger {
    /// <summary>Gets triggered every time a new entry is created.</summary>
    event NewLogEntryEventHandler NewLogEntry;

    /// <summary>Logs the given exception. The <see cref="LogLevel" /> is always set to <see cref="LogLevel.Error" />.</summary>
    /// <param name="ex">The exception to log.</param>
    void Log(Exception ex);

    /// <summary>Logs the given message.</summary>
    /// <param name="level">The level of the message.</param>
    /// <param name="message">The message to log.</param>
    void Log(LogLevel level, string message);
}
