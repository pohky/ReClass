using System.Data;
using System.Diagnostics;
using ReClass.Native;
using ReClass.UI;

namespace ReClass.Forms;

public partial class ProcessBrowserForm : IconForm {
    private const string NoPreviousProcess = "No previous process";

    private static readonly string[] commonProcesses = [
        "[system process]",
        "system",
        "svchost.exe",
        "services.exe",
        "wininit.exe",
        "smss.exe",
        "csrss.exe",
        "lsass.exe",
        "winlogon.exe",
        "wininit.exe",
        "dwm.exe"
    ];

    /// <summary>Gets the selected process.</summary>
    public Process? SelectedProcess {
        get {
            var row = processDataGridView.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
            if (row == null)
                return null;

            var dataRow = ((DataRowView)row.DataBoundItem)?.Row;
            if (dataRow == null)
                return null;

            var pid = dataRow.Field<int>("id");
            return Process.GetProcessById(pid);
        }
    }

    public ProcessBrowserForm(string previousProcess) {
        InitializeComponent();

        processDataGridView.AutoGenerateColumns = false;

        previousProcessLinkLabel.Text = string.IsNullOrEmpty(previousProcess) ? NoPreviousProcess : previousProcess;

        RefreshProcessList();

        foreach (var row in processDataGridView.Rows.Cast<DataGridViewRow>()) {
            if (row.Cells[1].Value as string == previousProcess) {
                processDataGridView.CurrentCell = row.Cells[1];
                break;
            }
        }
    }

    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);

        GlobalWindowManager.AddWindow(this);
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
        base.OnFormClosed(e);

        GlobalWindowManager.RemoveWindow(this);
    }

    /// <summary>Queries all processes and displays them.</summary>
    private void RefreshProcessList() {
        var dt = new DataTable();
        dt.Columns.Add("icon", typeof(Image));
        dt.Columns.Add("name", typeof(string));
        dt.Columns.Add("id", typeof(int));
        dt.Columns.Add("path", typeof(string));

        var shouldFilter = filterCheckBox.Checked;

        foreach (var process in NativeMethods.GetProcesses()) {
            if (process.Id == 0) // Idle process
                continue;

            if (shouldFilter && commonProcesses.Contains(process.Name.ToLower()))
                continue;

            var row = dt.NewRow();
            row["icon"] = process.Icon;
            row["name"] = process.Name;
            row["id"] = process.Id;
            row["path"] = process.Path;
            dt.Rows.Add(row);
        }

        dt.DefaultView.Sort = "name ASC";

        processDataGridView.DataSource = dt;

        ApplyFilter();
    }

    private void ApplyFilter() {
        var filter = filterTextBox.Text;
        if (!string.IsNullOrEmpty(filter)) {
            filter = $"name like '%{filter}%' or path like '%{filter}%' or CONVERT(id, System.String) like '%{filter}%'";
        }
        ((DataTable)processDataGridView.DataSource).DefaultView.RowFilter = filter;
    }

    #region Event Handler

    private void filterCheckBox_CheckedChanged(object sender, EventArgs e) {
        RefreshProcessList();
    }

    private void filterTextBox_TextChanged(object sender, EventArgs e) {
        ApplyFilter();
    }

    private void refreshButton_Click(object sender, EventArgs e) {
        RefreshProcessList();
    }

    private void previousProcessLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
        filterTextBox.Text = previousProcessLinkLabel.Text == NoPreviousProcess ? string.Empty : previousProcessLinkLabel.Text;
    }

    private void processDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
        AcceptButton.PerformClick();
    }

    #endregion

}
