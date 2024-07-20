using System.Diagnostics.Contracts;
using ReClassNET.Memory;

namespace ReClassNET.Debugger;

public sealed class SoftwareBreakpoint : IBreakpoint {

    private readonly BreakpointHandler handler;

    private byte orig;

    public SoftwareBreakpoint(IntPtr address, BreakpointHandler handler) {
        Contract.Requires(handler != null);

        Address = address;

        this.handler = handler;
    }
    public IntPtr Address { get; }

    public bool Set(RemoteProcess process) {
        var temp = new byte[1];
        if (!process.ReadRemoteMemoryIntoBuffer(Address, ref temp)) {
            return false;
        }
        orig = temp[0];

        return process.WriteRemoteMemory(Address, [0xCC]);
    }

    public void Remove(RemoteProcess process) {
        process.WriteRemoteMemory(Address, [orig]);
    }

    public void Handler(ref DebugEvent evt) {
        handler?.Invoke(this, ref evt);
    }

    public override bool Equals(object obj) {
        if (!(obj is SoftwareBreakpoint bp)) {
            return false;
        }

        return Address == bp.Address;
    }

    public override int GetHashCode() => Address.GetHashCode();
}
