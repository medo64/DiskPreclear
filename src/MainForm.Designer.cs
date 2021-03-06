namespace DiskPreclear;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.bwTest = new System.ComponentModel.BackgroundWorker();
            this.mnu = new System.Windows.Forms.ToolStrip();
            this.mnuDisks = new System.Windows.Forms.ToolStripComboBox();
            this.mnuExecute = new System.Windows.Forms.ToolStripSplitButton();
            this.mnuExecuteUseRW = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecuteUseRO = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecuteUseWO = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuOrder = new System.Windows.Forms.ToolStripSplitButton();
            this.mnuOrderSequential = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOrderRandom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPattern = new System.Windows.Forms.ToolStripSplitButton();
            this.mnuPatternSecure = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPatternRandom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPatternRepeat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPatternZero = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRefresh = new System.Windows.Forms.ToolStripButton();
            this.mnuApp = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuAppFeedback = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAppUpgrade = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuApp0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAppAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.sta = new System.Windows.Forms.StatusStrip();
            this.staDisk = new System.Windows.Forms.ToolStripStatusLabel();
            this.staWriteSpeed = new System.Windows.Forms.ToolStripStatusLabel();
            this.staReadSpeed = new System.Windows.Forms.ToolStripStatusLabel();
            this.staValidationErrors = new System.Windows.Forms.ToolStripStatusLabel();
            this.staAccessErrors = new System.Windows.Forms.ToolStripStatusLabel();
            this.staPercents = new System.Windows.Forms.ToolStripStatusLabel();
            this.staProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.staRemaining = new System.Windows.Forms.ToolStripStatusLabel();
            this.staProcessed = new System.Windows.Forms.ToolStripStatusLabel();
            this.staElementMB = new System.Windows.Forms.ToolStripStatusLabel();
            this.dfgMain = new DiskPreclear.Controls.DefragControl();
            this.bwUpgradeCheck = new System.ComponentModel.BackgroundWorker();
            this.mnu.SuspendLayout();
            this.sta.SuspendLayout();
            this.SuspendLayout();
            // 
            // bwTest
            // 
            this.bwTest.WorkerReportsProgress = true;
            this.bwTest.WorkerSupportsCancellation = true;
            this.bwTest.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwTest_DoWork);
            this.bwTest.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwTest_ProgressChanged);
            this.bwTest.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwTest_RunWorkerCompleted);
            // 
            // mnu
            // 
            this.mnu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mnu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisks,
            this.mnuExecute,
            this.mnu0,
            this.mnuOrder,
            this.mnuPattern,
            this.mnu1,
            this.mnuRefresh,
            this.mnuApp});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(782, 28);
            this.mnu.TabIndex = 2;
            // 
            // mnuDisks
            // 
            this.mnuDisks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mnuDisks.Name = "mnuDisks";
            this.mnuDisks.Size = new System.Drawing.Size(270, 28);
            this.mnuDisks.SelectedIndexChanged += new System.EventHandler(this.mnuDisks_SelectedIndexChanged);
            // 
            // mnuExecute
            // 
            this.mnuExecute.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExecuteUseRW,
            this.mnuExecuteUseRO,
            this.mnuExecuteUseWO});
            this.mnuExecute.Image = global::DiskPreclear.Properties.Resources.mnuExecuteRW_16;
            this.mnuExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuExecute.Name = "mnuExecute";
            this.mnuExecute.Size = new System.Drawing.Size(154, 25);
            this.mnuExecute.Tag = "mnuExecuteRW";
            this.mnuExecute.Text = "Test Read/Write";
            this.mnuExecute.ToolTipText = "Start disk test";
            this.mnuExecute.ButtonClick += new System.EventHandler(this.mnuExecute_Click);
            this.mnuExecute.DropDownOpening += new System.EventHandler(this.mnuExecute_DropDownOpening);
            // 
            // mnuExecuteUseRW
            // 
            this.mnuExecuteUseRW.Name = "mnuExecuteUseRW";
            this.mnuExecuteUseRW.Size = new System.Drawing.Size(254, 26);
            this.mnuExecuteUseRW.Text = "Read/Write (destructive)";
            this.mnuExecuteUseRW.Click += new System.EventHandler(this.mnuExecuteUseRW_Click);
            // 
            // mnuExecuteUseRO
            // 
            this.mnuExecuteUseRO.Name = "mnuExecuteUseRO";
            this.mnuExecuteUseRO.Size = new System.Drawing.Size(254, 26);
            this.mnuExecuteUseRO.Text = "Read-only";
            this.mnuExecuteUseRO.Click += new System.EventHandler(this.mnuExecuteUseRO_Click);
            // 
            // mnuExecuteUseWO
            // 
            this.mnuExecuteUseWO.Name = "mnuExecuteUseWO";
            this.mnuExecuteUseWO.Size = new System.Drawing.Size(254, 26);
            this.mnuExecuteUseWO.Text = "Write-only (destructive)";
            this.mnuExecuteUseWO.Click += new System.EventHandler(this.mnuExecuteUseWO_Click);
            // 
            // mnu0
            // 
            this.mnu0.Name = "mnu0";
            this.mnu0.Size = new System.Drawing.Size(6, 28);
            // 
            // mnuOrder
            // 
            this.mnuOrder.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOrderRandom,
            this.mnuOrderSequential});
            this.mnuOrder.Image = global::DiskPreclear.Properties.Resources.mnuOrder_16;
            this.mnuOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuOrder.Name = "mnuOrder";
            this.mnuOrder.Size = new System.Drawing.Size(104, 25);
            this.mnuOrder.Tag = "mnuOrderRandom";
            this.mnuOrder.Text = "Random";
            this.mnuOrder.ToolTipText = "Select order of operations";
            this.mnuOrder.ButtonClick += new System.EventHandler(this.mnuOrder_ButtonClick);
            this.mnuOrder.DropDownOpening += new System.EventHandler(this.mnuOrder_DropDownOpening);
            // 
            // mnuOrderSequential
            // 
            this.mnuOrderSequential.Name = "mnuOrderSequential";
            this.mnuOrderSequential.Size = new System.Drawing.Size(202, 26);
            this.mnuOrderSequential.Text = "Sequential order";
            this.mnuOrderSequential.ToolTipText = "Disk is accessed in the sequential order";
            this.mnuOrderSequential.Click += new System.EventHandler(this.mnuOrderSequential_Click);
            // 
            // mnuOrderRandom
            // 
            this.mnuOrderRandom.Name = "mnuOrderRandom";
            this.mnuOrderRandom.Size = new System.Drawing.Size(202, 26);
            this.mnuOrderRandom.Text = "Random order";
            this.mnuOrderRandom.ToolTipText = "Disk is accessed in the random order";
            this.mnuOrderRandom.Click += new System.EventHandler(this.mnuOrderRandom_Click);
            // 
            // mnuPattern
            // 
            this.mnuPattern.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPatternSecure,
            this.mnuPatternRandom,
            this.mnuPatternRepeat,
            this.mnuPatternZero});
            this.mnuPattern.Image = global::DiskPreclear.Properties.Resources.mnuPattern_16;
            this.mnuPattern.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuPattern.Name = "mnuPattern";
            this.mnuPattern.Size = new System.Drawing.Size(104, 25);
            this.mnuPattern.Tag = "mnuPatternRandom";
            this.mnuPattern.Text = "Random";
            this.mnuPattern.ToolTipText = "Select what data pattern will be used for write operations";
            this.mnuPattern.ButtonClick += new System.EventHandler(this.mnuPattern_ButtonClick);
            this.mnuPattern.DropDownOpening += new System.EventHandler(this.mnuPattern_DropDownOpening);
            // 
            // mnuPatternSecure
            // 
            this.mnuPatternSecure.Name = "mnuPatternSecure";
            this.mnuPatternSecure.Size = new System.Drawing.Size(160, 26);
            this.mnuPatternSecure.Text = "Secure";
            this.mnuPatternSecure.ToolTipText = "Data is written to disk twice, each using secure random";
            this.mnuPatternSecure.Click += new System.EventHandler(this.mnuPatternSecure_Click);
            // 
            // mnuPatternRandom
            // 
            this.mnuPatternRandom.Name = "mnuPatternRandom";
            this.mnuPatternRandom.Size = new System.Drawing.Size(160, 26);
            this.mnuPatternRandom.Text = "Random";
            this.mnuPatternRandom.ToolTipText = "Data is written using secure random";
            this.mnuPatternRandom.Click += new System.EventHandler(this.mnuPatternRandom_Click);
            // 
            // mnuPatternRepeat
            // 
            this.mnuPatternRepeat.Name = "mnuPatternRepeat";
            this.mnuPatternRepeat.Size = new System.Drawing.Size(160, 26);
            this.mnuPatternRepeat.Text = "Repeating";
            this.mnuPatternRepeat.ToolTipText = "Data is written using repeating pattern";
            this.mnuPatternRepeat.Click += new System.EventHandler(this.mnuPatternRepeat_Click);
            // 
            // mnuPatternZero
            // 
            this.mnuPatternZero.Name = "mnuPatternZero";
            this.mnuPatternZero.Size = new System.Drawing.Size(160, 26);
            this.mnuPatternZero.Text = "Zero";
            this.mnuPatternZero.ToolTipText = "Data is written using zeros";
            this.mnuPatternZero.Click += new System.EventHandler(this.mnuPatternZero_Click);
            // 
            // mnu1
            // 
            this.mnu1.Name = "mnu1";
            this.mnu1.Size = new System.Drawing.Size(6, 28);
            // 
            // mnuRefresh
            // 
            this.mnuRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuRefresh.Image = global::DiskPreclear.Properties.Resources.mnuRefresh_16;
            this.mnuRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuRefresh.Name = "mnuRefresh";
            this.mnuRefresh.Size = new System.Drawing.Size(29, 25);
            this.mnuRefresh.Text = "Refresh";
            this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
            // 
            // mnuApp
            // 
            this.mnuApp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuApp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAppFeedback,
            this.mnuAppUpgrade,
            this.mnuApp0,
            this.mnuAppAbout});
            this.mnuApp.Image = global::DiskPreclear.Properties.Resources.mnuApp_16;
            this.mnuApp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuApp.Name = "mnuApp";
            this.mnuApp.Size = new System.Drawing.Size(34, 25);
            this.mnuApp.Text = "Application";
            // 
            // mnuAppFeedback
            // 
            this.mnuAppFeedback.Name = "mnuAppFeedback";
            this.mnuAppFeedback.Size = new System.Drawing.Size(216, 26);
            this.mnuAppFeedback.Text = "Send &Feedback";
            this.mnuAppFeedback.Click += new System.EventHandler(this.mnuAppFeedback_Click);
            // 
            // mnuAppUpgrade
            // 
            this.mnuAppUpgrade.Name = "mnuAppUpgrade";
            this.mnuAppUpgrade.Size = new System.Drawing.Size(216, 26);
            this.mnuAppUpgrade.Text = "Check for &Upgrade";
            this.mnuAppUpgrade.Click += new System.EventHandler(this.mnuAppUpgrade_Click);
            // 
            // mnuApp0
            // 
            this.mnuApp0.Name = "mnuApp0";
            this.mnuApp0.Size = new System.Drawing.Size(213, 6);
            // 
            // mnuAppAbout
            // 
            this.mnuAppAbout.Name = "mnuAppAbout";
            this.mnuAppAbout.Size = new System.Drawing.Size(216, 26);
            this.mnuAppAbout.Text = "&About";
            this.mnuAppAbout.Click += new System.EventHandler(this.mnuAppAbout_Click);
            // 
            // sta
            // 
            this.sta.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.sta.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staDisk,
            this.staWriteSpeed,
            this.staReadSpeed,
            this.staValidationErrors,
            this.staAccessErrors,
            this.staPercents,
            this.staProgress,
            this.staRemaining,
            this.staProcessed,
            this.staElementMB});
            this.sta.Location = new System.Drawing.Point(0, 407);
            this.sta.Name = "sta";
            this.sta.ShowItemToolTips = true;
            this.sta.Size = new System.Drawing.Size(782, 26);
            this.sta.TabIndex = 3;
            // 
            // staDisk
            // 
            this.staDisk.Margin = new System.Windows.Forms.Padding(0, 4, 2, 2);
            this.staDisk.Name = "staDisk";
            this.staDisk.Size = new System.Drawing.Size(15, 20);
            this.staDisk.Text = "-";
            // 
            // staWriteSpeed
            // 
            this.staWriteSpeed.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.staWriteSpeed.Name = "staWriteSpeed";
            this.staWriteSpeed.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.staWriteSpeed.Size = new System.Drawing.Size(26, 20);
            this.staWriteSpeed.Text = "W:";
            this.staWriteSpeed.Visible = false;
            // 
            // staReadSpeed
            // 
            this.staReadSpeed.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.staReadSpeed.Name = "staReadSpeed";
            this.staReadSpeed.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.staReadSpeed.Size = new System.Drawing.Size(21, 20);
            this.staReadSpeed.Text = "R:";
            this.staReadSpeed.Visible = false;
            // 
            // staAccessErrors
            // 
            this.staAccessErrors.ForeColor = System.Drawing.Color.DarkRed;
            this.staAccessErrors.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.staAccessErrors.Name = "staAccessErrors";
            this.staAccessErrors.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.staAccessErrors.Size = new System.Drawing.Size(47, 20);
            this.staAccessErrors.Text = "access errors";
            this.staAccessErrors.Visible = false;
            // 
            // staValidationErrors
            // 
            this.staValidationErrors.ForeColor = System.Drawing.Color.Red;
            this.staValidationErrors.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.staValidationErrors.Name = "staValidationErrors";
            this.staValidationErrors.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.staValidationErrors.Size = new System.Drawing.Size(47, 20);
            this.staValidationErrors.Text = "errors";
            this.staValidationErrors.Visible = false;
            // 
            // staPercents
            // 
            this.staPercents.Margin = new System.Windows.Forms.Padding(2, 4, 0, 2);
            this.staPercents.Name = "staPercents";
            this.staPercents.Size = new System.Drawing.Size(29, 20);
            this.staPercents.Text = "0%";
            this.staPercents.Visible = false;
            // 
            // staProgress
            // 
            this.staProgress.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.staProgress.Maximum = 1000;
            this.staProgress.Name = "staProgress";
            this.staProgress.Size = new System.Drawing.Size(270, 20);
            this.staProgress.Visible = false;
            // 
            // staRemaining
            // 
            this.staRemaining.Name = "staRemaining";
            this.staRemaining.Size = new System.Drawing.Size(76, 20);
            this.staRemaining.Text = "remaining";
            this.staRemaining.Visible = false;
            // 
            // staProcessed
            // 
            this.staProcessed.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.staProcessed.Name = "staProcessed";
            this.staProcessed.Size = new System.Drawing.Size(102, 20);
            this.staProcessed.Text = "MB processed";
            this.staProcessed.Visible = false;
            // 
            // staElementMB
            // 
            this.staElementMB.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.staElementMB.Name = "staElementMB";
            this.staElementMB.Size = new System.Drawing.Size(85, 20);
            this.staElementMB.Text = "block = MB";
            // 
            // dfgMain
            // 
            this.dfgMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dfgMain.Location = new System.Drawing.Point(0, 28);
            this.dfgMain.Name = "dfgMain";
            this.dfgMain.Size = new System.Drawing.Size(782, 379);
            this.dfgMain.TabIndex = 4;
            this.dfgMain.Text = "defragControl1";
            this.dfgMain.Walker = null;
            this.dfgMain.ElementCountUpdated += new System.EventHandler<System.EventArgs>(this.dfgMain_ElementCountUpdated);
            // 
            // bwUpgradeCheck
            // 
            this.bwUpgradeCheck.WorkerSupportsCancellation = true;
            this.bwUpgradeCheck.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwUpgradeCheck_DoWork);
            this.bwUpgradeCheck.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwUpgradeCheck_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 433);
            this.Controls.Add(this.dfgMain);
            this.Controls.Add(this.sta);
            this.Controls.Add(this.mnu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(480, 320);
            this.Name = "MainForm";
            this.Text = "Disk Preclear";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.sta.ResumeLayout(false);
            this.sta.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private System.ComponentModel.BackgroundWorker bwTest;
    private System.Windows.Forms.ToolStrip mnu;
    private System.Windows.Forms.ToolStripComboBox mnuDisks;
    private System.Windows.Forms.StatusStrip sta;
    private System.Windows.Forms.ToolStripProgressBar staProgress;
    private System.Windows.Forms.ToolStripSeparator mnu0;
    private System.Windows.Forms.ToolStripButton mnuRefresh;
    private System.Windows.Forms.ToolStripStatusLabel staPercents;
    private System.Windows.Forms.ToolStripStatusLabel staRemaining;
    private System.Windows.Forms.ToolStripStatusLabel staWriteSpeed;
    private System.Windows.Forms.ToolStripStatusLabel staReadSpeed;
    private System.Windows.Forms.ToolStripStatusLabel staDisk;
    private System.Windows.Forms.ToolStripDropDownButton mnuApp;
    private System.Windows.Forms.ToolStripMenuItem mnuAppAbout;
    private Controls.DefragControl dfgMain;
    private System.Windows.Forms.ToolStripStatusLabel staAccessErrors;
    private System.Windows.Forms.ToolStripStatusLabel staValidationErrors;
    private System.Windows.Forms.ToolStripSplitButton mnuExecute;
    private System.Windows.Forms.ToolStripMenuItem mnuExecuteUseRW;
    private System.Windows.Forms.ToolStripMenuItem mnuExecuteUseRO;
    private System.Windows.Forms.ToolStripMenuItem mnuExecuteUseWO;
    private System.Windows.Forms.ToolStripMenuItem mnuAppFeedback;
    private System.Windows.Forms.ToolStripSeparator mnuApp0;
    private System.Windows.Forms.ToolStripMenuItem mnuAppUpgrade;
    private System.Windows.Forms.ToolStripStatusLabel staElementMB;
    private System.Windows.Forms.ToolStripSplitButton mnuPattern;
    private System.Windows.Forms.ToolStripMenuItem mnuPatternRandom;
    private System.Windows.Forms.ToolStripMenuItem mnuPatternRepeat;
    private System.Windows.Forms.ToolStripMenuItem mnuPatternZero;
    private System.Windows.Forms.ToolStripSeparator mnu1;
    private System.ComponentModel.BackgroundWorker bwUpgradeCheck;
    private System.Windows.Forms.ToolStripSplitButton mnuOrder;
    private System.Windows.Forms.ToolStripMenuItem mnuOrderRandom;
    private System.Windows.Forms.ToolStripMenuItem mnuOrderSequential;
    private System.Windows.Forms.ToolStripStatusLabel staProcessed;
    private System.Windows.Forms.ToolStripMenuItem mnuPatternSecure;
}
