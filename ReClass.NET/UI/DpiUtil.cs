using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ReClassNET.UI;

public static class DpiUtil {
    public const int DefalutDpi = 96;

    private static int _dpiX = DefalutDpi;
    private static int _dpiY = DefalutDpi;

    private static double _scaleX = 1.0;
    private static double _scaleY = 1.0;
    
    public static void SetDpi(int x, int y) {
        _dpiX = x;
        _dpiY = y;

        if (_dpiX <= 0 || _dpiY <= 0) {
            _dpiX = DefalutDpi;
            _dpiY = DefalutDpi;
        }

        _scaleX = _dpiX / (double)DefalutDpi;
        _scaleY = _dpiY / (double)DefalutDpi;
    }

    public static void TrySetDpiFromCurrentDesktop() {
        try {
            using var g = Graphics.FromHwnd(IntPtr.Zero);

            SetDpi((int)g.DpiX, (int)g.DpiY);
        } catch {
            // ignored
        }
    }

    public static int ScaleIntX(int i) => (int)Math.Round(i * _scaleX);

    public static int ScaleIntY(int i) => (int)Math.Round(i * _scaleY);

    public static Image ScaleImage(Image sourceImage) {
        var width = sourceImage.Width;
        var height = sourceImage.Height;
        var scaledWidth = ScaleIntX(width);
        var scaledHeight = ScaleIntY(height);

        if (width == scaledWidth && height == scaledHeight) {
            return sourceImage;
        }

        return ScaleImage(sourceImage, scaledWidth, scaledHeight);
    }

    private static Image ScaleImage(Image sourceImage, int width, int height) {
        var scaledImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);

        using var g = Graphics.FromImage(scaledImage);
        g.Clear(Color.Transparent);

        g.SmoothingMode = SmoothingMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;

        var sourceWidth = sourceImage.Width;
        var sourceHeight = sourceImage.Height;

        var interpolationMode = InterpolationMode.HighQualityBicubic;
        if (sourceWidth > 0 && sourceHeight > 0) {
            if (width % sourceWidth == 0 && height % sourceHeight == 0) {
                interpolationMode = InterpolationMode.NearestNeighbor;
            }
        }

        g.InterpolationMode = interpolationMode;

        var srcRect = new RectangleF(0.0f, 0.0f, sourceWidth, sourceHeight);
        var destRect = new RectangleF(0.0f, 0.0f, width, height);
        AdjustScaleRects(ref srcRect, ref destRect);

        g.DrawImage(sourceImage, destRect, srcRect, GraphicsUnit.Pixel);

        return scaledImage;
    }

    private static void AdjustScaleRects(ref RectangleF srcRect, ref RectangleF destRect) {
        if (destRect.Width > srcRect.Width)
            srcRect.X -= 0.5f;
        if (destRect.Height > srcRect.Height)
            srcRect.Y -= 0.5f;

        if (destRect.Width < srcRect.Width)
            srcRect.X += 0.5f;
        if (destRect.Height < srcRect.Height)
            srcRect.Y += 0.5f;
    }
}
