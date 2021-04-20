using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace HydraPaper
{
	static class Program
	{
		private const UInt32 StdOutputHandle = 0xFFFFFFF5;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AttachConsole(int dwProcessId);
		[DllImport("Kernel32.dll", SetLastError = true)]
		static extern Boolean AllocConsole();
		[DllImport("Kernel32.dll", SetLastError = true)]
		static extern Boolean FreeConsole();
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

		private const int MY_CODE_PAGE = 437;
		private const uint GENERIC_WRITE = 0x40000000;
		private const uint FILE_SHARE_WRITE = 0x2;
		private const uint OPEN_EXISTING = 0x3;

		static bool isDebug = false;

		// Mutex code from : http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c/229567
		// All comments regarding edits are from the original code
		[STAThread]
		static void Main(string[] args)
		{
			#if DEBUG
			isDebug = true;
			#endif

			if (args.Length > 0 && args[0].ToLower() == "-debug")
			{
				isDebug = true;
			}

			// get application GUID as defined in AssemblyInfo.cs
			string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();

			// unique id for global mutex - Global prefix means it is global to the machine
			string mutexId = string.Format("Global\\{{{0}}}", appGuid);

			using (var mutex = new Mutex(false, mutexId))
			{
				// edited by Jeremy Wiebe to add example of setting up security for multi-user usage
				// edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
				var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
				var securitySettings = new MutexSecurity();
				securitySettings.AddAccessRule(allowEveryoneRule);
				mutex.SetAccessControl(securitySettings);

				// edited by acidzombie24
				var hasHandle = false;
				try
				{
					try
					{
						// note, you may want to time out here instead of waiting forever
						// edited by acidzombie24
						// mutex.WaitOne(Timeout.Infinite, false);
						hasHandle = mutex.WaitOne(5000, false);
						if (hasHandle == false)
						{
							//throw new TimeoutException("Timeout waiting for exclusive access to mutex");
							MessageBox.Show("Another instance of Hydrapaper is already running.");
							return;
						}
					}
					catch (AbandonedMutexException)
					{
						// Log the fact the mutex was abandoned in another process, it will still get acquired
						hasHandle = true;
					}

#region Perform Your Work Here
					bool alloccedConsole = false;
					try
					{
						if (isDebug)
						{
							alloccedConsole = CreateConsole();
						}
					}
					catch { }

					frmMain mainForm = null;
					try
					{
						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);
						mainForm = new frmMain();
						Application.Run(mainForm);
					}
					catch (Exception ex)
					{
						try
						{
							EventLogMessage(true, "Exception Message: {0}\n\nStack Trace:\n\n{1}", ex.Message, ex.StackTrace);
						}
						catch { }

						throw;
					}
					finally
					{
						if (mainForm != null)
						{
							mainForm.CleanEvents();
						}
						if (alloccedConsole) FreeConsole();
					}
#endregion
				}
				finally
				{
					// edited by acidzombie24, added if statement
					if (hasHandle)
					{
						mutex.ReleaseMutex();
					}
				}
			}
		}

		// https://stackoverflow.com/a/15960495/5583585
		private static bool CreateConsole()
		{
			bool alloccedConsole = false;
			if (!AttachConsole(-1))
			{
				alloccedConsole = true;
				AllocConsole();
			}

			// stdout's handle seems to always be equal to 7
			IntPtr defaultStdout = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0); //https://stackoverflow.com/a/57517624/5583585
			IntPtr currentStdout = GetStdHandle(StdOutputHandle);

			if (currentStdout != defaultStdout)
			{
				// reset stdout
				SetStdHandle(StdOutputHandle, defaultStdout);
			}

			// reopen stdout
			var writer = new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
			Console.SetOut(writer);
			NonBlockingConsole.StartProcessingMessages();

			return alloccedConsole;
		}

		public static void EventLogMessage(bool isError, string message, params string[] parts)
		{
			try
			{
				string eventMessage = null;
				if (parts.Length > 0)
				{
					eventMessage = string.Format(message, parts); ;
				}
				else
				{
					eventMessage = message;
				}

				string src = "HydraPaper";
				if (!EventLog.SourceExists(src)) EventLog.CreateEventSource(src, "Application");
				System.Diagnostics.EventLog.WriteEntry(src, eventMessage, isError ? EventLogEntryType.Error : EventLogEntryType.Information);
			}
			catch { }

		}

		public static void DebugMessage(string message, params string[] parts)
		{
			if (!isDebug) return;

			//EventLogMessage(false, message, parts);

			try
			{
				if (parts.Length > 0)
				{
					NonBlockingConsole.WriteLine(DateTime.Now.ToShortTimeString() + ": " + string.Format(message, parts));
				}
				else
				{
					NonBlockingConsole.WriteLine(DateTime.Now.ToShortTimeString() + ": " + message);
				}
			}
			catch
			{ // Debugging errors should be suppressed
			}
		}

	}
}
