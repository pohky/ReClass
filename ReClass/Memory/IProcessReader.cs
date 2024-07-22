using System.Diagnostics;

namespace ReClass.Memory;

public interface IProcessReader : IRemoteMemoryReader {
    Section? GetSectionToPointer(IntPtr address);

    ProcessModule? GetModuleToPointer(IntPtr address);

    ProcessModule? GetModuleByName(string name);
}
