namespace ReClass.UI;

public sealed class GlobalWindowManagerEventArgs : EventArgs {
    public Form Form { get; }

    public GlobalWindowManagerEventArgs(Form form) {
        Form = form;
    }
}

public static class GlobalWindowManager {
    private static readonly List<Form> windows = [];

    public static Form TopWindow => windows.Last();
    public static IEnumerable<Form> Windows => windows;

    public static event EventHandler<GlobalWindowManagerEventArgs>? WindowAdded;
    public static event EventHandler<GlobalWindowManagerEventArgs>? WindowRemoved;

    public static void AddWindow(Form form) {
        windows.Add(form);

        form.TopMost = Program.Settings.StayOnTop;

        WindowAdded?.Invoke(null, new GlobalWindowManagerEventArgs(form));
    }

    public static void RemoveWindow(Form form) {
        if (windows.Remove(form)) {
            WindowRemoved?.Invoke(null, new GlobalWindowManagerEventArgs(form));
        }
    }
}
