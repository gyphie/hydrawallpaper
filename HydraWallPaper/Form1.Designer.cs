namespace HydraPaper
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.btnStart = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.lblSingleScreenImages = new System.Windows.Forms.Label();
			this.txtSSIImageFolderPath = new System.Windows.Forms.TextBox();
			this.lblRotateTime = new System.Windows.Forms.Label();
			this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
			this.cmsTray = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startStopStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.numRotateMinutes = new System.Windows.Forms.NumericUpDown();
			this.btnSSIImageFolderChooser = new System.Windows.Forms.Button();
			this.bwWallPaper = new System.ComponentModel.BackgroundWorker();
			this.btnChangeNow = new System.Windows.Forms.Button();
			this.lblWorking = new System.Windows.Forms.Label();
			this.cmbSSIImageSize = new System.Windows.Forms.ComboBox();
			this.lblSSIResizeMethod = new System.Windows.Forms.Label();
			this.lblSSIFolder = new System.Windows.Forms.Label();
			this.lblMSIFolder = new System.Windows.Forms.Label();
			this.lblMultiScreenImages = new System.Windows.Forms.Label();
			this.txtMSIImageFolderPath = new System.Windows.Forms.TextBox();
			this.lblMSIResizeMethod = new System.Windows.Forms.Label();
			this.cmbMSIImageSize = new System.Windows.Forms.ComboBox();
			this.btnMSIImageFolderChooser = new System.Windows.Forms.Button();
			this.lblImagesUsed = new System.Windows.Forms.Label();
			this.lbImagesUsed = new System.Windows.Forms.ListBox();
			this.cmsImagesUsed = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cmsTray.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numRotateMinutes)).BeginInit();
			this.cmsImagesUsed.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStart
			// 
			this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStart.Location = new System.Drawing.Point(12, 303);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 14;
			this.btnStart.Text = "&Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnExit
			// 
			this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnExit.Location = new System.Drawing.Point(460, 303);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(75, 23);
			this.btnExit.TabIndex = 19;
			this.btnExit.Text = "E&xit";
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// lblSingleScreenImages
			// 
			this.lblSingleScreenImages.AutoSize = true;
			this.lblSingleScreenImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSingleScreenImages.Location = new System.Drawing.Point(17, 9);
			this.lblSingleScreenImages.Name = "lblSingleScreenImages";
			this.lblSingleScreenImages.Size = new System.Drawing.Size(130, 13);
			this.lblSingleScreenImages.TabIndex = 0;
			this.lblSingleScreenImages.Text = "Single Screen Images";
			// 
			// txtSSIImageFolderPath
			// 
			this.txtSSIImageFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSSIImageFolderPath.Location = new System.Drawing.Point(136, 37);
			this.txtSSIImageFolderPath.Name = "txtSSIImageFolderPath";
			this.txtSSIImageFolderPath.ReadOnly = true;
			this.txtSSIImageFolderPath.Size = new System.Drawing.Size(355, 20);
			this.txtSSIImageFolderPath.TabIndex = 2;
			// 
			// lblRotateTime
			// 
			this.lblRotateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblRotateTime.AutoSize = true;
			this.lblRotateTime.Location = new System.Drawing.Point(215, 308);
			this.lblRotateTime.Name = "lblRotateTime";
			this.lblRotateTime.Size = new System.Drawing.Size(67, 13);
			this.lblRotateTime.TabIndex = 16;
			this.lblRotateTime.Text = "&Rotate (min):";
			// 
			// niTray
			// 
			this.niTray.BalloonTipText = "Hydra Paper";
			this.niTray.ContextMenuStrip = this.cmsTray;
			this.niTray.Icon = ((System.Drawing.Icon)(resources.GetObject("niTray.Icon")));
			this.niTray.Text = "Hydra Paper";
			this.niTray.Visible = true;
			this.niTray.MouseClick += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseClick);
			this.niTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niTray_MouseDoubleClick);
			// 
			// cmsTray
			// 
			this.cmsTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusToolStripMenuItem,
            this.toolStripSeparator2,
            this.nextToolStripMenuItem,
            this.blankToolStripMenuItem,
            this.toolStripSeparator1,
            this.openToolStripMenuItem,
            this.startStopStripMenuItem,
            this.exitToolStripMenuItem});
			this.cmsTray.Name = "cmsTray";
			this.cmsTray.Size = new System.Drawing.Size(107, 148);
			this.cmsTray.Opening += new System.ComponentModel.CancelEventHandler(this.cmsTray_Opening);
			this.cmsTray.VisibleChanged += new System.EventHandler(this.cmsTray_VisibleChanged);
			// 
			// statusToolStripMenuItem
			// 
			this.statusToolStripMenuItem.Enabled = false;
			this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
			this.statusToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			this.statusToolStripMenuItem.Text = "Status";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(103, 6);
			// 
			// nextToolStripMenuItem
			// 
			this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
			this.nextToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			this.nextToolStripMenuItem.Text = "Next";
			this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
			// 
			// blankToolStripMenuItem
			// 
			this.blankToolStripMenuItem.Name = "blankToolStripMenuItem";
			this.blankToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			this.blankToolStripMenuItem.Text = "Blank";
			this.blankToolStripMenuItem.Click += new System.EventHandler(this.blankToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(103, 6);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// startStopStripMenuItem
			// 
			this.startStopStripMenuItem.Name = "startStopStripMenuItem";
			this.startStopStripMenuItem.Size = new System.Drawing.Size(106, 22);
			this.startStopStripMenuItem.Text = "Stop";
			this.startStopStripMenuItem.Click += new System.EventHandler(this.startStopStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// numRotateMinutes
			// 
			this.numRotateMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numRotateMinutes.Location = new System.Drawing.Point(288, 306);
			this.numRotateMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numRotateMinutes.Name = "numRotateMinutes";
			this.numRotateMinutes.Size = new System.Drawing.Size(58, 20);
			this.numRotateMinutes.TabIndex = 17;
			this.numRotateMinutes.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// btnSSIImageFolderChooser
			// 
			this.btnSSIImageFolderChooser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSSIImageFolderChooser.Location = new System.Drawing.Point(497, 36);
			this.btnSSIImageFolderChooser.Name = "btnSSIImageFolderChooser";
			this.btnSSIImageFolderChooser.Size = new System.Drawing.Size(38, 20);
			this.btnSSIImageFolderChooser.TabIndex = 3;
			this.btnSSIImageFolderChooser.Text = "...";
			this.btnSSIImageFolderChooser.UseVisualStyleBackColor = true;
			this.btnSSIImageFolderChooser.Click += new System.EventHandler(this.btnSSIImageFolderChooser_Click);
			// 
			// bwWallPaper
			// 
			this.bwWallPaper.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwWallPaper_DoWork);
			this.bwWallPaper.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwWallPaper_RunWorkerCompleted);
			// 
			// btnChangeNow
			// 
			this.btnChangeNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnChangeNow.Location = new System.Drawing.Point(93, 303);
			this.btnChangeNow.Name = "btnChangeNow";
			this.btnChangeNow.Size = new System.Drawing.Size(81, 23);
			this.btnChangeNow.TabIndex = 15;
			this.btnChangeNow.Text = "&Change Now";
			this.btnChangeNow.UseVisualStyleBackColor = true;
			this.btnChangeNow.Click += new System.EventHandler(this.btnChangeNow_Click);
			// 
			// lblWorking
			// 
			this.lblWorking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblWorking.AutoSize = true;
			this.lblWorking.Location = new System.Drawing.Point(352, 308);
			this.lblWorking.Name = "lblWorking";
			this.lblWorking.Size = new System.Drawing.Size(56, 13);
			this.lblWorking.TabIndex = 18;
			this.lblWorking.Text = "Working...";
			this.lblWorking.Visible = false;
			// 
			// cmbSSIImageSize
			// 
			this.cmbSSIImageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbSSIImageSize.FormattingEnabled = true;
			this.cmbSSIImageSize.Location = new System.Drawing.Point(136, 63);
			this.cmbSSIImageSize.Name = "cmbSSIImageSize";
			this.cmbSSIImageSize.Size = new System.Drawing.Size(99, 21);
			this.cmbSSIImageSize.TabIndex = 5;
			// 
			// lblSSIResizeMethod
			// 
			this.lblSSIResizeMethod.AutoSize = true;
			this.lblSSIResizeMethod.Location = new System.Drawing.Point(42, 66);
			this.lblSSIResizeMethod.Name = "lblSSIResizeMethod";
			this.lblSSIResizeMethod.Size = new System.Drawing.Size(81, 13);
			this.lblSSIResizeMethod.TabIndex = 4;
			this.lblSSIResizeMethod.Text = "&Resize Method:";
			// 
			// lblSSIFolder
			// 
			this.lblSSIFolder.AutoSize = true;
			this.lblSSIFolder.Location = new System.Drawing.Point(84, 40);
			this.lblSSIFolder.Name = "lblSSIFolder";
			this.lblSSIFolder.Size = new System.Drawing.Size(39, 13);
			this.lblSSIFolder.TabIndex = 1;
			this.lblSSIFolder.Text = "&Folder:";
			// 
			// lblMSIFolder
			// 
			this.lblMSIFolder.AutoSize = true;
			this.lblMSIFolder.Location = new System.Drawing.Point(84, 125);
			this.lblMSIFolder.Name = "lblMSIFolder";
			this.lblMSIFolder.Size = new System.Drawing.Size(39, 13);
			this.lblMSIFolder.TabIndex = 7;
			this.lblMSIFolder.Text = "F&older:";
			// 
			// lblMultiScreenImages
			// 
			this.lblMultiScreenImages.AutoSize = true;
			this.lblMultiScreenImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMultiScreenImages.Location = new System.Drawing.Point(17, 94);
			this.lblMultiScreenImages.Name = "lblMultiScreenImages";
			this.lblMultiScreenImages.Size = new System.Drawing.Size(122, 13);
			this.lblMultiScreenImages.TabIndex = 6;
			this.lblMultiScreenImages.Text = "Multi-Screen Images";
			// 
			// txtMSIImageFolderPath
			// 
			this.txtMSIImageFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtMSIImageFolderPath.Location = new System.Drawing.Point(136, 122);
			this.txtMSIImageFolderPath.Name = "txtMSIImageFolderPath";
			this.txtMSIImageFolderPath.ReadOnly = true;
			this.txtMSIImageFolderPath.Size = new System.Drawing.Size(355, 20);
			this.txtMSIImageFolderPath.TabIndex = 8;
			// 
			// lblMSIResizeMethod
			// 
			this.lblMSIResizeMethod.AutoSize = true;
			this.lblMSIResizeMethod.Location = new System.Drawing.Point(42, 151);
			this.lblMSIResizeMethod.Name = "lblMSIResizeMethod";
			this.lblMSIResizeMethod.Size = new System.Drawing.Size(81, 13);
			this.lblMSIResizeMethod.TabIndex = 10;
			this.lblMSIResizeMethod.Text = "R&esize Method:";
			// 
			// cmbMSIImageSize
			// 
			this.cmbMSIImageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbMSIImageSize.FormattingEnabled = true;
			this.cmbMSIImageSize.Location = new System.Drawing.Point(136, 148);
			this.cmbMSIImageSize.Name = "cmbMSIImageSize";
			this.cmbMSIImageSize.Size = new System.Drawing.Size(99, 21);
			this.cmbMSIImageSize.TabIndex = 11;
			// 
			// btnMSIImageFolderChooser
			// 
			this.btnMSIImageFolderChooser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMSIImageFolderChooser.Location = new System.Drawing.Point(497, 121);
			this.btnMSIImageFolderChooser.Name = "btnMSIImageFolderChooser";
			this.btnMSIImageFolderChooser.Size = new System.Drawing.Size(38, 20);
			this.btnMSIImageFolderChooser.TabIndex = 9;
			this.btnMSIImageFolderChooser.Text = "...";
			this.btnMSIImageFolderChooser.UseVisualStyleBackColor = true;
			this.btnMSIImageFolderChooser.Click += new System.EventHandler(this.btnMSIImageFolderChooser_Click);
			// 
			// lblImagesUsed
			// 
			this.lblImagesUsed.AutoSize = true;
			this.lblImagesUsed.Location = new System.Drawing.Point(51, 199);
			this.lblImagesUsed.Name = "lblImagesUsed";
			this.lblImagesUsed.Size = new System.Drawing.Size(72, 13);
			this.lblImagesUsed.TabIndex = 12;
			this.lblImagesUsed.Text = "Images Used:";
			// 
			// lbImagesUsed
			// 
			this.lbImagesUsed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbImagesUsed.BackColor = System.Drawing.SystemColors.Control;
			this.lbImagesUsed.ContextMenuStrip = this.cmsImagesUsed;
			this.lbImagesUsed.FormattingEnabled = true;
			this.lbImagesUsed.IntegralHeight = false;
			this.lbImagesUsed.Location = new System.Drawing.Point(136, 199);
			this.lbImagesUsed.Name = "lbImagesUsed";
			this.lbImagesUsed.Size = new System.Drawing.Size(399, 91);
			this.lbImagesUsed.TabIndex = 13;
			this.lbImagesUsed.DoubleClick += new System.EventHandler(this.lbImagesUsed_DoubleClick);
			this.lbImagesUsed.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lbImagesUsed_KeyUp);
			this.lbImagesUsed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbImagesUsed_MouseDown);
			// 
			// cmsImagesUsed
			// 
			this.cmsImagesUsed.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyFileNameToolStripMenuItem,
            this.copyPathToolStripMenuItem,
            this.openImageToolStripMenuItem,
            this.openFolderToolStripMenuItem});
			this.cmsImagesUsed.Name = "cmsImagesUsed";
			this.cmsImagesUsed.Size = new System.Drawing.Size(166, 92);
			this.cmsImagesUsed.Opening += new System.ComponentModel.CancelEventHandler(this.cmsImagesUsed_Opening);
			// 
			// copyFileNameToolStripMenuItem
			// 
			this.copyFileNameToolStripMenuItem.Name = "copyFileNameToolStripMenuItem";
			this.copyFileNameToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.copyFileNameToolStripMenuItem.Text = "&Copy File Path";
			this.copyFileNameToolStripMenuItem.Click += new System.EventHandler(this.copyFileNameToolStripMenuItem_Click);
			// 
			// copyPathToolStripMenuItem
			// 
			this.copyPathToolStripMenuItem.Name = "copyPathToolStripMenuItem";
			this.copyPathToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.copyPathToolStripMenuItem.Text = "Copy Folder &Path";
			this.copyPathToolStripMenuItem.Click += new System.EventHandler(this.copyPathToolStripMenuItem_Click);
			// 
			// openImageToolStripMenuItem
			// 
			this.openImageToolStripMenuItem.Name = "openImageToolStripMenuItem";
			this.openImageToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.openImageToolStripMenuItem.Text = "&Open Image";
			this.openImageToolStripMenuItem.Click += new System.EventHandler(this.openImageToolStripMenuItem_Click);
			// 
			// openFolderToolStripMenuItem
			// 
			this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
			this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.openFolderToolStripMenuItem.Text = "Open &Folder";
			this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.btnExit;
			this.ClientSize = new System.Drawing.Size(547, 338);
			this.Controls.Add(this.lbImagesUsed);
			this.Controls.Add(this.lblWorking);
			this.Controls.Add(this.btnSSIImageFolderChooser);
			this.Controls.Add(this.btnMSIImageFolderChooser);
			this.Controls.Add(this.cmbSSIImageSize);
			this.Controls.Add(this.cmbMSIImageSize);
			this.Controls.Add(this.lblSSIResizeMethod);
			this.Controls.Add(this.lblImagesUsed);
			this.Controls.Add(this.lblMSIResizeMethod);
			this.Controls.Add(this.btnChangeNow);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.txtSSIImageFolderPath);
			this.Controls.Add(this.txtMSIImageFolderPath);
			this.Controls.Add(this.numRotateMinutes);
			this.Controls.Add(this.lblRotateTime);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.lblSingleScreenImages);
			this.Controls.Add(this.lblMultiScreenImages);
			this.Controls.Add(this.lblMSIFolder);
			this.Controls.Add(this.lblSSIFolder);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Hydra Paper";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.cmsTray.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numRotateMinutes)).EndInit();
			this.cmsImagesUsed.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Label lblSingleScreenImages;
		private System.Windows.Forms.TextBox txtSSIImageFolderPath;
		private System.Windows.Forms.Label lblRotateTime;
		private System.Windows.Forms.NotifyIcon niTray;
		private System.Windows.Forms.NumericUpDown numRotateMinutes;
		private System.Windows.Forms.Button btnSSIImageFolderChooser;
		private System.ComponentModel.BackgroundWorker bwWallPaper;
		private System.Windows.Forms.Button btnChangeNow;
		private System.Windows.Forms.Label lblWorking;
		private System.Windows.Forms.ContextMenuStrip cmsTray;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ComboBox cmbSSIImageSize;
		private System.Windows.Forms.Label lblSSIResizeMethod;
		private System.Windows.Forms.Label lblSSIFolder;
		private System.Windows.Forms.Label lblMSIFolder;
		private System.Windows.Forms.Label lblMultiScreenImages;
		private System.Windows.Forms.TextBox txtMSIImageFolderPath;
		private System.Windows.Forms.Label lblMSIResizeMethod;
		private System.Windows.Forms.ComboBox cmbMSIImageSize;
		private System.Windows.Forms.Button btnMSIImageFolderChooser;
		private System.Windows.Forms.ToolStripMenuItem blankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startStopStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem statusToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
		private System.Windows.Forms.Label lblImagesUsed;
		private System.Windows.Forms.ListBox lbImagesUsed;
		private System.Windows.Forms.ContextMenuStrip cmsImagesUsed;
		private System.Windows.Forms.ToolStripMenuItem copyFileNameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyPathToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openImageToolStripMenuItem;
	}
}

