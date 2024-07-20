namespace ReClassNET.Debugger;

public class BreakpointAlreadySetException : Exception {
    public IBreakpoint Breakpoint { get; }

    public BreakpointAlreadySetException(IBreakpoint breakpoint)
        : base("This breakpoint is already set.") {
        Breakpoint = breakpoint;
    }
}
