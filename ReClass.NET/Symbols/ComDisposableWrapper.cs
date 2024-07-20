using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace ReClassNET.Symbols;

internal class DisposableWrapper : IDisposable {
    protected object Object;

    public DisposableWrapper(object obj) {
        Contract.Requires(obj != null);

        Object = obj;
    }

    public void Dispose() {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    [ContractInvariantMethod]
    private void ObjectInvariants() {
        Contract.Invariant(Object != null);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            Marshal.ReleaseComObject(Object);
        }
    }

    ~DisposableWrapper() {
        Dispose(false);
    }
}

internal class ComDisposableWrapper<T> : DisposableWrapper {
    public T Interface => (T)Object;

    public ComDisposableWrapper(T com)
        : base(com) {
        Contract.Requires(com != null);
    }
}
