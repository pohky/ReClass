using System.Diagnostics;
using ReClass.CodeGenerator;
using ReClass.Controls;
using ReClass.DataExchange.ReClass;
using ReClass.Extensions;
using ReClass.Nodes;
using ReClass.Project;

namespace ReClass.Forms;

public partial class MainForm {
    public void OpenTempCode(IReadOnlyList<ClassNode> partialClasses) {
        OpenTempCode(partialClasses, [], new CppCodeGenerator(currentProject.TypeMapping));
    }

    public void OpenTempCode(ICodeGenerator generator) {
        OpenTempCode(currentProject.Classes, currentProject.Enums, generator);
    }

    public void OpenTempCode(IReadOnlyList<ClassNode> classes, IReadOnlyList<EnumDescription> enums, ICodeGenerator generator) {
        var tempPath = Path.Join(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), generator.Language.GetFileExtension()));

        File.WriteAllText(tempPath, generator.GenerateCode(classes, enums, Program.Logger));
        Program.TempFiles.Add(new FileInfo(tempPath));

        using Process opener = new Process();
        opener.StartInfo.FileName = "explorer";
        opener.StartInfo.Arguments = "\"" + tempPath + "\"";
        opener.Start();
    }

    public void AttachToProcess(string? processName) {
        if (string.IsNullOrEmpty(processName))
            return;

        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
            return;

        AttachToProcess(processes.First());
    }

    public void AttachToProcess(Process process) {
        Program.RemoteProcess.Close();

        Program.RemoteProcess.Open(process);

        Program.Settings.LastProcess = Program.RemoteProcess.UnderlayingProcess.ProcessName;
    }

    /// <summary>Sets the current project.</summary>
    /// <param name="newProject">The new project.</param>
    public void SetProject(ReClassNetProject newProject) {
        if (currentProject == newProject) {
            return;
        }

        if (currentProject != null) {
            ClassNode.ClassCreated -= currentProject.AddClass;
        }

        void UpdateClassNodes(BaseNode node) {
            ProjectView.UpdateClassNode((ClassNode)node);
        }

        currentProject = newProject;
        currentProject.ClassAdded += c => {
            ProjectView.AddClass(c);
            c.NodesChanged += UpdateClassNodes;
            c.NameChanged += UpdateClassNodes;
        };
        currentProject.ClassRemoved += c => {
            ProjectView.RemoveClass(c);
            c.NodesChanged -= UpdateClassNodes;
            c.NameChanged -= UpdateClassNodes;
        };
        currentProject.EnumAdded += e => { ProjectView.AddEnum(e); };

        ClassNode.ClassCreated += currentProject.AddClass;

        ProjectView.Clear();
        ProjectView.AddEnums(currentProject.Enums);
        ProjectView.AddClasses(currentProject.Classes);

        var lastSelectedClassName = currentProject.CustomData.GetString("LastSelectedClass");
        var lastSelectedClass = !string.IsNullOrEmpty(lastSelectedClassName)
            ? currentProject.Classes.FirstOrDefault((c) => c.Name == lastSelectedClassName)
            : null;

        CurrentClassNode = lastSelectedClass ?? currentProject.Classes.FirstOrDefault();
    }

    /// <summary>Opens the <see cref="InputBytesForm" /> and calls <paramref name="callback" /> with the result.</summary>
    /// <param name="title">The title of the input form.</param>
    /// <param name="callback">The function to call afterwards.</param>
    private void AskAddOrInsertBytes(string title, Action<int> callback) {
        var classNode = CurrentClassNode;
        if (classNode == null) {
            return;
        }

        using var ib = new InputBytesForm(classNode.MemorySize) {
            Text = title
        };

        if (ib.ShowDialog() == DialogResult.OK) {
            callback(ib.Bytes);
        }
    }

    /// <summary>
    ///     Adds <paramref name="bytes" /> bytes at the end of the current class.
    /// </summary>
    /// <param name="bytes">Amount of bytes</param>
    public void AddBytesToClass(int bytes) {
        var node = memoryViewControl.GetSelectedNodes().Select(h => h.Node).FirstOrDefault();
        if (node == null) {
            return;
        }

        (node as BaseContainerNode ?? node.GetParentContainer())?.AddBytes(bytes);

        Invalidate();
    }

    /// <summary>
    ///     Inserts <paramref name="bytes" /> bytes at the first selected node to the current class.
    /// </summary>
    /// <param name="bytes">Amount of bytes</param>
    public void InsertBytesInClass(int bytes) {
        var node = memoryViewControl.GetSelectedNodes().Select(h => h.Node).FirstOrDefault();
        if (node == null) {
            return;
        }

        (node as BaseContainerNode ?? node.GetParentContainer())?.InsertBytes(node, bytes);

        Invalidate();
    }

    /// <summary>
    ///     Unselects all selected nodes.
    /// </summary>
    public void ClearSelection() {
        memoryViewControl.ClearSelection();
    }

    /// <summary>Shows an <see cref="OpenFileDialog" /> with all valid file extensions.</summary>
    /// <returns>The path to the selected file or null if no file was selected.</returns>
    public static string ShowOpenProjectFileDialog() {
        using var ofd = new OpenFileDialog();
        ofd.CheckFileExists = true;
        ofd.Filter = $"All ReClass Types |*{ReClassNetFile.FileExtension}|{ReClassNetFile.FormatName} (*{ReClassNetFile.FileExtension})|*{ReClassNetFile.FileExtension}";

        if (ofd.ShowDialog() == DialogResult.OK) {
            return ofd.FileName;
        }

        return null;
    }

    /// <summary>Loads the file as a new project.</summary>
    /// <param name="path">Full pathname of the file.</param>
    public void LoadProjectFromPath(string path) {
        var project = new ReClassNetProject();

        LoadProjectFromPath(path, ref project);

        // If the file is a ReClass.NET file remember the path.
        if (Path.GetExtension(path) == ReClassNetFile.FileExtension) {
            project.Path = path;
        }

        SetProject(project);
    }

    /// <summary>Loads the file into the given project.</summary>
    /// <param name="path">Full pathname of the file.</param>
    /// <param name="project">[in,out] The project.</param>
    private static void LoadProjectFromPath(string path, ref ReClassNetProject project) {
        var import = new ReClassNetFile(project);
        import.Load(path, Program.Logger);
    }

    public void ReplaceSelectedNodesWithType(Type type) {
        var selectedNodes = memoryViewControl.GetSelectedNodes();

        var newSelected = new List<MemoryViewControl.SelectedNodeInfo>(selectedNodes.Count);

        var hotSpotPartitions = selectedNodes
            .WhereNot(s => s.Node is ClassNode)
            .GroupBy(s => s.Node.GetParentContainer())
            .Select(g => new {
                Container = g.Key,
                Partitions = g.OrderBy(s => s.Node.Offset)
                    .GroupWhile((s1, s2) => s1.Node.Offset + s1.Node.MemorySize == s2.Node.Offset)
            });

        foreach (var containerPartitions in hotSpotPartitions) {
            containerPartitions.Container.BeginUpdate();

            foreach (var partition in containerPartitions.Partitions) {
                var hotSpotsToReplace = new Queue<MemoryViewControl.SelectedNodeInfo>(partition);
                while (hotSpotsToReplace.Count > 0) {
                    var selected = hotSpotsToReplace.Dequeue();

                    var node = BaseNode.CreateInstanceFromType(type);

                    var createdNodes = new List<BaseNode>();
                    containerPartitions.Container.ReplaceChildNode(selected.Node, node, ref createdNodes);

                    node.IsSelected = true;

                    var info = new MemoryViewControl.SelectedNodeInfo(node, selected.Process, selected.Memory, selected.Address, selected.Level);

                    newSelected.Add(info);

                    // If more than one node is selected I assume the user wants to replace the complete range with the desired node type.
                    if (selectedNodes.Count > 1) {
                        foreach (var createdNode in createdNodes) {
                            hotSpotsToReplace.Enqueue(new MemoryViewControl.SelectedNodeInfo(createdNode, selected.Process, selected.Memory, selected.Address + createdNode.Offset - node.Offset, selected.Level));
                        }
                    }
                }
            }

            containerPartitions.Container.EndUpdate();
        }

        memoryViewControl.ClearSelection();

        if (newSelected.Count > 0) {
            memoryViewControl.SetSelectedNodes(newSelected);
        }
    }

    private void CopySelectedNodesToClipboard() {
        var selectedNodes = memoryViewControl.GetSelectedNodes();
        if (selectedNodes.Count > 0) {
            ReClassClipboard.Copy(selectedNodes.Select(h => h.Node), Program.Logger);
        }
    }

    private void PasteNodeFromClipboardToSelection() {
        var (classNodes, nodes) = ReClassClipboard.Paste(CurrentProject, Program.Logger);
        foreach (var pastedClassNode in classNodes) {
            if (!CurrentProject.ContainsClass(pastedClassNode.Uuid)) {
                CurrentProject.AddClass(pastedClassNode);
            }
        }

        var selectedNodes = memoryViewControl.GetSelectedNodes();
        if (selectedNodes.Count == 1) {
            var selectedNode = selectedNodes[0].Node;
            var containerNode = selectedNode.GetParentContainer();
            var classNode = selectedNode.GetParentClass();
            if (containerNode != null && classNode != null) {
                containerNode.BeginUpdate();

                foreach (var node in nodes) {
                    if (node is BaseWrapperNode) {
                        var rootWrapper = node.GetRootWrapperNode();
                        Debug.Assert(rootWrapper == node);

                        if (rootWrapper.ShouldPerformCycleCheckForInnerNode()) {
                            if (rootWrapper.ResolveMostInnerNode() is ClassNode innerNode) {
                                if (!IsCycleFree(classNode, innerNode)) {
                                    continue;
                                }
                            }
                        }
                    }

                    containerNode.InsertNode(selectedNode, node);
                }

                containerNode.EndUpdate();
            }
        }
    }

    private void EditSelectedNodeName() {
        var selectedNodes = memoryViewControl.GetSelectedNodes();
        if (selectedNodes.Count == 1) {
            memoryViewControl.ShowNodeNameEditBox(selectedNodes[0].Node);
        }
    }

    private void RemoveSelectedNodes() {
        memoryViewControl.GetSelectedNodes()
            .WhereNot(h => h.Node is ClassNode)
            .ForEach(h => h.Node.GetParentContainer().RemoveNode(h.Node));

        ClearSelection();
    }

    private void HideSelectedNodes() {
        foreach (var hotSpot in memoryViewControl.GetSelectedNodes()) {
            hotSpot.Node.IsHidden = true;
        }

        ClearSelection();
    }

    private void UnhideChildNodes() {
        var selectedNodes = memoryViewControl.GetSelectedNodes();
        if (selectedNodes.Count != 1) {
            return;
        }

        if (!(selectedNodes[0].Node is BaseContainerNode containerNode)) {
            return;
        }

        foreach (var bn in containerNode.Nodes) {
            bn.IsHidden = false;
            bn.IsSelected = false;
        }

        containerNode.IsSelected = false;

        ClearSelection();
    }

    private void UnhideNodesBelow() {
        var selectedNodes = memoryViewControl.GetSelectedNodes();
        if (selectedNodes.Count != 1) {
            return;
        }

        var selectedNode = selectedNodes[0].Node;

        var parentNode = selectedNode.GetParentContainer();
        if (parentNode == null) {
            return;
        }

        var hiddenNodeStartIndex = parentNode.FindNodeIndex(selectedNode) + 1;
        if (hiddenNodeStartIndex >= parentNode.Nodes.Count) {
            return;
        }

        for (var i = hiddenNodeStartIndex; i < parentNode.Nodes.Count; i++) {
            var indexNode = parentNode.Nodes[i];
            if (indexNode.IsHidden) {
                indexNode.IsHidden = false;
                indexNode.IsSelected = false;
            } else {
                break;
            }
        }

        selectedNode.IsSelected = false;

        ClearSelection();
    }

    private void UnhideNodesAbove() {
        var selectedNodes = memoryViewControl.GetSelectedNodes();
        if (selectedNodes.Count != 1) {
            return;
        }

        var selectedNode = selectedNodes[0].Node;

        var parentNode = selectedNode.GetParentContainer();
        if (parentNode == null) {
            return;
        }

        var hiddenNodeStartIndex = parentNode.FindNodeIndex(selectedNode) - 1;
        if (hiddenNodeStartIndex < 0) {
            return;
        }

        for (var i = hiddenNodeStartIndex; i > -1; i--) {
            var indexNode = parentNode.Nodes[i];
            if (indexNode.IsHidden) {
                indexNode.IsHidden = false;
                indexNode.IsSelected = false;
            } else {
                break;
            }
        }

        selectedNode.IsSelected = false;

        ClearSelection();
    }

    private bool IsCycleFree(ClassNode parent, ClassNode node) {
        if (ClassUtil.IsCyclicIfClassIsAccessibleFromParent(parent, node, CurrentProject.Classes)) {
            MessageBox.Show("Invalid operation because this would create a class cycle.", "Cycle Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }

        return true;
    }
}
