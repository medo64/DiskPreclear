using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Forms;
using Medo.Math;
using Medo.Windows.Forms;

namespace DiskPreclear;

internal partial class MainForm : Form {
    public MainForm() {
        InitializeComponent();

        mnu.Renderer = Helpers.ToolStripBorderlessSystemRendererInstance;
        Helpers.ScaleToolstrip(mnu);
    }

    private void MainForm_Load(object sender, System.EventArgs e) {
        FillDisks();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
        if (bwTest.IsBusy) {
            bwTest.CancelAsync();
            e.Cancel = true;  // don't close the form if you had to cancel task
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

            if (Medo.Windows.Forms.MessageBox.ShowQuestion(this, "Are you sure you want to perform " + operation + " test?\n" + diskData, MessageBoxButtons.YesNo) == DialogResult.No) { return; }
            if (hasVolumes && Medo.Windows.Forms.MessageBox.ShowWarning(this, "Selected disk has volumes present.\nAre you really sure you want to perform " + operation + " test?\nPlease note that test could fail if another process keeps disk open.\n" + diskData, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }
            if (hasPaths && Medo.Windows.Forms.MessageBox.ShowError(this, "Selected disk is in use!\nAre you goddamn sure you want to perform " + operation + " test?\nPlease note that test could fail if another process keeps disk open.\n" + diskData, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }

            var walker = GetWalker(disk, allowRead, allowWrite);
            dfgMain.Walker = walker;
            if (walker.Open(allowRead, allowWrite)) {
                PrepareForTesting(walker);
                bwTest.RunWorkerAsync(walker);
            } else {
                Medo.Windows.Forms.MessageBox.ShowError(this, "Cannot open disk for " + operation + ".");
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
        mnuExecute.Text = "Execute Read-Write";
        Helpers.ScaleToolstrip(mnu);
    }

    private void mnuExecuteUseRO_Click(object sender, System.EventArgs e) {
        mnuExecute.Tag = "mnuExecuteRO";
        mnuExecute.Text = "Execute Read-Only";
        Helpers.ScaleToolstrip(mnu);
    }

    private void mnuExecuteUseWO_Click(object sender, System.EventArgs e) {
        mnuExecute.Tag = "mnuExecuteWO";
        mnuExecute.Text = "Execute Write-Only";
        Helpers.ScaleToolstrip(mnu);
    }

    private void mnuRefresh_Click(object sender, System.EventArgs e) {
        FillDisks();
    }

    private void mnuAppAbout_Click(object sender, System.EventArgs e) {
        AboutBox.ShowDialog(this);
    }


    private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();

    private void bwTest_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
        if (e.Argument is not DiskWalker walker) { return; }

        var swTotal = Stopwatch.StartNew();
        var nextUpdate = swTotal.ElapsedMilliseconds;

        var okCount = 0;
        var nokCount = 0;

        var dataOut = new byte[walker.MaxBufferSize];
        var dataIn = new byte[walker.MaxBufferSize];

        var writeSpeed = new MovingAverage(42);
        var readSpeed = new MovingAverage(42);

        var blockCount = walker.BlockCount;
        for (var i = 0; i < blockCount; i++) {
            if (walker.AllowWrite) {
                var swRandom = Stopwatch.StartNew();
                Rnd.GetBytes(dataOut);
                swRandom.Stop();
            }

            var ok = true;

            if (walker.AllowWrite) {
                var swWrite = Stopwatch.StartNew();
                ok &= walker.Write(dataOut);
                var writeTime = (double)swWrite.ElapsedMilliseconds / 1000;
                if (writeTime == 0) { writeTime = 0.000001; }
                writeSpeed.Add(walker.OffsetLength / writeTime);
            }

            if (walker.AllowRead) {
                var swRead = Stopwatch.StartNew();
                ok &= walker.Read(dataIn);
                var readTime = (double)swRead.ElapsedMilliseconds / 1000;
                if (readTime == 0) { readTime = 0.000001; }
                readSpeed.Add(walker.OffsetLength / readTime);
            }

            if (walker.AllowRead && walker.AllowWrite) {
                ok &= DiskWalker.Validate(dataOut, dataIn);
            }
            if (ok) { okCount += 1; } else { nokCount += 1; }
            dfgMain.SetBlockState(walker.BlockIndex, ok);

            if (bwTest.CancellationPending) {
                walker.Close();
                e.Cancel = true;
                break;
            }

            if (nextUpdate < swTotal.ElapsedMilliseconds) {
                var progress = new ProgressObjectState(swTotal, walker.Index + 1, walker.BlockCount, okCount, nokCount, walker.MaxBufferSize, writeSpeed.Average, readSpeed.Average);
                bwTest.ReportProgress(0, progress);
                nextUpdate = swTotal.ElapsedMilliseconds + 420;  // next update in 420ms
            }

            walker.Index += 1;
        }

        walker.Close();
        var finalProgress = new ProgressObjectState(swTotal, walker.Index, walker.BlockCount, okCount, nokCount, walker.MaxBufferSize, writeSpeed.Average, readSpeed.Average);
        bwTest.ReportProgress(100, finalProgress);
        e.Result = finalProgress;
    }

    private void bwTest_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
        if (e.UserState is ProgressObjectState state) {
            staWriteSpeed.Text = $"W:{state.WriteSpeed:#,##0} MB/s";
            staReadSpeed.Text = $"R:{state.ReadSpeed:#,##0} MB/s";
            if (state.NokCount > 0) {
                staErrors.Text = $"{state.NokCount:#,##0} " + ((state.NokCount != 1) ? "errors" : "error");
            }
            staPercents.Text = state.Percents.ToString("0.000", CultureInfo.CurrentCulture) + "%";
            staProgress.Value = state.Permilles;
            staRemaining.Text = state.EstimatedRemainingAsString + " remaining";
        }
    }

    private void bwTest_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
        dfgMain.Invalidate();
        if (e.Cancelled) {
            Medo.Windows.Forms.MessageBox.ShowWarning(this, "Operation cancelled.");
        } else if (e.Error != null) {
            Medo.Windows.Forms.MessageBox.ShowError(this, e.Error.Message);
        } else {
            if (e.Result is ProgressObjectState state) {
                var text = $"Verification completed in {state.TimeUsedAsString}.";
                if (state.NokCount == 0) {
                    text += "\nNo errors found.";
                    Medo.Windows.Forms.MessageBox.ShowInformation(this, text);
                } else {
                    text += $"\n{state.NokCount} " + ((state.NokCount != 1) ? "errors" : "error") + " found.";
                    Medo.Windows.Forms.MessageBox.ShowError(this, text);
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
        mnuRefresh.Enabled = !testing;
        staWriteSpeed.Visible = testingWrite;
        staWriteSpeed.Text = "";
        staReadSpeed.Visible = testingRead;
        staReadSpeed.Text = "";
        staErrors.Visible = testing;
        staErrors.Text = "";
        staPercents.Visible = testing;
        staPercents.Text = "";
        staPercents.Height = staDisk.Height;
        staProgress.Visible = testing;
        staProgress.Value = 0;
        staRemaining.Visible = testing;
        staRemaining.Text = "";
    }

    private static DiskWalker GetWalker(PhysicalDisk disk, bool allowRead = false, bool allowWrite = false) {
        var blockSizeMB = disk.SizeInGB > 1000 ? 16 : disk.SizeInGB > 1000 ? 8 : 4;
        return new DiskWalker(disk, blockSizeMB) {
            AllowRead = allowRead,
            AllowWrite = allowWrite
        };
    }

}
