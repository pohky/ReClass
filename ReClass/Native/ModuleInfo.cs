namespace ReClass.Native;

public record ModuleInfo {
    public required nuint BaseAddress { get; init; }
    public required nuint Size { get; init; }
    public required string Path { get; init; }
}
