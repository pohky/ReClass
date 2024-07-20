using ReClassNET.Memory;

namespace ReClassNET.Debugger;

public delegate void BreakpointHandler(IBreakpoint breakpoint, ref DebugEvent evt);

public interface IBreakpoint {
    IntPtr Address { get; }

    bool Set(RemoteProcess process);
    void Remove(RemoteProcess process);

    void Handler(ref DebugEvent evt);
}
