namespace ReClass.CodeGenerator;

public enum Language {
    Cpp,
    CSharp
}

public static class LanguageExtensions {
    public static string GetFileExtension(this Language language) {
        return language switch {
            Language.Cpp => "cpp",
            Language.CSharp => "cs",
            _ => throw new NotImplementedException()
        };
    }
}
