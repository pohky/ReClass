using System.Resources;
using ReClass.Forms;
using ReClass.Logger;
using ReClass.Memory;

namespace ReClass.Plugins;

internal sealed class DefaultPluginHost : IPluginHost {
    public DefaultPluginHost(MainForm form, RemoteProcess process, ILogger logger) {
        MainWindow = form;
        Process = process;
        Logger = logger;
    }
    public MainForm MainWindow { get; }

    public ResourceManager Resources => Properties.Resources.ResourceManager;

    public RemoteProcess Process { get; }

    public ILogger Logger { get; }

    public Settings Settings => Program.Settings;
}
