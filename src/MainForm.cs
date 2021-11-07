using Medo.Math;
using Medo.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Forms;

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
        var disk = mnuDisks.SelectedItem as PhysicalDisk;
        dfgMain.Walker = GetWalker(disk);
        mnuStart.Enabled = disk is not null;
        staDisk.Text = disk?.Path ?? "-";
    }

    private void mnuStart_Click(object sender, System.EventArgs e) {
        if (mnuDisks.SelectedItem is PhysicalDisk disk) {
            var hasVolumes = disk.GetLogicalVolumes().Count > 0;
            var hasPaths = false;
            var diskData = $"\n  Physical disk {disk.Number}"
                         + $"\n  {disk.SizeInGB} GB";
            foreach (var path in disk.GetLogicalVolumePaths()) {
                hasPaths = true;
                diskData += "\n  " + path;
            }

            if (Medo.Windows.Forms.MessageBox.ShowQuestion(this, "Are you sure you want to perform test on" + diskData, MessageBoxButtons.YesNo) == DialogResult.No) { return; }
            if (hasVolumes && Medo.Windows.Forms.MessageBox.ShowWarning(this, "Selected disk has volumes present.\nAre you readlly sure you want to perform test on" + diskData, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }
            if (hasPaths && Medo.Windows.Forms.MessageBox.ShowError(this, "Selected disk is in use!\nAre you goddamn sure you want to perform test on" + diskData, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }

            PrepareForTesting(true);
            bwTest.RunWorkerAsync(GetWalker(disk));
        }
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
            var swRandom = Stopwatch.StartNew();
            Rnd.GetBytes(dataOut);
            swRandom.Stop();

            var swWrite = Stopwatch.StartNew();
            walker.Write(dataOut);
            var writeTime = (double)swWrite.ElapsedMilliseconds / 1000;
            writeSpeed.Add(walker.OffsetLength / writeTime);

            var swRead = Stopwatch.StartNew();
            walker.Read(dataIn);
            var readTime = (double)swRead.ElapsedMilliseconds / 1000;
            readSpeed.Add(walker.OffsetLength / readTime);

            var ok = DiskWalker.Validate(dataOut, dataIn);
            if (ok) { okCount += 1; } else { nokCount += 1; }
            dfgMain.SetBlockState(walker.BlockIndex, ok);

            if (bwTest.CancellationPending) {
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

        e.Result = new ProgressObjectState(swTotal, walker.Index + 1, walker.BlockCount, okCount, nokCount, walker.MaxBufferSize, writeSpeed.Average, readSpeed.Average);
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
        PrepareForTesting(false);
    }


    private void FillDisks() {
        mnuStart.Enabled = false;

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

    private void PrepareForTesting(bool testing) {
        mnuDisks.Enabled = !testing;
        mnuStart.Enabled = !testing;
        mnuRefresh.Enabled = !testing;
        staWriteSpeed.Visible = testing;
        staWriteSpeed.Text = "";
        staReadSpeed.Visible = testing;
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

    private static DiskWalker? GetWalker(PhysicalDisk? disk) {
        if (disk == null) { return null; }
        var blockSizeMB = disk.SizeInGB > 1000 ? 16 : disk.SizeInGB > 1000 ? 8 : 4;
        return new DiskWalker(disk, blockSizeMB);
    }

}
