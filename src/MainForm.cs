using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        mnuStart.Enabled = mnuDisks.SelectedItem as PhysicalDisk is not null;
    }

    private void mnuStart_Click(object sender, System.EventArgs e) {
        if (mnuDisks.SelectedItem is PhysicalDisk disk) {
            if (disk.SizeInGB != 14000) { return; }  // TOFIX: to save my ass during testing

            PrepareToStart(true);
            bwTest.RunWorkerAsync(new DiskWalker(disk));
        }
    }

    private void mnuRefresh_Click(object sender, System.EventArgs e) {
        FillDisks();
    }


    private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();

    private void bwTest_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
        if (e.Argument is not DiskWalker walker) { return; }

        var swTotal = Stopwatch.StartNew();
        var nextUpdate = swTotal.ElapsedMilliseconds;

        var dataOut = new byte[DiskWalker.MaxBufferSize];
        var dataIn = new byte[DiskWalker.MaxBufferSize];

        var blockCount = walker.BlockCount;
        for (var i = 0; i < blockCount; i++) {
            var swRandom = Stopwatch.StartNew();
            Rnd.GetBytes(dataOut);
            swRandom.Stop();

            var swWrite = Stopwatch.StartNew();
            walker.Write(dataOut);
            swWrite.Stop();

            var swRead = Stopwatch.StartNew();
            walker.Read(dataIn);
            swRead.Stop();

            if (!DiskWalker.Validate(dataOut, dataIn)) {
                throw new InvalidDataException();
            }

            if (bwTest.CancellationPending) { break; }

            if (nextUpdate < swTotal.ElapsedMilliseconds) {
                var progress = new ProgressObjectState(swTotal, walker.Index + 1, walker.BlockCount);
                bwTest.ReportProgress(progress.PercentageAsInt, progress);
                nextUpdate = swTotal.ElapsedMilliseconds + 420;  // next update in 420ms
            }

            walker.Index += 1;
        }
    }

    private void bwTest_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
        staProgress.Value = e.ProgressPercentage;
        if (e.UserState is ProgressObjectState state) {
            staPercentage.Text = state.Percentage.ToString("0.000", CultureInfo.CurrentCulture) + "%";
            staRemaining.Text = state.EstimatedRemainingAsString;
        }
    }

    private void bwTest_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
        PrepareToStart(false);
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

    private void PrepareToStart(bool starting) {
        mnuDisks.Enabled = !starting;
        mnuStart.Enabled = !starting;
        mnuRefresh.Enabled = !starting;
        staProgress.Visible = starting;
        staProgress.Value = 0;
        staPercentage.Visible = starting;
        staPercentage.Text = "";
        staRemaining.Visible = starting;
        staRemaining.Text = "";
    }

}
