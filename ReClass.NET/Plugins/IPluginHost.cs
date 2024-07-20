using System.Diagnostics.Contracts;
using System.Resources;
using ReClassNET.Forms;
using ReClassNET.Logger;
using ReClassNET.Memory;

namespace ReClassNET.Plugins;

[ContractClass(typeof(PluginHostContract))]
public interface IPluginHost {
    /// <summary>Gets the main window of ReClass.NET.</summary>
    MainForm MainWindow { get; }

    /// <summary>Gets the resources of ReClass.NET.</summary>
    ResourceManager Resources { get; }

    /// <summary>Gets the process ReClass.NET is attached to.</summary>
    RemoteProcess Process { get; }

    /// <summary>Gets the logger ReClass.NET is using.</summary>
    ILogger Logger { get; }

    /// <summary>Gets the settings ReClass.NET is using.</summary>
    Settings Settings { get; }
}

[ContractClassFor(typeof(IPluginHost))]
internal abstract class PluginHostContract : IPluginHost {
    public ILogger Logger {
        get {
            throw new NotImplementedException();
        }
    }

    public MainForm MainWindow {
        get {
            throw new NotImplementedException();
        }
    }

    public RemoteProcess Process {
        get {
            throw new NotImplementedException();
        }
    }

    public ResourceManager Resources {
        get {
            throw new NotImplementedException();
        }
    }

    public Settings Settings {
        get {
            throw new NotImplementedException();
        }
    }
}
