using ReClass.Memory;
using ReClass.Nodes;

namespace ReClass.Controls;

public class DrawContextRequestEventArgs : EventArgs {
    public DateTime CurrentTime { get; set; } = DateTime.UtcNow;

    public Settings? Settings { get; set; }

    public RemoteProcess? Process { get; set; }

    public MemoryBuffer? Memory { get; set; }

    public BaseNode? Node { get; set; }

    public nint BaseAddress { get; set; }
}

public delegate void DrawContextRequestEventHandler(object sender, DrawContextRequestEventArgs args);
