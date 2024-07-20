using ReClassNET.MemoryScanner.Comparer;

namespace ReClassNET.MemoryScanner;

internal class SimpleScannerWorker : IScannerWorker {
    private readonly ISimpleScanComparer comparer;
    private readonly ScanSettings settings;

    public SimpleScannerWorker(ScanSettings settings, ISimpleScanComparer comparer) {
        this.settings = settings;
        this.comparer = comparer;
    }

    public IList<ScanResult> Search(byte[] data, int count, CancellationToken ct) {
        var results = new List<ScanResult>();

        var endIndex = count - comparer.ValueSize;

        for (var i = 0; i < endIndex; i += settings.FastScanAlignment) {
            if (ct.IsCancellationRequested) {
                break;
            }

            if (comparer.Compare(data, i, out var result)) {
                result.Address = i;

                results.Add(result);
            }
        }

        return results;
    }

    public IList<ScanResult> Search(byte[] data, int count, IEnumerable<ScanResult> previousResults, CancellationToken ct) {
        var results = new List<ScanResult>();

        var endIndex = count - comparer.ValueSize;

        foreach (var previousResult in previousResults) {
            if (ct.IsCancellationRequested) {
                break;
            }

            var offset = previousResult.Address.ToInt32();
            if (offset <= endIndex) {
                if (comparer.Compare(data, offset, previousResult, out var result)) {
                    result.Address = previousResult.Address;

                    results.Add(result);
                }
            }
        }

        return results;
    }
}
