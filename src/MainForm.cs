using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using Medo.Math;
using Medo.Windows.Forms;

namespace DiskPreclear;

internal partial class MainForm : Form {
    public MainForm() {
        InitializeComponent();

        mnu.Font = SystemFonts.MessageBoxFont;
        mnuDisks.Font = SystemFonts.MessageBoxFont;
        mnu.Renderer = Helpers.ToolStripBorderlessSystemRendererInstance;
        Helpers.ScaleToolstrip(mnu);
    }


    private void MainForm_Load(object sender, System.EventArgs e) {
        FillDisks();
        bwUpgradeCheck.RunWorkerAsync();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
        bwUpgradeCheck.CancelAsync();
        if (bwTest.IsBusy) {
            if (!bwTest.CancellationPending) {
                if (MsgBox.ShowQuestion(this, "Are you sure you want to cancel?") == DialogResult.Yes) {
                    bwTest.CancelAsync();
                }
            }
            e.Cancel = true;  // don't close the form
        }
    }


    private bool SuppressMenuKey = false;

    private void ToggleMenu() {
        if (!mnu.ContainsFocus) {
            mnu.Select();
            if (mnuDisks.Enabled) {
                mnuDisks.ComboBox.Select();
            } else if (mnuApp.Enabled) {
                mnuApp.ShowDropDown();
            }
        } else {
            dfgMain.Focus();
        }
    }

    protected override bool ProcessDialogKey(Keys keyData) {
        if (((keyData & Keys.Alt) == Keys.Alt) && (keyData != (Keys.Alt | Keys.Menu))) { SuppressMenuKey = true; }

        switch (keyData) {
            case Keys.F10:
                ToggleMenu();
                return true;

            case Keys.Alt | Keys.D:
                if (mnuPattern.Enabled) { mnuPattern.ShowDropDown(); }
                return true;

            case Keys.Alt | Keys.O:
                if (mnuOrder.Enabled) { mnuOrder.ShowDropDown(); }
                return true;

            case Keys.Alt | Keys.T:
                if (mnuExecute.Enabled) { mnuExecute.ShowDropDown(); }
                return true;

            case Keys.Escape:
                if (bwTest.IsBusy) { Close(); }  // cancel operation only if running
                return true;

            case Keys.PageUp:
                if (mnuDisks.Enabled && mnuDisks.SelectedIndex > 0) { mnuDisks.SelectedIndex -= 1; }
                return true;

            case Keys.PageDown:
                if (mnuDisks.Enabled && mnuDisks.SelectedIndex < mnuDisks.Items.Count - 1) { mnuDisks.SelectedIndex += 1; }
                return true;

            case Keys.F1:
                if (mnuApp.Enabled) { mnuApp.ShowDropDown(); }
                return true;

            case Keys.F5:
                if (mnuExecute.Enabled) { mnuExecute.PerformButtonClick(); }
                return true;

            case Keys.Control | Keys.R:
                if (mnuRefresh.Enabled) { mnuRefresh.PerformClick(); }
                return true;
        }

        return base.ProcessDialogKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.KeyData == Keys.Menu) {
            if (SuppressMenuKey) { SuppressMenuKey = false; return; }
            ToggleMenu();
            e.Handled = true;
            e.SuppressKeyPress = true;
        } else {
            base.OnKeyDown(e);
        }
    }

    protected override void OnKeyUp(KeyEventArgs e) {
        if (e.KeyData == Keys.Menu) {
            if (SuppressMenuKey) { SuppressMenuKey = false; return; }
            ToggleMenu();
            e.Handled = true;
            e.SuppressKeyPress = true;
        } else {
            base.OnKeyUp(e);
        }
    }


    private void mnuDisks_SelectedIndexChanged(object sender, System.EventArgs e) {
        if (mnuDisks.SelectedItem is PhysicalDisk disk) {
            dfgMain.Walker = GetWalker(disk);
            mnuExecute.Enabled = disk is not null;
            staDisk.Text = disk?.Path ?? "-";
        }
    }

    private void mnuExecute_Click(object sender, System.EventArgs e) {
        var allowRead = false;
        var allowWrite = false;
        var operation = "";
        if (mnuExecute.Tag is string tag) {
            if (tag == "mnuExecuteRO") {
                allowRead = true;
                operation = "read";
            } else if (tag == "mnuExecuteWO") {
                allowWrite = true;
                operation = "write";
            } else {
                allowRead = true;
                allowWrite = true;
                operation = "read/write";
            }
        }

        if (mnuDisks.SelectedItem is PhysicalDisk disk) {
            var hasVolumes = disk.GetLogicalVolumes().Count > 0;
            var hasPaths = false;
            var diskData = $"\nPhysical disk {disk.Number}"
                         + $"\n{disk.SizeInGB} GB";
            foreach (var path in disk.GetLogicalVolumePaths()) {
                hasPaths = true;
                diskData += "\n  " + path;
            }

            if (MsgBox.ShowQuestion(this, "Are you sure you want to perform " + operation + " test?\n" + diskData, MessageBoxButtons.YesNo) == DialogResult.No) { return; }
            if (hasVolumes && MsgBox.ShowWarning(this, "Selected disk has volumes present.\nAre you really sure you want to perform " + operation + " test?\nPlease note that test could fail if another process keeps disk open.\n" + diskData, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }
            if (hasPaths && MsgBox.ShowError(this, "Selected disk is in use!\nAre you goddamn sure you want to perform " + operation + " test?\nPlease note that test could fail if another process keeps disk open.\n" + diskData, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }

            var randomKind = RandomKind.Random;
            if ("mnuPatternZero".Equals(mnuPattern.Tag)) {
                randomKind = RandomKind.Zero;
            } else if ("mnuPatternRepeat".Equals(mnuPattern.Tag)) {
                randomKind = RandomKind.Repeat;
            } else if ("mnuPatternSecure".Equals(mnuPattern.Tag)) {
                randomKind = RandomKind.Secure;
            }

            var randomAccess = "mnuOrderRandom".Equals(mnuOrder.Tag);

            var walker = GetWalker(disk, randomAccess, allowRead, allowWrite);
            dfgMain.Walker = walker;

            try {
                walker.Open(allowRead, allowWrite);
                PrepareForTesting(walker);
                bwTest.RunWorkerAsync(new InitObjectState(walker, randomKind));
            } catch (Exception ex) {
                MsgBox.ShowError(this, $"Cannot open disk for {operation}.\n{ex.Message}");
            }
        }
    }

    private void mnuExecute_DropDownOpening(object sender, System.EventArgs e) {
        mnuExecuteUseRW.Checked = false;
        mnuExecuteUseRO.Checked = false;
        mnuExecuteUseWO.Checked = false;
        if (mnuExecute.Tag is string tag) {
            if (tag == "mnuExecuteRO") {
                mnuExecuteUseRO.Checked = true;
            } else if (tag == "mnuExecuteWO") {
                mnuExecuteUseWO.Checked = true;
            } else {
                mnuExecuteUseRW.Checked = true;
            }
        }
    }

    private void mnuExecuteUseRW_Click(object sender, System.EventArgs e) {
        mnuExecute.Tag = "mnuExecuteRW";
        mnuExecute.Text = "Test Read-Write";
        Helpers.ScaleToolstrip(mnu);
        mnuPattern.Enabled = true;
    }

    private void mnuExecuteUseRO_Click(object sender, System.EventArgs e) {
        mnuExecute.Tag = "mnuExecuteRO";
        mnuExecute.Text = "Test Read-Only";
        Helpers.ScaleToolstrip(mnu);
        mnuPattern.Enabled = false;
    }

    private void mnuExecuteUseWO_Click(object sender, System.EventArgs e) {
        mnuExecute.Tag = "mnuExecuteWO";
        mnuExecute.Text = "Test Write-Only";
        Helpers.ScaleToolstrip(mnu);
        mnuPattern.Enabled = true;
    }

    private void mnuPattern_ButtonClick(object sender, EventArgs e) {
        if ("mnuPatternSecure".Equals(mnuPattern.Tag)) {
            mnuPatternRandom_Click(sender, EventArgs.Empty);
        } else if ("mnuPatternRepeat".Equals(mnuPattern.Tag)) {
            mnuPatternZero_Click(sender, EventArgs.Empty);
        } else if ("mnuPatternZero".Equals(mnuPattern.Tag)) {
            mnuPatternSecure_Click(sender, EventArgs.Empty);
        } else {
            mnuPatternRepeat_Click(sender, EventArgs.Empty);
        }
    }

    private void mnuPattern_DropDownOpening(object sender, EventArgs e) {
        mnuPatternSecure.Checked = false;
        mnuPatternRandom.Checked = false;
        mnuPatternRepeat.Checked = false;
        mnuPatternZero.Checked = false;
        if ("mnuPatternSecure".Equals(mnuPattern.Tag)) {
            mnuPatternSecure.Checked = true;
        } else if ("mnuPatternRepeat".Equals(mnuPattern.Tag)) {
            mnuPatternRepeat.Checked = true;
        } else if ("mnuPatternZero".Equals(mnuPattern.Tag)) {
            mnuPatternZero.Checked = true;
        } else {
            mnuPatternRandom.Checked = true;
        }
    }

    private void mnuPatternSecure_Click(object sender, EventArgs e) {
        mnuPattern.Tag = "mnuPatternSecure";
        mnuPattern.Text = "Secure";
    }

    private void mnuPatternRandom_Click(object sender, EventArgs e) {
        mnuPattern.Tag = "mnuPatternRandom";
        mnuPattern.Text = "Random";
    }

    private void mnuPatternRepeat_Click(object sender, EventArgs e) {
        mnuPattern.Tag = "mnuPatternRepeat";
        mnuPattern.Text = "Repeat";
    }

    private void mnuPatternZero_Click(object sender, EventArgs e) {
        mnuPattern.Tag = "mnuPatternZero";
        mnuPattern.Text = "Zero";
    }

    private void mnuOrder_ButtonClick(object sender, EventArgs e) {
        if ("mnuOrderSequential".Equals(mnuOrder.Tag)) {
            mnuOrderRandom_Click(sender, EventArgs.Empty);
        } else {
            mnuOrderSequential_Click(sender, EventArgs.Empty);
        }
    }

    private void mnuOrder_DropDownOpening(object sender, EventArgs e) {
        mnuOrderRandom.Checked = false;
        mnuOrderSequential.Checked = false;
        if ("mnuOrderRandom".Equals(mnuOrder.Tag)) {
            mnuOrderRandom.Checked = true;
        } else {
            mnuOrderSequential.Checked = true;
        }
    }

    private void mnuOrderSequential_Click(object sender, EventArgs e) {
        mnuOrder.Tag = "mnuOrderSequential";
        mnuOrder.Text = "Sequential";
    }

    private void mnuOrderRandom_Click(object sender, EventArgs e) {
        mnuOrder.Tag = "mnuOrderRandom";
        mnuOrder.Text = "Random";
    }

    private void mnuRefresh_Click(object sender, System.EventArgs e) {
        FillDisks();
    }

    private void mnuAppFeedback_Click(object sender, System.EventArgs e) {
        ErrorReportBox.ShowDialog(this, new Uri("https://medo64.com/feedback/"));
    }

    private void mnuAppUpgrade_Click(object sender, EventArgs e) {
        UpgradeBox.ShowDialog(this, new Uri("https://medo64.com/upgrade/"));
    }

    private void mnuAppAbout_Click(object sender, System.EventArgs e) {
        AboutBox.ShowDialog(this);
    }


    private void dfgMain_ElementCountUpdated(object sender, EventArgs e) {
        var sizeInMB = dfgMain.BlockElementSizeInMegabytes;
        if (sizeInMB > 0) {
            staElementMB.Text = $"{1} block = {dfgMain.BlockElementSizeInMegabytes:#,##0} MB";
            staElementMB.ToolTipText = $"{dfgMain.BlockElementCount} blocks total";
        } else {
            staElementMB.Text = "";
            staElementMB.ToolTipText = "";
        }
    }


    private void bwUpgradeCheck_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
        e.Cancel = true;

        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 3000) { //wait for three seconds
            Thread.Sleep(100);
            if (bwUpgradeCheck.CancellationPending) { return; }
        }

        var file = UpgradeBox.GetUpgradeFile(new Uri("https://medo64.com/upgrade/"));
        if (file != null) {
            if (bwUpgradeCheck.CancellationPending) { return; }
            e.Cancel = false;
        }
    }

    private void bwUpgradeCheck_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
        if (!e.Cancelled && (e.Error == null)) {
            Helpers.ScaleToolstripItem(mnuApp, "mnuAppUpgrade");
            mnuAppUpgrade.Text = "Upgrade is available";
        }
    }


    private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();

    private void bwTest_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
        if (e.Argument is not InitObjectState state) { return; }

        var walker = state.Walker;
        var randomKind = state.RandomKind;

        var swTotal = Stopwatch.StartNew();
        var nextUpdate = swTotal.ElapsedMilliseconds;

        var okCount = 0;
        var nokAccessCount = 0;
        var nokValidationCount = 0;

        var dataOut = new byte[walker.MaxBlockSize];
        var dataIn = new byte[walker.MaxBlockSize];

        var writeSpeed = new MovingAverage(42);
        var readSpeed = new MovingAverage(42);

        var blockCount = walker.BlockCount;
        for (var i = 0; i < blockCount; i++) {
            // data setup
            if (walker.AllowWrite) {
                var swRandom = Stopwatch.StartNew();
                if (randomKind is RandomKind.Secure or RandomKind.Random) {
                    Rnd.GetBytes(dataOut);
                } else if ((randomKind == RandomKind.Repeat) && (i == 0)) {
                    for (var j = 0; j < dataOut.Length; j += 2) {  // dataOut length has to be even
                        dataOut[j] = 0x55;
                        dataOut[j + 1] = 0xAA;
                    }
                }
                swRandom.Stop();
            }

            var okAccess = true;
            var okValidate = true;

            // first pass write
            if (walker.AllowWrite) {
                var swWrite = Stopwatch.StartNew();
                var status = walker.Write(dataOut);
                okAccess &= (status == IOStatus.Ok);
                if (status != IOStatus.InternalError) {
                    var writeTime = (double)swWrite.ElapsedMilliseconds / 1000;
                    if (writeTime == 0) { writeTime = 0.000001; }
                    writeSpeed.Add(walker.OffsetLength / writeTime);
                }
            }

            // first pass read
            if (walker.AllowRead) {
                var swRead = Stopwatch.StartNew();
                var status = walker.Read(dataIn);
                okAccess &= (status == IOStatus.Ok);
                if (status != IOStatus.InternalError) {
                    var readTime = (double)swRead.ElapsedMilliseconds / 1000;
                    if (readTime == 0) { readTime = 0.000001; }
                    readSpeed.Add(walker.OffsetLength / readTime);
                }
            }

            // first pass validate
            if (walker.AllowRead && walker.AllowWrite) {
                okValidate &= DiskWalker.Validate(dataOut, dataIn);
            }

            // second pass
            if (randomKind is RandomKind.Secure) {
                // reverse data "polarity"
                for (var j = 0; j < dataOut.Length; j += 1) {
                    dataOut[j] = (byte)~dataOut[j];
                }

                // second pass write
                if (walker.AllowWrite) {
                    var swWrite = Stopwatch.StartNew();
                    var status = walker.Write(dataOut);
                    okAccess &= (status == IOStatus.Ok);
                    if (status != IOStatus.InternalError) {
                        var writeTime = (double)swWrite.ElapsedMilliseconds / 1000;
                        if (writeTime == 0) { writeTime = 0.000001; }
                        writeSpeed.Add(walker.OffsetLength / writeTime);
                    }
                }

                // second pass read
                if (walker.AllowRead) {
                    var swRead = Stopwatch.StartNew();
                    var status = walker.Read(dataIn);
                    okAccess &= (status == IOStatus.Ok);
                    if (status != IOStatus.InternalError) {
                        var readTime = (double)swRead.ElapsedMilliseconds / 1000;
                        if (readTime == 0) { readTime = 0.000001; }
                        readSpeed.Add(walker.OffsetLength / readTime);
                    }
                }

                // second pass validate
                if (walker.AllowRead && walker.AllowWrite) {
                    okValidate &= DiskWalker.Validate(dataOut, dataIn);
                }
            }

            // accounting
            if (okValidate) {
                okCount += 1;
                dfgMain.SetBlockState(walker.BlockIndex, BlockState.Validated);
            } else if (!okAccess) {
                nokAccessCount += 1;
                dfgMain.SetBlockState(walker.BlockIndex, BlockState.AccessError);
            } else {
                nokValidationCount += 1;
                dfgMain.SetBlockState(walker.BlockIndex, BlockState.ValidationError);
            }

            // check for cancellation
            if (bwTest.CancellationPending) {
                walker.Close();
                e.Cancel = true;
                return;  // done
            }

            // progress update
            if (nextUpdate < swTotal.ElapsedMilliseconds) {
                var progress = new ProgressObjectState(swTotal, walker.Index + 1, walker.BlockCount, walker.TotalSize, okCount, nokAccessCount, nokValidationCount, walker.MaxBlockSize, writeSpeed.Average, readSpeed.Average);
                bwTest.ReportProgress(0, progress);
                nextUpdate = swTotal.ElapsedMilliseconds + 420;  // next update in 420ms
            }

            // next block
            walker.Index += 1;
        }

        walker.Close();
        var finalProgress = new ProgressObjectState(swTotal, walker.Index, walker.BlockCount, walker.TotalSize, okCount, nokAccessCount, nokValidationCount, walker.MaxBlockSize, writeSpeed.Average, readSpeed.Average);
        bwTest.ReportProgress(100, finalProgress);
        e.Result = finalProgress;
    }

    private void bwTest_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
        if (e.UserState is ProgressObjectState state) {
            staWriteSpeed.Text = $"W: {state.WriteSpeed:#,##0} MB/s";
            staReadSpeed.Text = $"R: {state.ReadSpeed:#,##0} MB/s";
            if (state.NokValidationCount > 0) {
                staValidationErrors.Text = $"{state.NokValidationCount:#,##0} " + ((state.NokValidationCount != 1) ? "errors" : "error");
            }
            if (state.NokAccessCount > 0) {
                staAccessErrors.Text = $"{state.NokAccessCount:#,##0} " + ((state.NokAccessCount != 1) ? "access errors" : "access error");
            }
            staPercents.Text = state.Percents.ToString("0.000", CultureInfo.CurrentCulture) + "%";
            staProgress.Value = state.Permilles;
            staRemaining.Text = state.EstimatedRemainingAsString + " remaining";
            staProcessed.Text = $"{state.ProcessedMB:#,##0} MB processed";
        }
    }

    private void bwTest_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
        dfgMain.Invalidate();
        if (e.Cancelled) {
            MsgBox.ShowWarning(this, "Operation cancelled.");
        } else if (e.Error != null) {
            MsgBox.ShowError(this, e.Error.Message);
        } else {
            if (e.Result is ProgressObjectState state) {
                var isOk = true;
                var text = $"Verification completed in {state.TimeUsedAsString}.";
                if (state.NokValidationCount > 0) {
                    text += $"\n{state.NokValidationCount} " + ((state.NokValidationCount != 1) ? "errors" : "error") + " found.";
                    if (state.NokAccessCount > 0) {
                        text += $"\nAdditionally, {state.NokAccessCount} " + ((state.NokAccessCount != 1) ? "access errors" : "access error") + " occurred.";
                    }
                    isOk = false;
                } else if (state.NokAccessCount > 0) {
                    text += $"\n{state.NokAccessCount} " + ((state.NokAccessCount != 1) ? "access errors" : "access error") + " but no validation errors.";
                    text += $"\n\nTry restarting computer and testing disk again as this could be either OS issue or controller error.";
                    isOk = false;
                } else {  // no issues
                    text += "\nNo errors found.";
                }
                if (isOk) {
                    MsgBox.ShowInformation(this, text);
                } else {
                    MsgBox.ShowError(this, text);
                }
            }
        }
        PrepareForTesting(null);
    }


    private void FillDisks() {
        mnuExecute.Enabled = false;

        mnuDisks.BeginUpdate();

        var prevSelection = mnuDisks.SelectedItem as PhysicalDisk;

        mnuDisks.Items.Clear();
        foreach (var disk in PhysicalDisk.GetAllDisks()) {
            mnuDisks.Items.Add(disk);
            if ((prevSelection is not null) && (disk.Number == prevSelection.Number)) {
                mnuDisks.SelectedItem = disk;
            }
        }
        if ((mnuDisks.SelectedItem is null) && (mnuDisks.Items.Count > 0)) {  // select the last disk
            mnuDisks.SelectedIndex = mnuDisks.Items.Count - 1;
        }

        mnuDisks.EndUpdate();
    }

    private void PrepareForTesting(DiskWalker? walker) {
        var testing = (walker != null);
        var testingRead = (walker != null) && walker.AllowRead;
        var testingWrite = (walker != null) && walker.AllowWrite;

        mnuDisks.Enabled = !testing;
        mnuExecute.Enabled = !testing;
        mnuOrder.Enabled = !testing;
        mnuPattern.Enabled = !testing;
        mnuRefresh.Enabled = !testing;
        mnuAppUpgrade.Enabled = !testing;

        staWriteSpeed.Visible = testingWrite;
        staWriteSpeed.Text = "";
        staReadSpeed.Visible = testingRead;
        staReadSpeed.Text = "";
        staAccessErrors.Visible = testing;
        staAccessErrors.Text = "";
        staValidationErrors.Visible = testing;
        staValidationErrors.Text = "";
        staPercents.Visible = testing;
        staPercents.Text = "";
        staPercents.Height = staDisk.Height;
        staProgress.Visible = testing;
        staProgress.Value = 0;
        staRemaining.Visible = testing;
        staRemaining.Text = "";
        staProcessed.Visible = testing;
        staProcessed.Text = "";
    }

    private static DiskWalker GetWalker(PhysicalDisk disk, bool randomAccess = false, bool allowRead = false, bool allowWrite = false) {
        var blockSizeMB = disk.SizeInGB switch {
            > 10000 => 16,
            > 1000 => 8,
            > 100 => 4,
            > 10 => 2,
            _ => 1,
        };
        return new DiskWalker(disk, blockSizeMB, randomAccess) {
            AllowRead = allowRead,
            AllowWrite = allowWrite
        };
    }

}
