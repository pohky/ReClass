namespace ReClass.Native;

public record ModuleInfo {
    public required nint BaseAddress { get; init; }
    public required uint Size { get; init; }
    public required string Path { get; init; }
    public required string Name { get; init; }
}
