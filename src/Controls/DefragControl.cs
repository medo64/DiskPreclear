using System;
using System.Drawing;
using System.Windows.Forms;
using Medo.Diagnostics;

namespace DiskPreclear.Controls;

internal partial class DefragControl : Control {
    public DefragControl() {
        InitializeComponent();
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        RefreshTimer.Tick += delegate (object? _, EventArgs _) {
            bool updateNeeded;
            lock (SyncBlockStates) {
                updateNeeded = BlockStatesUpdateNeeded;
                BlockStatesUpdateNeeded = false;
            }
            if (updateNeeded) { Invalidate(); }
        };
        RefreshTimer.Start();

        for (var i = 0; i < OkBrushes.Length; i++) {
            OkBrushes[i] = new SolidBrush(Color.FromArgb(0, 128 + i, 0));
        }
        OkBrushes[101] = new SolidBrush(Color.FromArgb(0, 255, 0));
    }

    private readonly Brush[] OkBrushes = new Brush[102];  // 0th brush is shown as soon as 1 is hit; 1-100: shown at percent; 101: last hit

    private int ElementWidth;
    private int ElementHeight;
    private int ElementCount;
    private int ElementFoldCount;
    private int HorizontalElements;
    private int VerticalElements;
    private int OriginLeft;
    private int OriginTop;
    private bool?[] BlockStates = Array.Empty<bool?>();  // null: not visited; true: ok, false: nok
    private int[] FoldedBlockStates = Array.Empty<int>();  // handy cache for BlockStates -> negative number is NOK; 0: not visited; pos: how many visited
    private bool BlockStatesUpdateNeeded = false;
    private int LastFoldedIndex = -1;
    private readonly object SyncBlockStates = new();
    private readonly Timer RefreshTimer = new() { Interval = 230 };

    protected override void OnResize(EventArgs e) {
        base.OnResize(e);
        ElementWidth = SystemInformation.VerticalScrollBarWidth;
        ElementHeight = SystemInformation.VerticalScrollBarWidth;

        HorizontalElements = (ClientRectangle.Width - SystemInformation.Border3DSize.Width * 4) / ElementWidth;
        VerticalElements = (ClientRectangle.Height - SystemInformation.Border3DSize.Height * 4) / ElementHeight;
        var maxElements = HorizontalElements * VerticalElements;

        //OriginLeft = (ClientRectangle.Width - HorizontalElements * ElementWidth) / 2;
        //OriginTop = Math.Max(OriginLeft - SystemInformation.Border3DSize.Height, 0);
        OriginLeft = SystemInformation.Border3DSize.Width * 2;
        OriginTop = SystemInformation.Border3DSize.Height;

        var walker = Walker;
        if (walker != null) {
            var blockCount = walker.BlockCount;
            int elementCount;
            int elementFoldCount = 1;
            if (maxElements > 0) {
                elementCount = blockCount;
                while (elementCount > maxElements) {
                    elementFoldCount += 1;
                    if (blockCount % elementFoldCount == 0) {
                        elementCount = blockCount / elementFoldCount;
                    } else {
                        elementCount = blockCount / elementFoldCount + 1;  // to cover the last part
                    }
                }
            } else {
                elementCount = 0;
            }

            lock (SyncBlockStates) {
                ElementCount = elementCount;
                ElementFoldCount = elementFoldCount;

                if (ElementCount != FoldedBlockStates.Length) {  // recalculate folded states
                    FoldedBlockStates = new int[ElementCount];
                    if (ElementCount > 0) {
                        for (var i = 0; i < BlockStates.Length; i++) {
                            var blockState = BlockStates[i];
                            if (blockState == null) { continue; }
                            var foldedIndex = i / ElementFoldCount;
                            if (blockState == true) {
                                if (FoldedBlockStates[foldedIndex] >= 0) { FoldedBlockStates[foldedIndex] += 1; }
                            } else if (blockState == false) {
                                FoldedBlockStates[foldedIndex] = int.MinValue;
                            }
                        }
                    }
                }
            }

        } else {
            ElementCount = 0;
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs pe) {
        base.OnPaint(pe);

        var g = pe.Graphics;
        using var lw = new LifetimeWatch("Paint");

        var index = 0;
        for (var y = 0; y < VerticalElements; y++) {
            for (var x = 0; x < HorizontalElements; x++) {
                if (index >= ElementCount) { break; }
                DrawBox(g, x, y, GetFoldedBlockBrush(index));
                index += 1;
            }
            if (index >= ElementCount) { break; }
        }
    }


    private void DrawBox(Graphics g, int x, int y, Brush brush) {
        var left = OriginLeft + x * ElementWidth;
        var right = left + ElementWidth - 1;
        var top = OriginTop + y * ElementHeight;
        var bottom = top + ElementHeight - 1;
        g.FillRectangle(brush, left + 1, top + 1, ElementWidth - 2, ElementHeight - 2);
        g.DrawLine(SystemPens.ButtonHighlight, left, top, left, bottom);
        g.DrawLine(SystemPens.ButtonHighlight, left, top, right, top);
        g.DrawLine(SystemPens.ButtonShadow, right, top + 1, right, bottom);
        g.DrawLine(SystemPens.ButtonShadow, left + 1, bottom, right, bottom);
    }

    private Brush GetFoldedBlockBrush(int elementIndex) {
        lock (SyncBlockStates) {
            if (elementIndex >= FoldedBlockStates.Length) { return SystemBrushes.Control; }  // something went wrong
            if (elementIndex == LastFoldedIndex) { return OkBrushes[101]; }

            var count = FoldedBlockStates[elementIndex];
            if (count == 0) {
                return SystemBrushes.Control;
            } else if (count < 0) {
                return Brushes.Red;
            } else if (count == 1) { // to make sure it doesn't get rounded down to 0
                return OkBrushes[0];
            } else {
                return OkBrushes[count * 100 / ElementFoldCount];
            }
        }
    }


    private DiskWalker? _walker;
    public DiskWalker? Walker {
        get { return _walker; }
        set {
            _walker = value;
            BlockStates = _walker != null ? new bool?[_walker.BlockCount] : Array.Empty<bool?>();
            OnResize(EventArgs.Empty);
        }
    }

    public void SetBlockState(int index, bool ok) {
        lock (SyncBlockStates) {
            BlockStates[index] = ok;
            if (FoldedBlockStates.Length > 0) {
                var foldedIndex = index / ElementFoldCount;
                if (ok) {
                    if (FoldedBlockStates[foldedIndex] >= 0) { FoldedBlockStates[foldedIndex] += 1; }
                } else {
                    FoldedBlockStates[foldedIndex] = int.MinValue;
                }
                LastFoldedIndex = foldedIndex;
                BlockStatesUpdateNeeded = true;
            }
        }
    }

}
