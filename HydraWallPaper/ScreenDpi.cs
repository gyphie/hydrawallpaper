using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HydraPaper
{
	public static class ScreenDpi
	{
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

		[DllImport("gdi32.dll")]
		static extern int DeleteDC([In]IntPtr hdc);

		[DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		public static Rectangle GetTrueBounds(System.Windows.Forms.Screen screen)
		{
			IntPtr screenDC = CreateDC(screen.DeviceName, "", "", IntPtr.Zero);
			int DESKTOPVERTRES = 117;
			int DESKTOPHORZRES = 118;
			int actualPixelsX = GetDeviceCaps(screenDC, DESKTOPHORZRES);
			int actualPixelsY = GetDeviceCaps(screenDC, DESKTOPVERTRES);
			DeleteDC(screenDC);

			return new Rectangle(screen.Bounds.X, screen.Bounds.Y, actualPixelsX, actualPixelsY);
		}

	}

	// https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
	public enum DpiType
	{
		Effective = 0,
		Angular = 1,
		Raw = 2,
	}
	public enum Flags : uint
	{
		MONITOR_DEFAULTTONEAREST = 2
	}


}
