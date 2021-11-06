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
            this.staProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.staPercentage = new System.Windows.Forms.ToolStripStatusLabel();
            this.staRemaining = new System.Windows.Forms.ToolStripStatusLabel();
            this.staWriteSpeed = new System.Windows.Forms.ToolStripStatusLabel();
            this.staReadSpeed = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.mnuRefresh});
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
            this.staProgress,
            this.staPercentage,
            this.staRemaining,
            this.staWriteSpeed,
            this.staReadSpeed});
            this.sta.Location = new System.Drawing.Point(0, 527);
            this.sta.Name = "sta";
            this.sta.Size = new System.Drawing.Size(782, 26);
            this.sta.TabIndex = 3;
            this.sta.Text = "statusStrip1";
            // 
            // staProgress
            // 
            this.staProgress.Name = "staProgress";
            this.staProgress.Size = new System.Drawing.Size(270, 18);
            this.staProgress.Visible = false;
            // 
            // staPercentage
            // 
            this.staPercentage.Name = "staPercentage";
            this.staPercentage.Size = new System.Drawing.Size(29, 20);
            this.staPercentage.Text = "0%";
            this.staPercentage.Visible = false;
            // 
            // staRemaining
            // 
            this.staRemaining.Name = "staRemaining";
            this.staRemaining.Size = new System.Drawing.Size(15, 20);
            this.staRemaining.Text = "-";
            // 
            // staWriteSpeed
            // 
            this.staWriteSpeed.Name = "staWriteSpeed";
            this.staWriteSpeed.Size = new System.Drawing.Size(26, 20);
            this.staWriteSpeed.Text = "W:";
            this.staWriteSpeed.Visible = false;
            // 
            // staReadSpeed
            // 
            this.staReadSpeed.Name = "staReadSpeed";
            this.staReadSpeed.Size = new System.Drawing.Size(21, 20);
            this.staReadSpeed.Text = "R:";
            this.staReadSpeed.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
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
    private System.Windows.Forms.ToolStripStatusLabel staPercentage;
    private System.Windows.Forms.ToolStripStatusLabel staRemaining;
    private System.Windows.Forms.ToolStripStatusLabel staWriteSpeed;
    private System.Windows.Forms.ToolStripStatusLabel staReadSpeed;
}
