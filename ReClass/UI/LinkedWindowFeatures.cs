using ReClass.Nodes;

namespace ReClass.UI;

public class LinkedWindowFeatures {
    public static ClassNode CreateClassAtAddress(nint address, bool addDefaultBytes) {
        var classView = Program.MainForm.ProjectView;

        var node = ClassNode.Create();
        node.AddressFormula = address.ToString("X");
        if (addDefaultBytes) {
            node.AddBytes(16 * nint.Size);
        }

        classView.SelectedClass = node;

        return node;
    }

    public static ClassNode CreateDefaultClass() {
        var address = ClassNode.DefaultAddress;

        var mainModule = Program.RemoteProcess.UnderlayingProcess?.MainModule;
        if (mainModule != null) {
            address = mainModule.BaseAddress;
        }

        return CreateClassAtAddress(address, true);
    }
}
