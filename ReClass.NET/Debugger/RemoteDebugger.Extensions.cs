namespace ReClassNET.Debugger;

public static class RemoteDebuggerExtensions {
    public static bool AskUserAndAttachDebugger(this RemoteDebugger debugger) {
        return debugger.StartDebuggerIfNeeded(
            () => MessageBox.Show(
                "This will attach the debugger of ReClass.NET to the current process. Continue?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            ) == DialogResult.Yes
        );
    }
}
