namespace ReClass.Native;

public enum SectionCategory {
    Unknown,
    CODE,
    DATA,
    HEAP
}

public record SectionInfo {
    public required nuint Start { get; set; }
    public required nuint End { get; set; }
    public required nuint Size { get; set; }
    public required SectionCategory Category { get; set; }
    public string? Name { get; set; }
    public string? ModuleName { get; set; }
    public string? ModulePath { get; set; }
}
