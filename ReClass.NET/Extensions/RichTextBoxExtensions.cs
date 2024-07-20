using System.Runtime.InteropServices;

namespace ReClassNET.Extensions;

public static class RichTextBoxExtension {

    private const int EmGetrect = 0xB2;
    private const int EmSetrect = 0xB3;
    public static void SetInnerMargin(this TextBoxBase textBox, int left, int top, int right, int bottom) {
        var rect = textBox.GetFormattingRect();

        var newRect = new Rectangle(left, top, rect.Width - left - right, rect.Height - top - bottom);
        textBox.SetFormattingRect(newRect);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, ref RECT rect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, ref Rectangle lParam);

    private static void SetFormattingRect(this TextBoxBase textbox, Rectangle rect) {
        var rc = new RECT(rect);
        SendMessage(textbox.Handle, EmSetrect, 0, ref rc);
    }

    private static Rectangle GetFormattingRect(this TextBoxBase textbox) {
        var rect = new Rectangle();
        SendMessage(textbox.Handle, EmGetrect, 0, ref rect);
        return rect;
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct RECT {
        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;

        private RECT(int left, int top, int right, int bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(Rectangle r)
            : this(r.Left, r.Top, r.Right, r.Bottom) {
        }
    }
}
