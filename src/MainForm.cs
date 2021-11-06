using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DiskPreclear;

internal partial class MainForm : Form {
    public MainForm() {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, System.EventArgs e) {
        FillCombo();
    }


    private void cmbPhysicalDisks_SelectedValueChanged(object sender, System.EventArgs e) {
        btnTest.Enabled = cmbPhysicalDisks.SelectedItem as PhysicalDisk is not null;
    }


    private void btnRefresh_Click(object sender, System.EventArgs e) {
        FillCombo();
    }

    private void btnTest_Click(object sender, System.EventArgs e) {
        if (cmbPhysicalDisks.SelectedItem is PhysicalDisk disk) {
            if (disk.SizeInGB != 14000) { return; }  // TOFIX: to save my ass during testing

            cmbPhysicalDisks.Enabled = false;
            btnRefresh.Visible = false;
            prgTest.Visible = true;
            btnTest.Visible = false;
            btnCancel.Visible = true;

            prgTest.Value = 0;
            bwTest.RunWorkerAsync(new DiskWalker(disk));
        }
    }

    private void btnCancel_Click(object sender, System.EventArgs e) {
        btnCancel.Enabled = false;
        if (bwTest.IsBusy) { bwTest.CancelAsync(); }
    }


    private void bwTest_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
        if (e.Argument is not DiskWalker walker) { return; }

        var blockCount = walker.BlockCount;
        for (var i = 0; i < blockCount; i++) {
            var swRandom = Stopwatch.StartNew();
            var dataOut = walker.GetRandomData();
            var dataIn = new byte[dataOut.Length];
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

            var percentage = (int)(i * 100 / blockCount);
            bwTest.ReportProgress(percentage, $"{i:#,##0}/{blockCount:#,##0}");

            if (bwTest.CancellationPending) { break; }

            walker.Index += 1;
        }
    }

    private void bwTest_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
        prgTest.Value = e.ProgressPercentage;
        Text = e.UserState as string ?? "";
    }

    private void bwTest_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
        cmbPhysicalDisks.Enabled = true;
        btnRefresh.Visible = true;
        prgTest.Visible = false;
        btnTest.Visible = true;
        btnCancel.Visible = false;
    }


    private void FillCombo() {
        btnTest.Enabled = false;

        cmbPhysicalDisks.BeginUpdate();

        var prevSelection = cmbPhysicalDisks.SelectedItem as PhysicalDisk;

        cmbPhysicalDisks.Items.Clear();
        foreach (var disk in PhysicalDisk.GetAllDisks()) {
            cmbPhysicalDisks.Items.Add(disk);
            if ((prevSelection is not null) && (disk.Number == prevSelection.Number)) {
                cmbPhysicalDisks.SelectedItem = disk;
            }
        }
        if ((cmbPhysicalDisks.SelectedItem is null) && (cmbPhysicalDisks.Items.Count > 0)) {  // select the last disk
            cmbPhysicalDisks.SelectedIndex = cmbPhysicalDisks.Items.Count - 1;
        }

        cmbPhysicalDisks.EndUpdate();
    }

}
