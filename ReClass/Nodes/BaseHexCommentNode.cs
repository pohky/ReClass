using System.Text;
using ReClass.Controls;
using ReClass.Extensions;
using ReClass.UI;

namespace ReClass.Nodes;

public abstract class BaseHexCommentNode : BaseHexNode {
    protected int AddComment(DrawContext view, int x, int y, float fvalue, nint ivalue, nuint uvalue) {
        if (view.Settings.ShowCommentFloat) {
            x = AddText(view, x, y, view.Settings.ValueColor, HotSpot.ReadOnlyId, fvalue > -999999.0f && fvalue < 999999.0f ? fvalue.ToString("0.000") : "#####") + view.Font.Width;
        }
        if (view.Settings.ShowCommentInteger) {
            if (ivalue == nint.Zero) {
                x = AddText(view, x, y, view.Settings.ValueColor, HotSpot.ReadOnlyId, "0") + view.Font.Width;
            } else {
                x = AddText(view, x, y, view.Settings.ValueColor, HotSpot.ReadOnlyId, ivalue.ToString()) + view.Font.Width;
                x = AddText(view, x, y, view.Settings.ValueColor, HotSpot.ReadOnlyId, $"0x{uvalue:X}") + view.Font.Width;
            }
        }

        if (ivalue != nint.Zero) {
            var namedAddress = view.Process.GetNamedAddress(ivalue);
            if (!string.IsNullOrEmpty(namedAddress)) {
                if (view.Settings.ShowCommentPointer) {
                    x = AddText(view, x, y, view.Settings.OffsetColor, HotSpot.NoneId, "->") + view.Font.Width;
                    x = AddText(view, x, y, view.Settings.OffsetColor, HotSpot.ReadOnlyId, namedAddress) + view.Font.Width;
                }

                if (view.Settings.ShowCommentString) {
                    var data = view.Process.ReadRemoteMemory(ivalue, 64);

                    var isWideString = false;
                    string text = string.Empty;

                    // First check if it could be an UTF8 string and if not try UTF16.
                    if (data.Take(nint.Size).InterpretAsSingleByteCharacter().IsPrintableData()) {
                        text = new string(Encoding.UTF8.GetChars(data).TakeWhile(c => c != 0).ToArray());
                    } else if (data.Take(nint.Size * 2).InterpretAsDoubleByteCharacter().IsPrintableData()) {
                        isWideString = true;

                        text = new string(Encoding.Unicode.GetChars(data).TakeWhile(c => c != 0).ToArray());
                    }

                    if (!string.IsNullOrEmpty(text)) {
                        x = AddText(view, x, y, view.Settings.TextColor, HotSpot.NoneId, isWideString ? "L'" : "'");
                        x = AddText(view, x, y, view.Settings.TextColor, HotSpot.ReadOnlyId, text);
                        x = AddText(view, x, y, view.Settings.TextColor, HotSpot.NoneId, "'") + view.Font.Width;
                    }
                }

                if (view.Settings.ShowCommentPluginInfo) {
                    var nodeAddress = view.Address + Offset;

                    foreach (var reader in NodeInfoReader) {
                        var info = reader.ReadNodeInfo(this, view.Process, view.Memory, nodeAddress, ivalue);
                        if (info != null) {
                            x = AddText(view, x, y, view.Settings.PluginColor, HotSpot.ReadOnlyId, info) + view.Font.Width;
                        }
                    }
                }
            }
        }

        return x;
    }
}
