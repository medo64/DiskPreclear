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
            this.mnuStart = new System.Windows.Forms.ToolStripButton();
            this.mnu0 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRefresh = new System.Windows.Forms.ToolStripButton();
            this.sta = new System.Windows.Forms.StatusStrip();
            this.staDisk = new System.Windows.Forms.ToolStripStatusLabel();
            this.staWriteSpeed = new System.Windows.Forms.ToolStripStatusLabel();
            this.staReadSpeed = new System.Windows.Forms.ToolStripStatusLabel();
            this.staPercents = new System.Windows.Forms.ToolStripStatusLabel();
            this.staProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.staRemaining = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnuApp = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuAppAbout = new System.Windows.Forms.ToolStripMenuItem();
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
            this.mnuStart,
            this.mnu0,
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
            // mnuStart
            // 
            this.mnuStart.Image = global::DiskPreclear.Properties.Resources.mnuStart_16;
            this.mnuStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuStart.Name = "mnuStart";
            this.mnuStart.Size = new System.Drawing.Size(64, 25);
            this.mnuStart.Text = "Start";
            this.mnuStart.Click += new System.EventHandler(this.mnuStart_Click);
            // 
            // mnu0
            // 
            this.mnu0.Name = "mnu0";
            this.mnu0.Size = new System.Drawing.Size(6, 28);
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
            // sta
            // 
            this.sta.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.sta.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staDisk,
            this.staWriteSpeed,
            this.staReadSpeed,
            this.staPercents,
            this.staProgress,
            this.staRemaining});
            this.sta.Location = new System.Drawing.Point(0, 407);
            this.sta.Name = "sta";
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
            this.staWriteSpeed.Size = new System.Drawing.Size(26, 20);
            this.staWriteSpeed.Text = "W:";
            this.staWriteSpeed.Visible = false;
            // 
            // staReadSpeed
            // 
            this.staReadSpeed.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.staReadSpeed.Name = "staReadSpeed";
            this.staReadSpeed.Size = new System.Drawing.Size(21, 20);
            this.staReadSpeed.Text = "R:";
            this.staReadSpeed.Visible = false;
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
            // mnuApp
            // 
            this.mnuApp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuApp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAppAbout});
            this.mnuApp.Image = global::DiskPreclear.Properties.Resources.mnuApp_16;
            this.mnuApp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuApp.Name = "mnuApp";
            this.mnuApp.Size = new System.Drawing.Size(34, 25);
            this.mnuApp.Text = "Application";
            // 
            // mnuAppAbout
            // 
            this.mnuAppAbout.Name = "mnuAppAbout";
            this.mnuAppAbout.Size = new System.Drawing.Size(224, 26);
            this.mnuAppAbout.Text = "&About";
            this.mnuAppAbout.Click += new System.EventHandler(this.mnuAppAbout_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 433);
            this.Controls.Add(this.sta);
            this.Controls.Add(this.mnu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
    private System.Windows.Forms.ToolStripButton mnuStart;
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
}
