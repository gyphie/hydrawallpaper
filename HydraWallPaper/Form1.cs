using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Common;
using HydraPaper.Properties;
using Ookii.Dialogs;

namespace HydraPaper
{
	public partial class frmMain : Form
	{
		private VistaFolderBrowserDialog dlgVistaFolder;
		private JobArguments jobArguments;
		private System.Timers.Timer timDisplaySettingsChanged;
		private System.Timers.Timer timRotate;
		private ApplicationStatus AppStatus = ApplicationStatus.None;
		private AsyncOperation asyncOp;

		public frmMain()
		{
			InitializeComponent();
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
			Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);

			this.dlgVistaFolder = new VistaFolderBrowserDialog();
			this.jobArguments = new JobArguments();
			this.AppStatus = System.Windows.Forms.SystemInformation.TerminalServerSession ? ApplicationStatus.Remote : ApplicationStatus.Desktop;

			this.SetupControls();
			this.LoadSettings();

			this.btnStart_Click(null, null);

		}

		private void SetupControls()
		{
			// Manually build the item list to match the enum
			this.cmbSSIImageSize.Items.Clear();
			this.cmbMSIImageSize.Items.Clear();

			this.cmbSSIImageSize.ValueMember = this.cmbMSIImageSize.ValueMember = "ID";
			this.cmbSSIImageSize.DisplayMember = this.cmbMSIImageSize.DisplayMember = "Text";
	
			List<object> behaviors = new List<object>();
			behaviors.Add(new { ID = (int)ImageBehavior.TouchInside, Text = ImageBehavior.TouchInside });
			behaviors.Add(new { ID = (int)ImageBehavior.TouchOutside, Text = ImageBehavior.TouchOutside });
			behaviors.Add(new { ID = (int)ImageBehavior.Stretch, Text = ImageBehavior.Stretch });
			behaviors.Add(new { ID = (int)ImageBehavior.Center, Text = ImageBehavior.Center });
			
			this.cmbSSIImageSize.DataSource = behaviors;

			behaviors = new List<object>();
			behaviors.Add(new { ID = (int)ImageBehavior.TouchInside, Text = ImageBehavior.TouchInside });
			behaviors.Add(new { ID = (int)ImageBehavior.TouchOutside, Text = ImageBehavior.TouchOutside });
			behaviors.Add(new { ID = (int)ImageBehavior.Stretch, Text = ImageBehavior.Stretch });
			behaviors.Add(new { ID = (int)ImageBehavior.Center, Text = ImageBehavior.Center });
	
			this.cmbMSIImageSize.DataSource = behaviors;

			this.timRotate = new System.Timers.Timer();
			this.timRotate.Elapsed += timRotate_Elapsed;
			this.timRotate.AutoReset = true;

			this.timDisplaySettingsChanged = new System.Timers.Timer(3000);
			this.timDisplaySettingsChanged.Elapsed += displayTimer_Elapsed;
			this.timDisplaySettingsChanged.AutoReset = false;

			this.asyncOp = AsyncOperationManager.CreateOperation(null);
		}

		bool formCanBeVisible = false;

		protected override void SetVisibleCore(bool value)
		{
			if (!formCanBeVisible) value = false;
			base.SetVisibleCore(value);
		}

		public void LoadSettings()
		{
			this.txtSSIImageFolderPath.Text = Settings.Default.SingleScreenImagePath;
			this.txtMSIImageFolderPath.Text = Settings.Default.MultiScreenImagePath;
			this.cmbSSIImageSize.SelectedValue = (int)Settings.Default.SingleScreenImageBehavior;
			this.cmbMSIImageSize.SelectedValue = (int)Settings.Default.MultiScreenImageBehavior;

			this.numRotateMinutes.Value = Math.Min(this.numRotateMinutes.Maximum, Math.Max(this.numRotateMinutes.Minimum, Settings.Default.ChangeInterval));

			this.jobArguments.Settings = Settings.Default;
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.timRotate.Stop();
			this.CleanlyExitApp();
		}

		private void SaveSettings()
		{
			Settings.Default.SingleScreenImagePath = this.txtSSIImageFolderPath.Text.Trim();
			Settings.Default.MultiScreenImagePath = this.txtMSIImageFolderPath.Text.Trim();

			Settings.Default.SingleScreenImageBehavior = (ImageBehavior)this.cmbSSIImageSize.SelectedValue;
			Settings.Default.MultiScreenImageBehavior = (ImageBehavior)this.cmbMSIImageSize.SelectedValue;

	
			Settings.Default.ChangeInterval = Convert.ToInt32(this.numRotateMinutes.Value);

			Settings.Default.Save();
		}

		private void btnSSIImageFolderChooser_Click(object sender, EventArgs e)
		{
			this.ImageFolderChooser(this.txtSSIImageFolderPath);
		}
		private void btnMSIImageFolderChooser_Click(object sender, EventArgs e)
		{
			this.ImageFolderChooser(this.txtMSIImageFolderPath);
		}


		private void ImageFolderChooser(TextBox targetTextBox)
		{
			this.dlgVistaFolder.Description = "Wallpapers";
			this.dlgVistaFolder.ShowNewFolderButton = true;
			this.dlgVistaFolder.RootFolder = Environment.SpecialFolder.MyPictures;
			string imageFolder = targetTextBox.Text.Trim();

			if (!string.IsNullOrEmpty(imageFolder) && Directory.Exists(imageFolder))
			{
				this.dlgVistaFolder.SelectedPath = imageFolder;
			}
			else
			{
				this.dlgVistaFolder.SelectedPath = Environment.GetFolderPath(this.dlgVistaFolder.RootFolder);
			}

			this.dlgVistaFolder.SelectedPath = this.dlgVistaFolder.SelectedPath.TrimEnd('\\') + "\\";

			if (this.dlgVistaFolder.ShowDialog() == DialogResult.OK)
			{
				targetTextBox.Text = this.dlgVistaFolder.SelectedPath;
			}
		}

		private void btnChangeNow_Click(object sender, EventArgs e)
		{
			this.UpdateWallPaper();
		}

		public void UpdateWallPaper()
		{
			//Program.DebugMessage("Triggering Wallpaper update");
			this.timDisplaySettingsChanged.Stop();
			this.SaveSettings();
			
			if (!this.bwWallPaper.IsBusy)
			{
				this.bwWallPaper.RunWorkerAsync(this.jobArguments);
				this.lblWorking.Visible = true;
			}
		}


		internal void CleanEvents()
		{
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= new EventHandler(SystemEvents_DisplaySettingsChanged);
			Microsoft.Win32.SystemEvents.SessionSwitch -= new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
		}

		private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			if (this.AppStatus == ApplicationStatus.Desktop)
			{
				Program.DebugMessage("Detected Display Setting Change. Starting timer for delayed rotation");
				if (this.timDisplaySettingsChanged.Enabled) this.timDisplaySettingsChanged.Stop();

				this.timDisplaySettingsChanged.Start();	// We do this on a timer so other events can have a chance to cancel the timer and prevent the display change from updating the wallpaper
			}
		}

		/// <summary>
		/// Runs on a separate thread so we need to sync with the form thread
		/// </summary>
		private void timRotate_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.asyncOp.Post((state) => { this.UpdateWallPaper(); }, null);
		}

		/// <summary>
		/// Runs on a separate thread so we need to sync with the form thread
		/// </summary>
		private void displayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.asyncOp.Post((state) =>
			{
				this.timRotate.Stop();
				this.UpdateWallPaper();
				this.timRotate.Start();
				Program.DebugMessage("Started rotate timer");
			}, null);
		}


		private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
		{
			switch (e.Reason)
			{
				case Microsoft.Win32.SessionSwitchReason.RemoteConnect:
				case Microsoft.Win32.SessionSwitchReason.SessionRemoteControl:
				case Microsoft.Win32.SessionSwitchReason.SessionLock:
					Program.DebugMessage("Detected lock or remote session. Stopping rotation.");
					this.AppStatus = System.Windows.Forms.SystemInformation.TerminalServerSession ? ApplicationStatus.Remote : ApplicationStatus.Locked;
					this.timDisplaySettingsChanged.Stop();
					this.timRotate.Stop();
					this.SaveSettings();
					this.blankToolStripMenuItem_Click(null, null);
					break;
				case Microsoft.Win32.SessionSwitchReason.ConsoleConnect:	// triggers after a remote desktop session
				case Microsoft.Win32.SessionSwitchReason.SessionUnlock:		// triggers after a workstation is locked (WIN+L)
					Program.DebugMessage("Detected remote end or unlock.");
					this.AppStatus = System.Windows.Forms.SystemInformation.TerminalServerSession ? ApplicationStatus.Remote : ApplicationStatus.Desktop;
					if (this.WindowState == FormWindowState.Minimized)
					{
						this.timRotate.Stop();
						this.UpdateWallPaper();
						this.timRotate.Start();
						Program.DebugMessage("Started rotate timer");
					}
					break;
				default:
					break;
			}
		}


		private void bwWallPaper_DoWork(object sender, DoWorkEventArgs e)
		{
			WallPaperBuilder.BuildWallPaper(e.Argument as JobArguments);
		}

		private void bwWallPaper_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.lblWorking.Visible = false;
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.timRotate.Stop();
			this.SaveSettings();
			this.WindowState = FormWindowState.Minimized;
			this.UpdateWallPaper();
			this.timRotate.Start();
			Program.DebugMessage("Started rotate timer");
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.timRotate.Stop();
			this.formCanBeVisible = true;
			this.Show();
			this.WindowState = FormWindowState.Normal;
			Program.DebugMessage("Stopped rotate timer");
		}

		private void frmMain_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.formCanBeVisible = false;
				this.Hide();
			}
		}

		private void numRotateMinutes_ValueChanged(object sender, EventArgs e)
		{
			this.timRotate.Interval = Convert.ToInt32(this.numRotateMinutes.Value * 60000);
		}

		private void niTray_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.openToolStripMenuItem_Click(null, null);
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.SaveSettings();
		}

		private void CleanlyExitApp()
		{
			this.SaveSettings();
			Application.Exit();
		}

		private void pauseStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.timRotate.Enabled)
			{
				this.timRotate.Stop();
				Program.DebugMessage("Stopped rotate timer");
			}
			else
			{
				this.UpdateWallPaper();
				this.timRotate.Start();
				Program.DebugMessage("Started rotate timer");
			}
		}

		private void blankToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.timRotate.Stop();
			Program.DebugMessage("Stopped rotate timer. Blanked wallpaper.");
			CSSetDesktopWallpaper.SolidColor.SetColor(Color.Black);

		}

		private void cmsTray_Opening(object sender, CancelEventArgs e)
		{
			string statusText = "Unknown";

			switch (this.AppStatus)
			{
				case ApplicationStatus.None:
					statusText = "Unknown";
					break;
				case ApplicationStatus.Desktop:
					statusText = "Desktop";
					break;
				case ApplicationStatus.Remote:
					statusText = "Remote";
					break;
				case ApplicationStatus.Locked:
					statusText = "Locked";
					break;
				default:
					break;
			}

			statusText += this.timRotate.Enabled ? " - Running" : " - Paused";
			
			this.statusToolStripMenuItem.Text = statusText;
			this.pauseStripMenuItem.Text = this.timRotate.Enabled ? "Stop" : "Start";

		}

		private void nextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool wasEnabled = this.timRotate.Enabled;
			this.timRotate.Stop();
			this.UpdateWallPaper();

			if (wasEnabled)	// Reset the timer if it's running
			{
				this.timRotate.Start();
			}

		}
	}
}
