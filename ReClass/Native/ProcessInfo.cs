namespace ReClass.Native;

public record ProcessInfo {
    public required uint Id { get; init; }
    public required string Name { get; init; }
    public required string Path { get; init; }

    private Lazy<Image?>? _icon;
    public Image? Icon {
        get {
            _icon ??= new(() => System.Drawing.Icon.ExtractAssociatedIcon(Path)?.ToBitmap());
            return _icon.Value;
        }
    }
}
