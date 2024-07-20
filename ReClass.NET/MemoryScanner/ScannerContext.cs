namespace ReClassNET.MemoryScanner;

internal class ScannerContext {
    public byte[] Buffer { get; private set; }
    public IScannerWorker Worker { get; }

    public ScannerContext(IScannerWorker worker, int bufferSize) {
        EnsureBufferSize(bufferSize);

        Worker = worker;
    }

    public void EnsureBufferSize(int size) {
        if (Buffer == null || Buffer.Length < size) {
            Buffer = new byte[size];
        }
    }
}
