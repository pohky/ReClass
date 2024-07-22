using System.Data;
using System.Diagnostics;
using ReClass.Extensions;
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
    public Process SelectedProcess => (processDataGridView.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault()?.DataBoundItem as DataRowView)
        ?.Row
        ?.Field<Process>("process");

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
        dt.Columns.Add("process", typeof(Process));

        var shouldFilter = filterCheckBox.Checked;

        foreach (var p in Process.GetProcesses()) {
            try {
                if (shouldFilter && commonProcesses.Contains(p.ProcessName.ToLower()))
                    continue;

                var row = dt.NewRow();
                row["icon"] = p.GetIcon();
                row["name"] = p.ProcessName;
                row["id"] = p.Id;
                row["path"] = p.MainModule!.FileName;
                row["process"] = p;
                dt.Rows.Add(row);
            } catch {
                // might get access denied for system processes
            }
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
