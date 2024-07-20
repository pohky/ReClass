using ReClassNET.MemoryScanner.Comparer;

namespace ReClassNET.MemoryScanner;

internal class ComplexScannerWorker : IScannerWorker {
    private readonly IComplexScanComparer comparer;
    private readonly ScanSettings settings;

    public ComplexScannerWorker(ScanSettings settings, IComplexScanComparer comparer) {
        this.settings = settings;
        this.comparer = comparer;
    }

    public IList<ScanResult> Search(byte[] data, int count, CancellationToken ct) {
        var results = new List<ScanResult>();

        foreach (var result in comparer.Compare(data, count)) {
            results.Add(result);

            if (ct.IsCancellationRequested) {
                break;
            }
        }

        return results;
    }

    public IList<ScanResult> Search(byte[] data, int count, IEnumerable<ScanResult> previousResults, CancellationToken ct) {
        var results = new List<ScanResult>();

        foreach (var previousResult in previousResults) {
            if (ct.IsCancellationRequested) {
                break;
            }

            if (comparer.CompareWithPrevious(data, count, previousResult, out var result)) {
                results.Add(result);
            }
        }

        return results;
    }
}
