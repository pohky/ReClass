using ReClass.Native;

namespace ReClass.Memory;

public interface IProcessReader : IRemoteMemoryReader {
    SectionInfo? GetSectionToPointer(nint address);
    ModuleInfo? GetModuleToPointer(nint address);
    ModuleInfo? GetModuleByName(string name);
}
