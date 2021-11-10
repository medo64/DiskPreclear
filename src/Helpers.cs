using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DiskPreclear;

internal static class Helpers {

    #region ToolStripBorderlessProfessionalRenderer

    private readonly static Lazy<ToolStripBorderlessProfessionalRenderer> _toolStripBorderlessSystemRendererInstance = new(
        () => new ToolStripBorderlessProfessionalRenderer());
    internal static ToolStripBorderlessProfessionalRenderer ToolStripBorderlessSystemRendererInstance => _toolStripBorderlessSystemRendererInstance.Value;

    internal class ToolStripBorderlessProfessionalRenderer : ToolStripProfessionalRenderer {

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
        }

    }

    #endregion

    #region Toolstrip images

    internal static void ScaleToolstrip(params ToolStrip[] toolstrips) {
        var sizeAndSet = GetSizeAndSet();
        var size = sizeAndSet.Key;
        var set = sizeAndSet.Value;

        var resources = DiskPreclear.Properties.Resources.ResourceManager;
        foreach (var toolstrip in toolstrips) {
            toolstrip.ImageScalingSize = new Size(size, size);
            foreach (ToolStripItem item in toolstrip.Items) {
                item.ImageScaling = ToolStripItemImageScaling.None;
                if (item.Image != null) { //update only those already having image
                    Bitmap? bitmap = null;
                    if (!string.IsNullOrEmpty(item.Name)) {
                        bitmap = resources.GetObject(item.Name + set) as Bitmap;
                    }
                    if ((bitmap == null) && !string.IsNullOrEmpty(item.Tag as string)) {
                        bitmap = resources.GetObject(item.Tag + set) as Bitmap;
                    }

                    item.ImageScaling = ToolStripItemImageScaling.None;
#if DEBUG
                    item.Image = (bitmap != null) ? new Bitmap(bitmap, size, size) : new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#else
                        if (bitmap != null) { item.Image = new Bitmap(bitmap, size, size); }
#endif
                }

                if (item is ToolStripSplitButton toolstripSplitButton) { ScaleToolstrip(toolstripSplitButton.DropDown); }
            }
        }
    }

    internal static void ScaleToolstripItem(ToolStripItem item, string name) {
        var sizeAndSet = GetSizeAndSet();
        var size = sizeAndSet.Key;
        var set = sizeAndSet.Value;

        var resources = DiskPreclear.Properties.Resources.ResourceManager;
        item.ImageScaling = ToolStripItemImageScaling.None;
        if (resources.GetObject(name + set) is Bitmap bitmap) {
            item.Image = new Bitmap(bitmap, size, size);
        } else {
#if DEBUG
            item.Image = new Bitmap(size, size, PixelFormat.Format8bppIndexed);
#endif
        }
    }

    private static KeyValuePair<int, string> GetSizeAndSet() {
        return GetSizeAndSet(0.5);
    }

    private static KeyValuePair<int, string> GetSizeAndSet(double scaleBoost) {
        double scale;
        using (var control = new Control())
        using (var g = control.CreateGraphics()) {
            scale = Math.Max(Math.Max(g.DpiX, g.DpiY), 96.0) / 96.0;
        }

        scale += scaleBoost;

        if (scale < 1.5) {
            return new KeyValuePair<int, string>(16, "_16");
        } else if (scale < 2) {
            return new KeyValuePair<int, string>(24, "_24");
        } else if (scale < 3) {
            return new KeyValuePair<int, string>(32, "_32");
        } else {
            var base32 = 16 * scale / 32;
            var base48 = 16 * scale / 48;
            if ((base48 - (int)base48) < (base32 - (int)base32)) {
                return new KeyValuePair<int, string>(48 * (int)base48, "_48");
            } else {
                return new KeyValuePair<int, string>(32 * (int)base32, "_32");
            }
        }
    }

    #endregion

}
