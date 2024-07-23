namespace ReClass.Native;

public enum SectionCategory {
    Unknown,
    CODE,
    DATA,
    HEAP
}

public record SectionInfo {
    public required nint Start { get; set; }
    public required nint End { get; set; }
    public required nint Size { get; set; }
    public required SectionCategory Category { get; set; }
    public string? Name { get; set; }
    public string? ModuleName { get; set; }
    public string? ModulePath { get; set; }
}
