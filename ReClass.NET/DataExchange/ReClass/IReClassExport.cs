using ReClassNET.Logger;

namespace ReClassNET.DataExchange.ReClass;

public interface IReClassExport {
    void Save(string filePath, ILogger logger);

    void Save(Stream output, ILogger logger);
}
