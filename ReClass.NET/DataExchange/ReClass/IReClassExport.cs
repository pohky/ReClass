using System.Diagnostics.Contracts;
using ReClassNET.Logger;

namespace ReClassNET.DataExchange.ReClass;

[ContractClass(typeof(ReClassExportContract))]
public interface IReClassExport {
    void Save(string filePath, ILogger logger);

    void Save(Stream output, ILogger logger);
}

[ContractClassFor(typeof(IReClassExport))]
internal abstract class ReClassExportContract : IReClassExport {
    public void Save(string filePath, ILogger logger) {
    }

    public void Save(Stream output, ILogger logger) {
    }
}
