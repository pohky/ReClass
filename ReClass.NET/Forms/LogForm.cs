using System.Diagnostics.Contracts;
using ReClassNET.Logger;
using ReClassNET.Properties;
using ReClassNET.UI;

namespace ReClassNET.Forms;

public partial class LogForm : IconForm {

    private readonly List<LogItem> items = [];

    public LogForm() {
        InitializeComponent();

        entriesDataGridView.AutoGenerateColumns = false;
        entriesDataGridView.DataSource = items;
    }

    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);

        GlobalWindowManager.AddWindow(this);
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
        base.OnFormClosed(e);

        GlobalWindowManager.RemoveWindow(this);
    }

    private void RefreshDataBinding() {
        var cm = entriesDataGridView.BindingContext[items] as CurrencyManager;
        cm?.Refresh();
    }

    public void Clear() {
        items.Clear();

        RefreshDataBinding();
    }

    public void Add(LogLevel level, string message, Exception ex) {
        Contract.Requires(message != null);

        Image icon;
        switch (level) {
            case LogLevel.Error:
                icon = Resources.B16x16_Error;
                break;
            case LogLevel.Warning:
                icon = Resources.B16x16_Warning;
                break;
            case LogLevel.Information:
                icon = Resources.B16x16_Information;
                break;
            default:
                icon = Resources.B16x16_Gear;
                break;
        }

        items.Add(new LogItem { Icon = icon, Message = message, Exception = ex });

        RefreshDataBinding();
    }

    private void ShowDetailsForm() {
        var item = entriesDataGridView.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault()?.DataBoundItem as LogItem;
        if (item?.Exception != null) {
            Program.ShowException(item.Exception);
        }
    }

    private class LogItem {
        public Image Icon { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }

    #region Event Handler

    private void copyToClipboardButton_Click(object sender, EventArgs e) {
        Clipboard.SetText(items.Select(i => i.Message).Aggregate((a, b) => $"{a}{Environment.NewLine}{b}"));
    }

    private void closeButton_Click(object sender, EventArgs e) {
        Close();
    }

    private void entriesDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
        ShowDetailsForm();
    }

    private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e) {
        ShowDetailsForm();
    }

    #endregion

}
