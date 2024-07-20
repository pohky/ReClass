using System.Resources;
using ReClassNET.Forms;
using ReClassNET.Logger;
using ReClassNET.Memory;

namespace ReClassNET.Plugins;

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
