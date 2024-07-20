using ReClassNET.Logger;

namespace ReClassNET.DataExchange.ReClass;

public interface IReClassImport {
    void Load(string filePath, ILogger logger);
}
