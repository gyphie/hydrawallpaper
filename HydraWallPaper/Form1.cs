using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Common;
using HydraPaper.Properties;
using Ookii.Dialogs;
using System.Diagnostics;
using System.Reflection;

namespace HydraPaper
{
	public partial class frmMain : Form
	{
		private VistaFolderBrowserDialog dlgVistaFolder;
		private JobArguments jobArguments;
		private System.Timers.Timer timDisplaySettingsChanged;
		private System.Windows.Forms.Timer timCMSStatus;
		private System.Windows.Forms.Timer timTraySingleClick;
		private HPTimer timRotate;
		private ApplicationStates AppState = ApplicationStates.Configuration;
		private AsyncOperation asyncOp;
		private bool previousAeroState = false;

		public frmMain()
		{
			InitializeComponent();
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
			Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);

			this.dlgVistaFolder = new VistaFolderBrowserDialog();
			this.jobArguments = new JobArguments();
			this.previousAeroState = AeroCheck.AeroEnabled;

			Program.DebugMessage("Starting with Aero: " + (this.previousAeroState ? "true" : "false"));

			this.SetupControls();
			this.LoadSettings();

			this.btnStart_Click(null, null);

		}

		private void SetupControls()
		{
			this.Icon = Icons.HydraIconForm;

			// Manually build the item list to match the enumeration
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

			this.timRotate = new HPTimer(60000);    // Timer with a default of 1 minute
			this.timRotate.Elapsed = timRotate_Elapsed;

			this.timDisplaySettingsChanged = new System.Timers.Timer(3000);
			this.timDisplaySettingsChanged.Elapsed += displayTimer_Elapsed;
			this.timDisplaySettingsChanged.AutoReset = false;

			this.timCMSStatus = new System.Windows.Forms.Timer();
			this.timCMSStatus.Interval = 1000;
			this.timCMSStatus.Tick += timCMSStatus_Elapsed;

			this.timTraySingleClick = new System.Windows.Forms.Timer();
			this.timTraySingleClick.Interval = SystemInformation.DoubleClickTime + 100;
			this.timTraySingleClick.Tick += timTraySingleClick_Elapsed;


			this.asyncOp = AsyncOperationManager.CreateOperation(null);

			this.Text += " v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
		}

		bool formCanBeVisible = false;

		protected override void SetVisibleCore(bool value)
		{
			if (!formCanBeVisible) value = false;
			base.SetVisibleCore(value);
		}

		public void LoadSettings()
		{
			// Check validity of configuration and recover if necessary: http://www.codeproject.com/Articles/30216/Handling-Corrupt-user-config-Settings
			try
			{
				var test = Settings.Default.SingleScreenImagePath;
			}
			catch (System.Configuration.ConfigurationErrorsException ex)
			{
				string filename = ((System.Configuration.ConfigurationErrorsException)ex.InnerException).Filename;
				File.Delete(filename);
				Settings.Default.Reload();
			}

			if (Settings.Default.UpgradeSettings)
			{
				Settings.Default.Upgrade();
				Settings.Default.Reload();
				Settings.Default.UpgradeSettings = false;
				Settings.Default.Save();
			}

			this.txtSSIImageFolderPath.Text = Settings.Default.SingleScreenImagePath;
			this.txtMSIImageFolderPath.Text = Settings.Default.MultiScreenImagePath;
			this.cmbSSIImageSize.SelectedValue = (int)Settings.Default.SingleScreenImageBehavior;
			this.cmbMSIImageSize.SelectedValue = (int)Settings.Default.MultiScreenImageBehavior;

			this.numRotateMinutes.Value = Math.Min(this.numRotateMinutes.Maximum, Math.Max(this.numRotateMinutes.Minimum, Settings.Default.ChangeInterval));

			this.jobArguments.Settings = Settings.Default;
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.ConfigureForStatus(StateCommands.Exit);
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
				this.jobArguments.RefreshFilesList = true;
			}
		}

		private void btnChangeNow_Click(object sender, EventArgs e)
		{
			this.ConfigureForStatus(StateCommands.ShowNext);
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
			Program.DebugMessage("Detected Display Setting Change. Starting timer for delayed rotation");
			if (this.timDisplaySettingsChanged.Enabled) this.timDisplaySettingsChanged.Stop();

			this.timDisplaySettingsChanged.Start(); // We do this on a timer so other events can have a chance to cancel the timer and prevent the display change from updating the wallpaper
		}

		/// <summary>
		/// Runs on a separate thread so we need to sync with the form thread
		/// </summary>
		private void timRotate_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.asyncOp.Post((state) => {
				Program.DebugMessage("Rotation timer elapsed.");
				this.UpdateWallPaper();
			}, null);
		}

		/// <summary>
		/// Runs on a separate thread so we need to sync with the form thread
		/// </summary>
		private void displayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.asyncOp.Post((state) =>
			{
				// Detect if the change was to a low graphics mode which indicates a video game or remote session (VNC) and we should suspend the timer
				if (this.previousAeroState != AeroCheck.AeroEnabled)
				{
					this.previousAeroState = AeroCheck.AeroEnabled;

					if (!this.previousAeroState)
					{
						Program.DebugMessage("Detected low graphics change. Moved to Remote state");
						this.ConfigureForStatus(StateCommands.Remote);
						return;
					}
				}

				// Aero didn't change (some other display property changed) or aero was enabled so go ahead and rotate the wallpaper so it is updated to match the new 
				// display settings
				Program.DebugMessage("State Update triggered by graphics change.");
				this.ConfigureForStatus(StateCommands.DisplayChange);
			}, null);
		}


		private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
		{
			switch (e.Reason)
			{
				case Microsoft.Win32.SessionSwitchReason.SessionLock:
				case Microsoft.Win32.SessionSwitchReason.RemoteConnect:
				case Microsoft.Win32.SessionSwitchReason.SessionRemoteControl:
					Program.DebugMessage($"Detected {(SystemInformation.TerminalServerSession ? "remote session" : "workstation lock")}. Terminal: {SystemInformation.TerminalServerSession}");
					if (SystemInformation.TerminalServerSession)
					{
						this.ConfigureForStatus(StateCommands.Remote);
					}
					else {
						this.ConfigureForStatus(StateCommands.Lock);
					}
					break;
				case Microsoft.Win32.SessionSwitchReason.ConsoleConnect:    // triggers after a remote desktop session
				case Microsoft.Win32.SessionSwitchReason.SessionUnlock:
					if (SystemInformation.TerminalServerSession)
					{
						Program.DebugMessage($"Detected remote end. Terminal: {SystemInformation.TerminalServerSession}");
						this.ConfigureForStatus(StateCommands.Remote);
					}
					else
					{
						Program.DebugMessage($"Detected unlock. Terminal: {SystemInformation.TerminalServerSession}");
						this.ConfigureForStatus(StateCommands.Pause);
						this.ConfigureForStatus(StateCommands.StartRotation);
					}
					break;
				default:
					break;
			}
		}


		private void bwWallPaper_DoWork(object sender, DoWorkEventArgs e)
		{
			JobArguments settings = e.Argument as JobArguments;

			WallPaperBuilder.BuildWallPaper(settings);
		}

		private void bwWallPaper_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.lblWorking.Visible = false;
			this.txtImagesUsed.Text = string.Join(", ", this.jobArguments.FileNamesUsed);
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
			this.jobArguments.RefreshFilesList = true;

			this.ConfigureForStatus(StateCommands.StartRotation);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ConfigureForStatus(StateCommands.Configure);
		}

		private void frmMain_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.formCanBeVisible = false;
				this.Hide();
			}
		}

		private void niTray_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.timTraySingleClick.Stop(); // Prevent the two single clicks from triggering wallpaper rotations
				this.openToolStripMenuItem_Click(null, null);
			}
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.SaveSettings();
		}

		private void startStopStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.AppState == ApplicationStates.RotateWallpaper)
			{
				this.ConfigureForStatus(StateCommands.Pause);
			}
			else
			{
				this.ConfigureForStatus(StateCommands.StartRotation);
			}
		}

		private void blankToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ConfigureForStatus(StateCommands.Blank);
		}

		private void cmsTray_Opening(object sender, CancelEventArgs e)
		{
			this.statusToolStripMenuItem.Text = this.GetStatusMessage();
		}

		private string GetStatusMessage()
		{
			string statusText = "Unknown";

			switch (this.AppState)
			{
				case ApplicationStates.RotateWallpaper:
					var timeRemaining = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(this.timRotate.MillisecondsRemaining));
					statusText = $"Next wallpaper in {timeRemaining.Minutes}m {timeRemaining.Seconds}s";
					break;
				case ApplicationStates.Remote:
					statusText = "Remote";
					break;
				case ApplicationStates.Locked:
					statusText = "Locked";
					break;
				case ApplicationStates.Paused:
					statusText = "Stopped";
					break;
				case ApplicationStates.Blanked:
					statusText = "Blanked";
					break;
				case ApplicationStates.Configuration:
					statusText = "Stopped";
					break;
				default:
					break;
			}

			return statusText;
		}

		/// <summary>
		/// Restarts the rotation, also causes the wallpaper to be rotated immediately
		/// </summary>
		private void nextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ConfigureForStatus(StateCommands.ShowNext);
		}

		/// <summary>
		/// A tray icon single left click on the tray icon starts a timer to rotate the wallpaper. Setting a timer allows a double click to prevent the rotation and open the app instead.
		/// </summary>
		private void niTray_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				timTraySingleClick.Start();
			}
		}

		private void cmsTray_VisibleChanged(object sender, EventArgs e)
		{
			var cms = sender as ContextMenuStrip;
			if (cms.Visible)
			{
				this.timCMSStatus.Start();
			}
			else
			{
				this.timCMSStatus.Stop();
			}
		}

		void timCMSStatus_Elapsed(object sender, System.EventArgs e)
		{
			this.cmsTray_Opening(null, null);
		}

		/// <summary>
		/// A Single click on the tray (after a delay to watch for double clicks) makes the wallpaper rotate
		/// </summary>
		void timTraySingleClick_Elapsed(object sender, System.EventArgs e)
		{
			timTraySingleClick.Stop();
			nextToolStripMenuItem_Click(null, null);
		}

		private void ConfigureForStatus(StateCommands command)
		{
			if (command == StateCommands.Blank)
			{
				Program.DebugMessage("Command: Blank");

				if (this.AppState != ApplicationStates.Remote)
				{
					this.AppState = ApplicationStates.Blanked; // If in remote state don't change, but still blank the screen
					this.SaveSettings();

					this.niTray.Icon = Icons.HydraBlankIcon.ToIcon();
					this.niTray.Text = this.GetStatusMessage();
				}

				this.startStopStripMenuItem.Text = "Start";
				this.nextToolStripMenuItem.Enabled = false;

				this.timDisplaySettingsChanged.Stop();
				this.timTraySingleClick.Stop();
				this.timRotate.Stop();
				CSSetDesktopWallpaper.SolidColor.SetColor(Color.Black);
			}
			else if (command == StateCommands.Lock)
			{
				Program.DebugMessage("Command: Lock");

				this.AppState = ApplicationStates.Locked;
				this.SaveSettings();

				this.startStopStripMenuItem.Enabled = true;
				this.nextToolStripMenuItem.Enabled = true;

				this.timDisplaySettingsChanged.Stop();
				this.timTraySingleClick.Stop();
				this.timRotate.Stop();
				this.niTray.Icon = Icons.HydraBlankIcon.ToIcon();
				this.niTray.Text = this.GetStatusMessage();
				CSSetDesktopWallpaper.SolidColor.SetColor(Color.Black);
			}
			else if (command == StateCommands.Pause)
			{
				Program.DebugMessage("Command: Pause");

				this.AppState = ApplicationStates.Paused;
				this.SaveSettings();

				this.startStopStripMenuItem.Text = "Start";

				this.timDisplaySettingsChanged.Stop();
				this.timRotate.Stop();
				this.timTraySingleClick.Stop();
				this.niTray.Icon = Icons.HydraStoppedIcon;
				this.niTray.Text = this.GetStatusMessage();
			}
			else if (command == StateCommands.Remote)
			{
				Program.DebugMessage("Command: Remote");

				this.AppState = ApplicationStates.Remote;
				this.SaveSettings();

				this.startStopStripMenuItem.Text = "Start";
				this.startStopStripMenuItem.Enabled = false;
				this.nextToolStripMenuItem.Enabled = false;

				this.timDisplaySettingsChanged.Stop();
				this.timTraySingleClick.Stop();
				this.timRotate.Stop();
				this.niTray.Icon = Icons.HydraRemoteIcon.ToIcon();
				this.niTray.Text = this.GetStatusMessage();
				CSSetDesktopWallpaper.SolidColor.SetColor(Color.Black);
			}
			else if (command == StateCommands.ShowNext)
			{
				Program.DebugMessage("Command: ShowNext");

				if (this.AppState == ApplicationStates.RotateWallpaper || this.AppState == ApplicationStates.Paused || this.AppState == ApplicationStates.Configuration)
				{
					this.timRotate.Stop();
					this.UpdateWallPaper();

					if (this.AppState == ApplicationStates.RotateWallpaper)
					{
						this.timRotate.Start();
					}
				}
			}
			else if (command == StateCommands.DisplayChange)
			{
				Program.DebugMessage("Command: DisplayChange");
				if (this.AppState == ApplicationStates.Remote && !System.Windows.Forms.SystemInformation.TerminalServerSession)
				{
					this.AppState = ApplicationStates.Paused;
					command = StateCommands.StartRotation;
				}
				else
				{
					if (this.AppState == ApplicationStates.Remote || this.AppState == ApplicationStates.Blanked || this.AppState == ApplicationStates.Locked)
					{
						return;
					}

					if (this.AppState == ApplicationStates.Paused)
					{
						// They don't want to rotate but now the background is going to be messed up so just blank.
						CSSetDesktopWallpaper.SolidColor.SetColor(Color.Black);
					}
					else
					{
						this.timRotate.Stop();
						this.timTraySingleClick.Stop();
						this.UpdateWallPaper();

						if (this.AppState == ApplicationStates.RotateWallpaper)
						{
							this.timRotate.Start();
						}
					}
				}
			}
			else if (command == StateCommands.Configure)
			{
				Program.DebugMessage("Command: Configure");
				this.AppState = ApplicationStates.Configuration;

				this.timRotate.Stop();
				this.timDisplaySettingsChanged.Stop();
				this.timTraySingleClick.Stop();
				this.niTray.Icon = Icons.HydraStoppedIcon;
				this.niTray.Text = this.GetStatusMessage();

				this.formCanBeVisible = true;
				this.Show();
				this.WindowState = FormWindowState.Normal;
			}
			else if (command == StateCommands.Exit)
			{
				try
				{
					Program.DebugMessage("Command: Exit");

					this.AppState = ApplicationStates.Paused;

					this.startStopStripMenuItem.Text = "Start";

					this.timDisplaySettingsChanged.Stop();
					this.timRotate.Stop();
					this.timTraySingleClick.Stop();
					this.niTray.Icon = Icons.HydraStoppedIcon;
					this.niTray.Text = this.GetStatusMessage();

					this.SaveSettings();
				}
				catch { }

				Application.Exit();
				return;
			}


			// The lack of ELSE IF is to allow another command to change to Start Rotation
			if (command == StateCommands.StartRotation)
			{
				Program.DebugMessage("Command: StartRotation");

				if (this.AppState == ApplicationStates.Remote || this.AppState == ApplicationStates.Locked)
				{
					return;
				}

				this.AppState = ApplicationStates.RotateWallpaper;
				this.SaveSettings();

				this.nextToolStripMenuItem.Enabled = true;
				this.startStopStripMenuItem.Enabled = true;
				this.startStopStripMenuItem.Text = "Stop";

				this.timRotate.Stop();
				this.timDisplaySettingsChanged.Stop();
				this.timTraySingleClick.Stop();
				this.niTray.Icon = Icons.HydraIcon;
				this.niTray.Text = "HydraPaper";

				this.UpdateWallPaper();

				this.timRotate.Interval = Math.Max(Convert.ToInt32(this.numRotateMinutes.Value * 60000), 60000);
				this.timRotate.Start();
			}
		}
	}

	public static class Ext
	{
		public static Icon ToIcon(this Bitmap bitMap)
		{
			return Icon.FromHandle(bitMap.GetHicon());
		}
	}

}
