using System.ComponentModel;

namespace ReClass.Controls;

[DefaultEvent(nameof(ColorChanged))]
[DefaultBindingProperty(nameof(Color))]
public partial class ColorBox : UserControl {
    private const int DefaultWidth = 123;
    private const int DefaultHeight = 20;

    private Color color;

    private bool updateTextBox = true;
    public Color Color {
        get => color;
        set {
            // Normalize the color because Color.Red != Color.FromArgb(255, 0, 0)
            value = Color.FromArgb(value.ToArgb());
            if (color != value) {
                color = value;

                colorPanel.BackColor = value;
                if (updateTextBox) {
                    valueTextBox.Text = ColorTranslator.ToHtml(value);
                }

                OnColorChanged(EventArgs.Empty);
            }

            updateTextBox = true;
        }
    }

    public ColorBox() {
        InitializeComponent();
    }

    public event EventHandler? ColorChanged;

    protected virtual void OnColorChanged(EventArgs e) {
        var eh = ColorChanged;
        eh?.Invoke(this, e);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
        base.SetBoundsCore(x, y, DefaultWidth, DefaultHeight, specified);
    }

    private void OnTextChanged(object sender, EventArgs e) {
        try {
            var str = valueTextBox.Text;
            if (!str.StartsWith("#")) {
                str = "#" + str;
            }

            var newColor = ColorTranslator.FromHtml(str);

            updateTextBox = false;
            Color = newColor;
        } catch {
            // ignored
        }
    }

    private void OnPanelClick(object sender, EventArgs e) {
        using var cd = new ColorDialog {
            FullOpen = true,
            Color = Color
        };

        if (cd.ShowDialog() == DialogResult.OK) {
            Color = cd.Color;
        }
    }

    private void OnPanelPaint(object sender, PaintEventArgs e) {
        var rect = colorPanel.ClientRectangle;
        rect.Width--;
        rect.Height--;
        e.Graphics.DrawRectangle(Pens.Black, rect);
    }
}
