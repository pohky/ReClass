using ReClass.Memory;
using ReClass.UI;

namespace ReClass.Forms;

public partial class NamedAddressesForm : IconForm {
    private readonly RemoteProcess process;

    public NamedAddressesForm(RemoteProcess process) {
        this.process = process;

        InitializeComponent();

        DisplayNamedAddresses();
    }

    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);

        GlobalWindowManager.AddWindow(this);
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
        base.OnFormClosed(e);

        GlobalWindowManager.RemoveWindow(this);
    }

    private void DisplayNamedAddresses() {
        namedAddressesListBox.DataSource = process.NamedAddresses
            .Select(kv => new BindingDisplayWrapper<KeyValuePair<nint, string>>(kv, v => $"0x{v.Key.ToString(Constants.AddressHexFormat)}: {v.Value}"))
            .ToList();

        namedAddressesListBox_SelectedIndexChanged(null, null);
    }

    private bool IsValidInput() {
        try {
            var address = process.ParseAddress(addressTextBox.Text.Trim());
            var name = nameTextBox.Text.Trim();

            return address != 0 && !string.IsNullOrEmpty(name);
        } catch {
            return false;
        }
    }

    #region Event Handler

    private void InputTextBox_TextChanged(object sender, EventArgs e) {
        addAddressIconButton.Enabled = IsValidInput();
    }

    private void namedAddressesListBox_SelectedIndexChanged(object sender, EventArgs e) {
        removeAddressIconButton.Enabled = namedAddressesListBox.SelectedIndex != -1;
    }

    private void addAddressIconButton_Click(object sender, EventArgs e) {
        if (!IsValidInput()) {
            return;
        }

        var address = process.ParseAddress(addressTextBox.Text.Trim());
        var name = nameTextBox.Text.Trim();

        process.NamedAddresses[address] = name;

        addressTextBox.Text = nameTextBox.Text = null;

        DisplayNamedAddresses();
    }

    private void removeAddressIconButton_Click(object sender, EventArgs e) {
        if (namedAddressesListBox.SelectedItem is BindingDisplayWrapper<KeyValuePair<nint, string>> namedAddress) {
            process.NamedAddresses.Remove(namedAddress.Value.Key);

            DisplayNamedAddresses();
        }
    }

    #endregion

}
