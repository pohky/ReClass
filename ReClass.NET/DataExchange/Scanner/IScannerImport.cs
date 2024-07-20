using ReClassNET.Logger;
using ReClassNET.MemoryScanner;

namespace ReClassNET.DataExchange.Scanner;

public interface IScannerImport {
    IEnumerable<MemoryRecord> Load(string filePath, ILogger logger);
}
