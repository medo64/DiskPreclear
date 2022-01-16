using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Medo.Diagnostics;

namespace DiskPreclear.Controls;

internal partial class DefragControl : Control {
    public DefragControl() {
        InitializeComponent();
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        RefreshTimer.Tick += delegate (object? _, EventArgs _) {
            bool needsUpdate;
            lock (SyncBlockStates) {
                needsUpdate = NeedsUpdate;
                NeedsUpdate = false;
            }
            if (needsUpdate) { Invalidate(); }
        };
        RefreshTimer.Start();

        var colorFrom = Color.FromArgb(160, 160, 0);
        var colorTo = SystemColors.Control;
        for (var i = 0; i <= 100; i++) {
            var r = (colorFrom.R * (110 - i) + colorTo.R * i) / 110;
            var g = (colorFrom.G * (110 - i) + colorTo.G * i) / 110;
            var b = (colorFrom.B * (110 - i) + colorTo.B * i) / 110;
            var a = (colorFrom.A * (110 - i) + colorTo.A * i) / 110;
            TrailBrushes[i] = new SolidBrush(Color.FromArgb(a, r, g, b));
        }
    }

    private readonly Brush[] TrailBrushes = new Brush[101];  // 0-100%
    private readonly Brush TrailBrushLastUpdate = new SolidBrush(Color.FromArgb(0, SystemColors.Control.G, SystemColors.Control.B));

    private int ElementWidth;
    private int ElementHeight;
    private int ElementCount;
    private int ElementFoldCount;
    private int HorizontalElements;
    private int VerticalElements;
    private int OriginLeft;
    private int OriginTop;
    private BlockState[] BlockStates = Array.Empty<BlockState>();
    private readonly LinkedList<int> BlockTrail = new();
    private bool NeedsUpdate = false;
    private readonly object SyncBlockStates = new();
    private readonly System.Windows.Forms.Timer RefreshTimer = new() { Interval = 230 };

    protected override void OnResize(EventArgs e) {
        base.OnResize(e);
        ElementWidth = SystemInformation.VerticalScrollBarWidth;
        ElementHeight = SystemInformation.VerticalScrollBarWidth;

        HorizontalElements = (ClientRectangle.Width - SystemInformation.Border3DSize.Width * 4) / ElementWidth;
        VerticalElements = (ClientRectangle.Height - SystemInformation.Border3DSize.Height * 4) / ElementHeight;
        var maxElements = HorizontalElements * VerticalElements;

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
            }

            ElementCountUpdated?.Invoke(this, EventArgs.Empty);
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
                DrawBox(g, x, y, GetElementBrush(index));
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

    private Brush GetElementBrush(int elementIndex) {
        var startIndex = elementIndex * ElementFoldCount;

        lock (SyncBlockStates) {
            var allDone = true;
            for (var i = 0; i < ElementFoldCount; i++) {
                var index = startIndex + i;
                if (index >= BlockStates.Length) { break; }

                if (BlockStates[index] == BlockState.ValidationError) {  // error takes precedence
                    return Brushes.Red;
                } else if (BlockStates[index] == BlockState.AccessError) {  // error takes precedence
                    return SystemBrushes.ControlDarkDark;
                } else if (BlockStates[index] == BlockState.None) {  // if any block is not filled; you can proceed with percent highlight logic
                    allDone = false;
                    break;
                }
            }

            if (allDone) {
                return Brushes.Green;
            } else {  // select color based on trail
                var i = 0;
                foreach (var index in BlockTrail) {
                    var foldedIndex = index / ElementFoldCount;  // fold trail
                    if (foldedIndex == elementIndex) {
                        if (i == 0) { return TrailBrushLastUpdate; }  // last updated element
                        var percent = i * 100 / BlockTrail.Count;
                        if (percent >= TrailBrushes.Length) { percent = TrailBrushes.Length - 1; }
                        return TrailBrushes[percent];
                    }
                    i++;
                }
            }

            return SystemBrushes.Control;
        }
    }


    private DiskWalker? _walker;
    public DiskWalker? Walker {
        get { return _walker; }
        set {
            lock (SyncBlockStates) {
                _walker = value;
                BlockStates = _walker != null ? new BlockState[_walker.BlockCount] : Array.Empty<BlockState>();
                BlockTrail.Clear();
            }
            OnResize(EventArgs.Empty);  // force resize to recalculate
        }
    }

    public void SetBlockState(int index, BlockState newState) {
        lock (SyncBlockStates) {
            BlockStates[index] = newState;

            BlockTrail.AddFirst(index);
            while (BlockTrail.Count > 4200) { BlockTrail.RemoveLast(); }

            NeedsUpdate = true;
        }
    }


    public event EventHandler<EventArgs>? ElementCountUpdated;

    public long BlockElementCount {
        get {
            lock (SyncBlockStates) {
                return ElementCount;
            }
        }
    }

    public long BlockElementSizeInBytes {
        get {
            lock (SyncBlockStates) {
                if (_walker is null) { return 0; }
                return ElementFoldCount * (long)_walker.MaxBlockSize;
            }
        }
    }

    public long BlockElementSizeInMegabytes {
        get {
            return BlockElementSizeInBytes / 1024 / 1024;
        }
    }

}

