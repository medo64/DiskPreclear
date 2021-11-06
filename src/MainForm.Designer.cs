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
            this.cmbPhysicalDisks = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.bwTest = new System.ComponentModel.BackgroundWorker();
            this.prgTest = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbPhysicalDisks
            // 
            this.cmbPhysicalDisks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPhysicalDisks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPhysicalDisks.FormattingEnabled = true;
            this.cmbPhysicalDisks.Location = new System.Drawing.Point(12, 12);
            this.cmbPhysicalDisks.Name = "cmbPhysicalDisks";
            this.cmbPhysicalDisks.Size = new System.Drawing.Size(318, 28);
            this.cmbPhysicalDisks.TabIndex = 0;
            this.cmbPhysicalDisks.SelectedValueChanged += new System.EventHandler(this.cmbPhysicalDisks_SelectedValueChanged);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Enabled = false;
            this.btnTest.Location = new System.Drawing.Point(236, 46);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(94, 29);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Preclear";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 46);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(94, 29);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.TabStop = false;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // bwTest
            // 
            this.bwTest.WorkerReportsProgress = true;
            this.bwTest.WorkerSupportsCancellation = true;
            this.bwTest.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwTest_DoWork);
            this.bwTest.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwTest_ProgressChanged);
            this.bwTest.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwTest_RunWorkerCompleted);
            // 
            // prgTest
            // 
            this.prgTest.Location = new System.Drawing.Point(12, 46);
            this.prgTest.Name = "prgTest";
            this.prgTest.Size = new System.Drawing.Size(218, 29);
            this.prgTest.TabIndex = 3;
            this.prgTest.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(236, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 29);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnTest;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(342, 87);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.cmbPhysicalDisks);
            this.Controls.Add(this.prgTest);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Disk Preclear";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox cmbPhysicalDisks;
    private System.Windows.Forms.Button btnTest;
    private System.Windows.Forms.Button btnRefresh;
    private System.ComponentModel.BackgroundWorker bwTest;
    private System.Windows.Forms.ProgressBar prgTest;
    private System.Windows.Forms.Button btnCancel;
}
