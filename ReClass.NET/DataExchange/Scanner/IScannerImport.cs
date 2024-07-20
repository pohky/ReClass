using System.Diagnostics.Contracts;
using ReClassNET.Logger;
using ReClassNET.MemoryScanner;

namespace ReClassNET.DataExchange.Scanner;

[ContractClass(typeof(ScannerImportContract))]
public interface IScannerImport {
    IEnumerable<MemoryRecord> Load(string filePath, ILogger logger);
}

[ContractClassFor(typeof(IScannerImport))]
internal abstract class ScannerImportContract : IScannerImport {
    public IEnumerable<MemoryRecord> Load(string filePath, ILogger logger) {
        throw new NotImplementedException();
    }
}
