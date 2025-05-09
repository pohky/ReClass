using System.ComponentModel;
using ReClass.Extensions;

namespace ReClass.Controls;

public class EnumComboBox<TEnum> : ComboBox where TEnum : struct {
    public EnumComboBox() {
        base.AutoCompleteMode = AutoCompleteMode.None;
        base.DropDownStyle = ComboBoxStyle.DropDownList;
        base.FormattingEnabled = false;
        base.DisplayMember = nameof(EnumDescriptionDisplay<TEnum>.Description);
        base.ValueMember = nameof(EnumDescriptionDisplay<TEnum>.Value);

        SetValues(EnumDescriptionDisplay<TEnum>.Create());
        if (base.Items.Count != 0) {
            SelectedIndex = 0;
        }
    }

    public void SetAvailableValues(TEnum item1, params TEnum[] items) {
        SetAvailableValues(items.Prepend(item1));
    }

    public void SetAvailableValues(IEnumerable<TEnum> values) {
        SetValues(EnumDescriptionDisplay<TEnum>.CreateExact(values));
    }

    public void SetAvailableValuesExclude(TEnum item1, params TEnum[] items) {
        SetAvailableValuesExclude(items.Prepend(item1));
    }

    public void SetAvailableValuesExclude(IEnumerable<TEnum> values) {
        SetValues(EnumDescriptionDisplay<TEnum>.CreateExclude(values));
    }

    private void SetValues(List<EnumDescriptionDisplay<TEnum>> values) {
        base.Items.Clear();
        base.Items.AddRange(values.ToArray());
    }

    #region Properties

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ObjectCollection Items => new(this);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new AutoCompleteMode AutoCompleteMode { get => AutoCompleteMode.None; set { } }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ComboBoxStyle DropDownStyle { get => ComboBoxStyle.DropDownList; set { } }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new string DisplayMember { get; set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool FormattingEnabled { get; set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new string ValueMember { get; set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new object DataSource { get; set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new TEnum SelectedValue {
        get => ((EnumDescriptionDisplay<TEnum>)base.SelectedItem)?.Value ?? default;
        set => base.SelectedItem = base.Items.Cast<EnumDescriptionDisplay<TEnum>>().PredicateOrFirst(e => e.Value.Equals(value));
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new TEnum SelectedItem {
        get => SelectedValue;
        set => SelectedValue = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new string SelectedText {
        get => ((EnumDescriptionDisplay<TEnum>)base.SelectedItem).Description;
        set => base.SelectedItem = base.Items.Cast<EnumDescriptionDisplay<TEnum>>().PredicateOrFirst(e => e.Description.Equals(value));
    }

    #endregion

}
