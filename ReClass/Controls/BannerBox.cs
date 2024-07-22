using System.ComponentModel;
using ReClass.UI;

namespace ReClass.Controls;

public class BannerBox : Control, ISupportInitialize {
    public const int DefaultBannerHeight = 48;

    private Image icon;

    private Image image;

    private bool inInitialize;
    private string text;
    private string title;

    public Image Icon {
        get => icon;
        set {
            icon = value;
            UpdateBanner();
        }
    }

    public string Title {
        get => title;
        set {
            title = value ?? string.Empty;
            UpdateBanner();
        }
    }

    public override string Text {
        get => text;
        set {
            text = value ?? string.Empty;
            UpdateBanner();
        }
    }

    public BannerBox() {
        title = string.Empty;
        text = string.Empty;
    }

    public void BeginInit() {
        inInitialize = true;
    }

    public void EndInit() {
        inInitialize = false;

        UpdateBanner();
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
        var oldWidth = Width;

        base.SetBoundsCore(x, y, width, DpiUtil.ScaleIntY(DefaultBannerHeight), specified);

        if (oldWidth != width && width > 0) {
            UpdateBanner();
        }
    }

    protected override void OnPaint(PaintEventArgs e) {
        if (image != null) {
            e.Graphics.DrawImage(image, ClientRectangle);
        }
    }

    private void UpdateBanner() {
        if (inInitialize) {
            return;
        }

        try {
            var oldImage = image;

            image = BannerFactory.CreateBanner(Width, Height, icon, title, text, true);

            oldImage?.Dispose();

            Invalidate();
        } catch {
            // ignored
        }
    }
}
