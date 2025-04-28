using ReClass.Nodes;
using ReClass.UI;

namespace ReClass.Forms;

public partial class ClassSelectionForm : IconForm {
    private readonly List<ClassNode> allClasses;

    public ClassNode? SelectedClass => (ClassNode?)classesListBox.SelectedItem;

    public ClassSelectionForm(IEnumerable<ClassNode> classes) {
        allClasses = classes.ToList();

        InitializeComponent();

        ShowFilteredClasses();
    }

    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);

        GlobalWindowManager.AddWindow(this);
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
        base.OnFormClosed(e);

        GlobalWindowManager.RemoveWindow(this);
    }

    private void filterNameTextBox_TextChanged(object sender, EventArgs e) {
        ShowFilteredClasses();
    }

    private void filterNameTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Down when classesListBox.SelectedIndex + 1 < classesListBox.Items.Count:
                classesListBox.SelectedIndex++;
                break;

            case Keys.Up when classesListBox.SelectedIndex > 0:
                classesListBox.SelectedIndex--;
                break;
        }
    }

    private void classesListBox_SelectedIndexChanged(object sender, EventArgs e) {
        selectButton.Enabled = SelectedClass != null;
    }

    private void classesListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
        if (SelectedClass != null) {
            selectButton.PerformClick();
        }
    }

    private void ShowFilteredClasses() {
        IEnumerable<ClassNode> classes = allClasses;

        if (!string.IsNullOrEmpty(filterNameTextBox.Text)) {
            classes = classes.Where(c => c.Name.IndexOf(filterNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        classesListBox.DataSource = classes.ToList();
    }
}
