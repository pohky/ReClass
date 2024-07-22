using System.ComponentModel;
using System.Diagnostics;
using ReClass.AddressParser;
using ReClass.CodeGenerator;
using ReClass.Controls;
using ReClass.DataExchange.ReClass;
using ReClass.Extensions;
using ReClass.Memory;
using ReClass.Nodes;
using ReClass.Plugins;
using ReClass.Project;
using ReClass.UI;
using ReClass.Util;
using ReClass.Util.Conversion;

namespace ReClass.Forms;

public partial class MainForm : IconForm {
    private readonly IconProvider iconProvider = new();

    private readonly MemoryBuffer memoryViewBuffer = new();
    private readonly PluginManager pluginManager;

    private ClassNode? currentClassNode;

    private ReClassNetProject currentProject;
    public ReClassNetProject CurrentProject => currentProject;

    public ProjectView ProjectView { get; private set; }

    public MenuStrip MainMenu => mainMenuStrip;

    public ClassNode? CurrentClassNode {
        get => currentClassNode;
        set {
            currentClassNode = value;

            ProjectView.SelectedClass = value;

            memoryViewControl.Reset();
            memoryViewControl.Invalidate();
        }
    }

    public MainForm() {
        InitializeComponent();
        UpdateWindowTitle();

        mainMenuStrip.Renderer = new CustomToolStripProfessionalRenderer(true, true);
        toolStrip.Renderer = new CustomToolStripProfessionalRenderer(true, false);
        isLittleEndianToolStripMenuItem.Checked = BitConverter.IsLittleEndian;

        Program.RemoteProcess.ProcessAttached += sender => {
            var text = $"{sender.UnderlayingProcess!.ProcessName} (ID: {sender.UnderlayingProcess.Id})";
            processInfoToolStripStatusLabel.Text = text;
            UpdateWindowTitle(text);

        };
        Program.RemoteProcess.ProcessClosed += sender => {
            UpdateWindowTitle();
            processInfoToolStripStatusLabel.Text = "No process selected";
        };

        pluginManager = new PluginManager(new DefaultPluginHost(this, Program.RemoteProcess, Program.Logger));
    }

    private void UpdateWindowTitle(string? extra = null) {
        var title = Program.Settings.RandomizeWindowTitle ? Utils.RandomString(Program.GlobalRandom.Next(15, 20)) : Constants.ApplicationName;
        if (!string.IsNullOrEmpty(extra)) {
            title += $" - {extra}";
        }
        Text = title;
    }

    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);

        GlobalWindowManager.AddWindow(this);

        pluginManager.LoadAllPlugins(Path.Combine(Application.StartupPath, Constants.PluginsFolder), Program.Logger);

        toolStrip.Items.AddRange(NodeTypesBuilder.CreateToolStripButtons(ReplaceSelectedNodesWithType).ToArray());
        changeTypeToolStripMenuItem.DropDownItems.AddRange(NodeTypesBuilder.CreateToolStripMenuItems(ReplaceSelectedNodesWithType, false).ToArray());

        var createDefaultProject = true;

        if (!string.IsNullOrEmpty(Program.CommandLineArgs.FileName)) {
            try {
                LoadProjectFromPath(Program.CommandLineArgs.FileName);

                createDefaultProject = false;
            } catch (Exception ex) {
                Program.Logger.Log(ex);
            }
        }

        if (!string.IsNullOrEmpty(Program.CommandLineArgs[Constants.CommandLineOptions.AttachTo])) {
            AttachToProcess(Program.CommandLineArgs[Constants.CommandLineOptions.AttachTo]);
        }

        if (createDefaultProject) {
            SetProject(new ReClassNetProject());

            LinkedWindowFeatures.CreateDefaultClass();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
        pluginManager.UnloadAllPlugins();

        GlobalWindowManager.RemoveWindow(this);

        base.OnFormClosed(e);
    }

    private void MainForm_DragEnter(object sender, DragEventArgs e) {
        if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any()) {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void MainForm_DragDrop(object sender, DragEventArgs e) {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any()) {
            try {
                var path = files.First();

                LoadProjectFromPath(path);
            } catch (Exception ex) {
                Program.Logger.Log(ex);
            }
        }
    }

    private void classesView_ClassSelected(object sender, ClassNode? node) {
        CurrentClassNode = node;
        CurrentProject.CustomData.SetString("LastSelectedClass", node?.Name ?? string.Empty);
    }

    private void memoryViewControl_KeyDown(object sender, KeyEventArgs args) {
        switch (args.KeyCode) {
            case Keys.C when args.Control:
                CopySelectedNodesToClipboard();
                break;
            case Keys.V when args.Control:
                PasteNodeFromClipboardToSelection();
                break;

            case Keys.Delete:
                RemoveSelectedNodes();
                break;

            case Keys.F2:
                EditSelectedNodeName();
                break;
        }
    }

    private void memoryViewControl_SelectionChanged(object sender, EventArgs e) {
        if (!(sender is MemoryViewControl memoryView)) {
            return;
        }

        var selectedNodes = memoryView.GetSelectedNodes();

        var node = selectedNodes.FirstOrDefault()?.Node;
        var parentContainer = node?.GetParentContainer();
        var nodeIsClass = node is ClassNode;
        var isContainerNode = node is BaseContainerNode;

        addBytesToolStripDropDownButton.Enabled = parentContainer != null || isContainerNode;
        insertBytesToolStripDropDownButton.Enabled = selectedNodes.Count == 1 && parentContainer != null && !isContainerNode;

        var enabled = selectedNodes.Count > 0 && !nodeIsClass;
        toolStrip.Items.OfType<TypeToolStripButton>().ForEach(b => b.Enabled = enabled);
    }

    private void memoryViewControl_ChangeClassTypeClick(object sender, NodeClickEventArgs e) {
        var classes = CurrentProject.Classes.OrderBy(c => c.Name);

        if (e.Node is FunctionNode functionNode) {
            var noneClass = new ClassNode(false) {
                Name = "None"
            };

            using var csf = new ClassSelectionForm(classes.Prepend(noneClass));

            if (csf.ShowDialog() == DialogResult.OK) {
                var selectedClassNode = csf.SelectedClass;
                if (selectedClassNode != null) {
                    if (selectedClassNode == noneClass) {
                        selectedClassNode = null;
                    }

                    functionNode.BelongsToClass = selectedClassNode;
                }
            }
        } else if (e.Node is BaseWrapperNode refNode) {
            using var csf = new ClassSelectionForm(classes);

            if (csf.ShowDialog() == DialogResult.OK) {
                var selectedClassNode = csf.SelectedClass;
                if (selectedClassNode != null && refNode.CanChangeInnerNodeTo(selectedClassNode)) {
                    if (!refNode.GetRootWrapperNode().ShouldPerformCycleCheckForInnerNode() || IsCycleFree(e.Node.GetParentClass(), selectedClassNode)) {
                        refNode.ChangeInnerNode(selectedClassNode);
                    }
                }
            }
        }
    }

    private void memoryViewControl_ChangeWrappedTypeClick(object sender, NodeClickEventArgs e) {
        if (e.Node is BaseWrapperNode wrapperNode) {
            var items = NodeTypesBuilder.CreateToolStripMenuItems(t => {
                var node = BaseNode.CreateInstanceFromType(t);
                if (wrapperNode.CanChangeInnerNodeTo(node)) {
                    wrapperNode.ChangeInnerNode(node);
                }
            }, wrapperNode.CanChangeInnerNodeTo(null));

            var menu = new ContextMenuStrip();
            menu.Items.AddRange(items.ToArray());
            menu.Show(this, e.Location);
        }
    }

    private void memoryViewControl_ChangeEnumTypeClick(object sender, NodeClickEventArgs e) {
        if (e.Node is EnumNode enumNode) {
            using var csf = new EnumSelectionForm(CurrentProject);

            var size = enumNode.Enum.Size;

            if (csf.ShowDialog() == DialogResult.OK) {
                var @enum = csf.SelectedItem;
                if (@enum != null) {
                    enumNode.ChangeEnum(@enum);
                }
            }

            if (size != enumNode.Enum.Size) {
                // Update the parent container because the enum size has changed.
                enumNode.GetParentContainer()?.ChildHasChanged(enumNode);
            }

            foreach (var @enum in CurrentProject.Enums) {
                ProjectView.UpdateEnumNode(@enum);
            }
        }
    }

    private void showCodeOfClassToolStripMenuItem2_Click(object sender, EventArgs e) {
        var classNode = ProjectView.SelectedClass;
        if (classNode == null) {
            return;
        }

        OpenTempCode([classNode]);
    }

    private void enableHierarchyViewToolStripMenuItem_Click(object sender, EventArgs e) {
        var isChecked = !enableHierarchyViewToolStripMenuItem.Checked;

        enableHierarchyViewToolStripMenuItem.Checked = isChecked;

        expandAllClassesToolStripMenuItem.Enabled = collapseAllClassesToolStripMenuItem.Enabled = isChecked;

        ProjectView.EnableClassHierarchyView = isChecked;
    }

    private void autoExpandHierarchyViewToolStripMenuItem_Click(object sender, EventArgs e) {
        var isChecked = !autoExpandHierarchyViewToolStripMenuItem.Checked;

        autoExpandHierarchyViewToolStripMenuItem.Checked = isChecked;

        ProjectView.AutoExpandClassNodes = isChecked;
    }

    private void expandAllClassesToolStripMenuItem_Click(object sender, EventArgs e) {
        ProjectView.ExpandAllClassNodes();
    }

    private void collapseAllClassesToolStripMenuItem_Click(object sender, EventArgs e) {
        ProjectView.CollapseAllClassNodes();
    }

    private void removeUnusedClassesToolStripMenuItem_Click(object sender, EventArgs e) {
        CurrentProject.RemoveUnusedClasses();
    }

    private void deleteClassToolStripMenuItem_Click(object sender, EventArgs e) {
        var classNode = ProjectView.SelectedClass;
        if (classNode == null) {
            return;
        }

        try {
            CurrentProject.Remove(classNode);
        } catch (ClassReferencedException ex) {
            Program.Logger.Log(ex);
        }
    }

    private void editEnumsToolStripMenuItem_Click(object sender, EventArgs e) {
        using var elf = new EnumListForm(currentProject);

        elf.ShowDialog();
    }

    private void editEnumToolStripMenuItem_Click(object sender, EventArgs e) {
        var @enum = ProjectView.SelectedEnum;
        if (@enum != null) {
            using var eef = new EnumEditorForm(@enum);

            eef.ShowDialog();
        }
    }

    private void showEnumsToolStripMenuItem_Click(object sender, EventArgs e) {
        using var elf = new EnumListForm(currentProject);

        elf.ShowDialog();
    }

    private void memoryViewControl_DrawContextRequested(object sender, DrawContextRequestEventArgs args) {
        var process = Program.RemoteProcess;

        var classNode = CurrentClassNode;
        if (classNode == null)
            return;

        memoryViewBuffer.Size = classNode.MemorySize;

        IntPtr address;
        try {
            address = process.ParseAddress(classNode.AddressFormula);
        } catch (ParseException) {
            address = IntPtr.Zero;
        }
        memoryViewBuffer.UpdateFrom(process, address);

        args.Settings = Program.Settings;
        args.IconProvider = iconProvider;
        args.Process = process;
        args.Memory = memoryViewBuffer;
        args.Node = classNode;
        args.BaseAddress = address;
    }

    #region Menustrip

    private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
        var lastProcess = Program.Settings.LastProcess;
        if (string.IsNullOrEmpty(lastProcess)) {
            reattachToProcessToolStripMenuItem.Visible = false;
        } else {
            reattachToProcessToolStripMenuItem.Visible = true;
            reattachToProcessToolStripMenuItem.Text = $"Re-Attach to '{lastProcess}'";
        }
    }

    private void reattachToProcessToolStripMenuItem_Click(object sender, EventArgs e) {
        var lastProcess = Program.Settings.LastProcess;
        if (string.IsNullOrEmpty(lastProcess)) {
            return;
        }

        AttachToProcess(lastProcess);
    }

    private void detachToolStripMenuItem_Click(object sender, EventArgs e) {
        Program.RemoteProcess.Close();
    }

    private void newClassToolStripButton_Click(object sender, EventArgs e) {
        LinkedWindowFeatures.CreateDefaultClass();
    }

    private void openProjectToolStripMenuItem_Click(object sender, EventArgs e) {
        try {
            var path = ShowOpenProjectFileDialog();
            if (path != null) {
                LoadProjectFromPath(path);
            }
        } catch (Exception ex) {
            Program.Logger.Log(ex);
        }
    }

    private void mergeWithProjectToolStripMenuItem_Click(object sender, EventArgs e) {
        try {
            var path = ShowOpenProjectFileDialog();
            if (path != null) {
                LoadProjectFromPath(path, ref currentProject);
            }
        } catch (Exception ex) {
            Program.Logger.Log(ex);
        }
    }

    private void goToClassToolStripMenuItem_Click(object sender, EventArgs e) {
        using var csf = new ClassSelectionForm(currentProject.Classes.OrderBy(c => c.Name));

        if (csf.ShowDialog() == DialogResult.OK) {
            var selectedClassNode = csf.SelectedClass;
            if (selectedClassNode != null) {
                ProjectView.SelectedClass = selectedClassNode;
            }
        }
    }

    private void clearProjectToolStripMenuItem_Click(object sender, EventArgs e) {
        SetProject(new ReClassNetProject());
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
        if (!currentProject.Classes.Any()) {
            return;
        }

        if (string.IsNullOrEmpty(currentProject.Path)) {
            saveAsToolStripMenuItem_Click(sender, e);

            return;
        }

        var file = new ReClassNetFile(currentProject);
        file.Save(currentProject.Path, Program.Logger);
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
        if (!currentProject.Classes.Any()) {
            return;
        }

        using var sfd = new SaveFileDialog {
            DefaultExt = ReClassNetFile.FileExtension,
            Filter = $"{ReClassNetFile.FormatName} (*{ReClassNetFile.FileExtension})|*{ReClassNetFile.FileExtension}"
        };

        if (sfd.ShowDialog() == DialogResult.OK) {
            currentProject.Path = sfd.FileName;

            saveToolStripMenuItem_Click(sender, e);
        }
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
        using var sd = new SettingsForm(Program.Settings, CurrentProject.TypeMapping);

        sd.ShowDialog();
    }

    private void pluginsToolStripButton_Click(object sender, EventArgs e) {
        using var pf = new PluginForm(pluginManager);

        pf.ShowDialog();
    }

    private void quitToolStripMenuItem_Click(object sender, EventArgs e) {
        Close();
    }

    private void namedAddressesToolStripMenuItem_Click(object sender, EventArgs e) {
        new NamedAddressesForm(Program.RemoteProcess).Show();
    }

    private void isLittleEndianToolStripMenuItem_Click(object sender, EventArgs e) {
        Program.RemoteProcess.BitConverter = isLittleEndianToolStripMenuItem.Checked ? EndianBitConverter.Little : EndianBitConverter.Big;
    }

    private void ControlRemoteProcessToolStripMenuItem_Click(object sender, EventArgs e) {
        if (!Program.RemoteProcess.IsValid) {
            return;
        }

        var action = ControlRemoteProcessAction.Terminate;
        if (sender == resumeProcessToolStripMenuItem) {
            action = ControlRemoteProcessAction.Resume;
        } else if (sender == suspendProcessToolStripMenuItem) {
            action = ControlRemoteProcessAction.Suspend;
        }

        Program.RemoteProcess.ControlRemoteProcess(action);
    }

    private void cleanUnusedClassesToolStripMenuItem_Click(object sender, EventArgs e) {
        currentProject.RemoveUnusedClasses();
    }

    private void generateCppCodeToolStripMenuItem_Click(object sender, EventArgs e) {
        OpenTempCode(new CppCodeGenerator(currentProject.TypeMapping));
    }

    private void generateCSharpCodeToolStripMenuItem_Click(object sender, EventArgs e) {
        OpenTempCode(new CSharpCodeGenerator());
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
        using var af = new AboutForm();

        af.ShowDialog();
    }

    #endregion

    #region Toolstrip

    private void attachToProcessToolStripSplitButton_ButtonClick(object sender, EventArgs e) {
        using var pb = new ProcessBrowserForm(Program.Settings.LastProcess);

        if (pb.ShowDialog() == DialogResult.OK && pb.SelectedProcess != null) {
            AttachToProcess(pb.SelectedProcess);
        }
    }

    private void attachToProcessToolStripSplitButton_DropDownClosed(object sender, EventArgs e) {
        attachToProcessToolStripSplitButton.DropDownItems.Clear();
    }

    private void attachToProcessToolStripSplitButton_DropDownOpening(object sender, EventArgs e) {
        attachToProcessToolStripSplitButton.DropDownItems.AddRange(
            Process.GetProcesses()
                .OrderBy(p => p.ProcessName).ThenBy(p => p.Id)
                .Select(p => new ToolStripMenuItem($"[{p.Id}] {p.ProcessName}", p.GetIcon(), (sender2, e2) => AttachToProcess(p)))
                .Cast<ToolStripItem>()
                .ToArray()
        );
    }

    private void selectedNodeContextMenuStrip_Opening(object sender, CancelEventArgs e) {
        var selectedNodes = memoryViewControl.GetSelectedNodes();

        var count = selectedNodes.Count;
        var node = selectedNodes.Select(s => s.Node).FirstOrDefault();
        var parentNode = node?.GetParentContainer();

        var nodeIsClass = node is ClassNode;
        var nodeIsContainer = node is BaseContainerNode;
        var nodeIsSearchableValueNode = node switch {
            BaseHexNode _ => true,
            FloatNode _ => true,
            DoubleNode _ => true,
            Int8Node _ => true,
            UInt8Node _ => true,
            Int16Node _ => true,
            UInt16Node _ => true,
            Int32Node _ => true,
            UInt32Node _ => true,
            Int64Node _ => true,
            UInt64Node _ => true,
            NIntNode _ => true,
            NUIntNode _ => true,
            Utf8TextNode _ => true,
            Utf16TextNode _ => true,
            Utf32TextNode _ => true,
            _ => false
        };

        addBytesToolStripMenuItem.Enabled = parentNode != null || nodeIsContainer;
        insertBytesToolStripMenuItem.Enabled = count == 1 && parentNode != null && !nodeIsContainer;

        changeTypeToolStripMenuItem.Enabled = count > 0 && !nodeIsClass;

        createClassFromNodesToolStripMenuItem.Enabled = count > 0 && !nodeIsClass;
        dissectNodesToolStripMenuItem.Enabled = count > 0 && !nodeIsClass;

        pasteNodesToolStripMenuItem.Enabled = count == 1 && ReClassClipboard.ContainsNodes;
        removeToolStripMenuItem.Enabled = !nodeIsClass;

        copyAddressToolStripMenuItem.Enabled = !nodeIsClass;

        showCodeOfClassToolStripMenuItem.Enabled = nodeIsClass;
        shrinkClassToolStripMenuItem.Enabled = nodeIsClass;

        hideNodesToolStripMenuItem.Enabled = selectedNodes.All(h => !(h.Node is ClassNode));

        unhideChildNodesToolStripMenuItem.Enabled = count == 1 && node is BaseContainerNode bcn && bcn.Nodes.Any(n => n.IsHidden);
        unhideNodesAboveToolStripMenuItem.Enabled = count == 1 && parentNode != null && parentNode.TryGetPredecessor(node, out var predecessor) && predecessor.IsHidden;
        unhideNodesBelowToolStripMenuItem.Enabled = count == 1 && parentNode != null && parentNode.TryGetSuccessor(node, out var successor) && successor.IsHidden;
    }

    private void addBytesToolStripMenuItem_Click(object sender, EventArgs e) {
        if (!(sender is IntegerToolStripMenuItem item)) {
            return;
        }

        AddBytesToClass(item.Value);
    }

    private void addXBytesToolStripMenuItem_Click(object sender, EventArgs e) {
        AskAddOrInsertBytes("Add Bytes", AddBytesToClass);
    }

    private void insertBytesToolStripMenuItem_Click(object sender, EventArgs e) {
        if (!(sender is IntegerToolStripMenuItem item)) {
            return;
        }

        InsertBytesInClass(item.Value);
    }

    private void insertXBytesToolStripMenuItem_Click(object sender, EventArgs e) {
        AskAddOrInsertBytes("Insert Bytes", InsertBytesInClass);
    }

    private void createClassFromNodesToolStripMenuItem_Click(object sender, EventArgs e) {
        var selectedNodes = memoryViewControl.GetSelectedNodes();

        if (selectedNodes.Count > 0 && !(selectedNodes[0].Node is ClassNode)) {
            if (selectedNodes[0].Node.GetParentContainer() is ClassNode parentNode) {
                var newClassNode = ClassNode.Create();
                selectedNodes.Select(h => h.Node).ForEach(newClassNode.AddNode);

                var classInstanceNode = new ClassInstanceNode();
                classInstanceNode.ChangeInnerNode(newClassNode);

                parentNode.InsertNode(selectedNodes[0].Node, classInstanceNode);

                selectedNodes.Select(h => h.Node).ForEach(c => parentNode.RemoveNode(c));

                ClearSelection();
            }
        }
    }

    private void dissectNodesToolStripMenuItem_Click(object sender, EventArgs e) {
        var hexNodes = memoryViewControl.GetSelectedNodes().Where(h => h.Node is BaseHexNode).ToList();
        if (!hexNodes.Any()) {
            return;
        }

        foreach (var g in hexNodes.GroupBy(n => n.Node.GetParentContainer())) {
            NodeDissector.DissectNodes(g.Select(h => (BaseHexNode)h.Node), Program.RemoteProcess, g.First().Memory);
        }

        ClearSelection();
    }

    private void copyNodeToolStripMenuItem_Click(object sender, EventArgs e) {
        CopySelectedNodesToClipboard();
    }

    private void pasteNodesToolStripMenuItem_Click(object sender, EventArgs e) {
        PasteNodeFromClipboardToSelection();
    }

    private void removeToolStripMenuItem_Click(object sender, EventArgs e) {
        RemoveSelectedNodes();
    }

    private void hideNodesToolStripMenuItem_Click(object sender, EventArgs e) {
        HideSelectedNodes();
    }

    private void unhideChildNodesToolStripMenuItem_Click(object sender, EventArgs e) {
        UnhideChildNodes();
    }

    private void unhideNodesAboveToolStripMenuItem_Click(object sender, EventArgs e) {
        UnhideNodesAbove();
    }

    private void unhideNodesBelowToolStripMenuItem_Click(object sender, EventArgs e) {
        UnhideNodesBelow();
    }

    private void copyAddressToolStripMenuItem_Click(object sender, EventArgs e) {
        var selectedNodes = memoryViewControl.GetSelectedNodes();
        if (selectedNodes.Count > 0) {
            Clipboard.SetText(selectedNodes.First().Address.ToString("X"));
        }
    }

    private void showCodeOfClassToolStripMenuItem_Click(object sender, EventArgs e) {
        if (memoryViewControl.GetSelectedNodes().FirstOrDefault()?.Node is ClassNode node) {
            OpenTempCode([node]);
        }
    }

    private void shrinkClassToolStripMenuItem_Click(object sender, EventArgs e) {
        var node = memoryViewControl.GetSelectedNodes().Select(s => s.Node).FirstOrDefault();
        if (!(node is ClassNode classNode)) {
            return;
        }

        foreach (var nodeToDelete in classNode.Nodes.Reverse().TakeWhile(n => n is BaseHexNode)) {
            classNode.RemoveNode(nodeToDelete);
        }
    }

    #endregion

}
