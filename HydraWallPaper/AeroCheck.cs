using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HydraPaper
{
	public static class AeroCheck
	{
		[DllImport("dwmapi.dll", PreserveSig = false)]
		private static extern bool DwmIsCompositionEnabled();

		public static bool AeroEnabled {
			get
			{
				// Check to see if composition is Enabled
				if (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
