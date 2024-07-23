using System.Diagnostics;
using ReClass.Native;

namespace ReClass.Memory;

public interface IProcessReader : IRemoteMemoryReader {
    SectionInfo? GetSectionToPointer(nuint address);

    ProcessModule? GetModuleToPointer(nint address);

    ProcessModule? GetModuleByName(string name);
}
