using ReClassNET.Logger;
using ReClassNET.MemoryScanner;

namespace ReClassNET.DataExchange.Scanner;

public interface IScannerExport {
    void Save(IEnumerable<MemoryRecord> records, string filePath, ILogger logger);
}
