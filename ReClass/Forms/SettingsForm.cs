using ReClass.Controls;
using ReClass.Extensions;
using ReClass.Native;
using ReClass.Project;
using ReClass.Properties;
using ReClass.UI;
using ReClass.Util;

namespace ReClass.Forms;

public partial class SettingsForm : IconForm {
    private readonly Settings settings;
    private readonly CppTypeMapping typeMapping;

    public TabControl SettingsTabControl { get; private set; }

    public SettingsForm(Settings settings, CppTypeMapping typeMapping) {
        this.settings = settings;
        this.typeMapping = typeMapping;

        InitializeComponent();

        var imageList = new ImageList();
        imageList.Images.Add(Resources.B16x16_Gear);
        imageList.Images.Add(Resources.B16x16_Color_Wheel);
        imageList.Images.Add(Resources.B16x16_Settings_Edit);

        SettingsTabControl.ImageList = imageList;
        generalSettingsTabPage.ImageIndex = 0;
        colorsSettingTabPage.ImageIndex = 1;
        typeDefinitionsSettingsTabPage.ImageIndex = 2;

        SetGeneralBindings();
        SetColorBindings();
        SetTypeDefinitionBindings();

        NativeMethods.SetButtonShield(createAssociationButton, true);
        NativeMethods.SetButtonShield(removeAssociationButton, true);
    }

    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);

        GlobalWindowManager.AddWindow(this);
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
        base.OnFormClosed(e);

        GlobalWindowManager.RemoveWindow(this);
    }

    private void createAssociationButton_Click(object sender, EventArgs e) {
        WinUtil.RunElevated(PathUtil.ExecutablePath, $"-{Constants.CommandLineOptions.FileExtRegister}");
    }

    private void removeAssociationButton_Click(object sender, EventArgs e) {
        WinUtil.RunElevated(PathUtil.ExecutablePath, $"-{Constants.CommandLineOptions.FileExtUnregister}");
    }

    private static void SetBinding(IBindableComponent control, string propertyName, object dataSource, string dataMember) {
        control.DataBindings.Add(propertyName, dataSource, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void SetGeneralBindings() {
        SetBinding(stayOnTopCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.StayOnTop));
        stayOnTopCheckBox.CheckedChanged += (_, _2) => GlobalWindowManager.Windows.ForEach(w => w.TopMost = stayOnTopCheckBox.Checked);

        SetBinding(showNodeAddressCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowNodeAddress));
        SetBinding(showNodeOffsetCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowNodeOffset));
        SetBinding(showTextCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowNodeText));
        SetBinding(highlightChangedValuesCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.HighlightChangedValues));

        SetBinding(showFloatCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowCommentFloat));
        SetBinding(showIntegerCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowCommentInteger));
        SetBinding(showPointerCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowCommentPointer));
        SetBinding(showStringCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowCommentString));
        SetBinding(showPluginInfoCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.ShowCommentPluginInfo));
        SetBinding(runAsAdminCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.RunAsAdmin));
        SetBinding(randomizeWindowTitleCheckBox, nameof(CheckBox.Checked), settings, nameof(Settings.RandomizeWindowTitle));
    }

    private void SetColorBindings() {
        SetBinding(backgroundColorBox, nameof(ColorBox.Color), settings, nameof(Settings.BackgroundColor));

        SetBinding(nodeSelectedColorBox, nameof(ColorBox.Color), settings, nameof(Settings.SelectedColor));
        SetBinding(nodeHiddenColorBox, nameof(ColorBox.Color), settings, nameof(Settings.HiddenColor));
        SetBinding(nodeAddressColorBox, nameof(ColorBox.Color), settings, nameof(Settings.AddressColor));
        SetBinding(nodeOffsetColorBox, nameof(ColorBox.Color), settings, nameof(Settings.OffsetColor));
        SetBinding(nodeHexValueColorBox, nameof(ColorBox.Color), settings, nameof(Settings.HexColor));
        SetBinding(nodeTypeColorBox, nameof(ColorBox.Color), settings, nameof(Settings.TypeColor));
        SetBinding(nodeNameColorBox, nameof(ColorBox.Color), settings, nameof(Settings.NameColor));
        SetBinding(nodeValueColorBox, nameof(ColorBox.Color), settings, nameof(Settings.ValueColor));
        SetBinding(nodeIndexColorBox, nameof(ColorBox.Color), settings, nameof(Settings.IndexColor));
        SetBinding(nodeVTableColorBox, nameof(ColorBox.Color), settings, nameof(Settings.VTableColor));
        SetBinding(nodeCommentColorBox, nameof(ColorBox.Color), settings, nameof(Settings.CommentColor));
        SetBinding(nodeTextColorBox, nameof(ColorBox.Color), settings, nameof(Settings.TextColor));
        SetBinding(nodePluginColorBox, nameof(ColorBox.Color), settings, nameof(Settings.PluginColor));
    }

    private void SetTypeDefinitionBindings() {
        SetBinding(boolTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeBool));
        SetBinding(int8TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeInt8));
        SetBinding(int16TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeInt16));
        SetBinding(int32TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeInt32));
        SetBinding(int64TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeInt64));
        SetBinding(nintTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeNInt));
        SetBinding(uint8TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeUInt8));
        SetBinding(uint16TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeUInt16));
        SetBinding(uint32TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeUInt32));
        SetBinding(uint64TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeUInt64));
        SetBinding(nuintTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeNUInt));
        SetBinding(floatTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeFloat));
        SetBinding(doubleTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeDouble));
        SetBinding(vector2TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeVector2));
        SetBinding(vector3TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeVector3));
        SetBinding(vector4TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeVector4));
        SetBinding(matrix3x3TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeMatrix3x3));
        SetBinding(matrix3x4TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeMatrix3x4));
        SetBinding(matrix4x4TypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeMatrix4x4));
        SetBinding(utf8TextTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeUtf8Text));
        SetBinding(utf16TextTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeUtf16Text));
        SetBinding(utf32TextTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeUtf32Text));
        SetBinding(functionPtrTypeTextBox, nameof(TextBox.Text), typeMapping, nameof(CppTypeMapping.TypeFunctionPtr));
    }
}
