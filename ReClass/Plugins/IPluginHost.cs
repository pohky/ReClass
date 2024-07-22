using System.Resources;
using ReClass.Forms;
using ReClass.Logger;
using ReClass.Memory;

namespace ReClass.Plugins;

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
