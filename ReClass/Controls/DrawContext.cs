using ReClass.Memory;
using ReClass.UI;

namespace ReClass.Controls;

public class DrawContext {
    public Settings Settings { get; set; }

    public Graphics Graphics { get; set; }
    public required FontEx Font { get; set; }

    public RemoteProcess Process { get; set; }
    public MemoryBuffer Memory { get; set; } = new();

    public DateTime CurrentTime { get; set; }

    public Rectangle ClientArea { get; set; }
    public List<HotSpot> HotSpots { get; set; } = [];
    public nint Address { get; set; }
    public int Level { get; set; }
    public bool MultipleNodesSelected { get; set; }

    public DrawContext Clone() => new() {
        Settings = Settings,
        Graphics = Graphics,
        Font = Font,
        Process = Process,
        Memory = Memory,
        CurrentTime = CurrentTime,
        ClientArea = ClientArea,
        HotSpots = HotSpots,
        Address = Address,
        Level = Level,
        MultipleNodesSelected = MultipleNodesSelected
    };
}
