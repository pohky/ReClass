using ReClassNET.Native;

namespace ReClassNET.Memory;

public class ProcessInfo {
    private readonly Lazy<Image?> icon;
    public IntPtr Id { get; }
    public string Name { get; }
    public string Path { get; }
    public Image? Icon => icon.Value;

    public ProcessInfo(IntPtr id, string name, string path) {
        Id = id;
        Name = name;
        Path = path;
        icon = new Lazy<Image?>(() => {
            using var i = NativeMethods.GetIconForFile(Path);
            return i?.ToBitmap();
        });
    }
}
