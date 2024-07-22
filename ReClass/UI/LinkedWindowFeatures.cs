using ReClass.Nodes;

namespace ReClass.UI;

public class LinkedWindowFeatures {
    public static ClassNode CreateClassAtAddress(IntPtr address, bool addDefaultBytes) {
        var classView = Program.MainForm.ProjectView;

        var node = ClassNode.Create();
        node.AddressFormula = address.ToString("X");
        if (addDefaultBytes) {
            node.AddBytes(16 * IntPtr.Size);
        }

        classView.SelectedClass = node;

        return node;
    }

    public static ClassNode CreateDefaultClass() {
        var address = ClassNode.DefaultAddress;

        var mainModule = Program.RemoteProcess.GetModuleByName(Program.RemoteProcess.UnderlayingProcess?.Name);
        if (mainModule != null) {
            address = mainModule.Start;
        }

        return CreateClassAtAddress(address, true);
    }

    public static void SetCurrentClassAddress(IntPtr address) {
        var classNode = Program.MainForm.ProjectView.SelectedClass;
        if (classNode == null) {
            return;
        }

        classNode.AddressFormula = address.ToString("X");
    }
}
