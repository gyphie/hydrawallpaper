using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydraPaper
{
	public class JobArguments
	{
		public JobArguments()
		{
			this.Files = new List<FileProperties>();
			this.Randomizer = new Random();
			this.RefreshFilesList = false;
		}
	
		public List<FileProperties> Files { get; set; }
		public Properties.Settings Settings { get; set; }
		public Random Randomizer { get; set; }
		public bool RefreshFilesList { get; set; }

	}

	public class FileProperties : IComparable
	{
		public FileProperties(string fileName, bool isSingleScreen)
		{
			this.Filename = fileName;
			this.IsSingleScreen = isSingleScreen;
			this.HasBeenShown = false;
		}

		public string Filename { get; set; }
		public bool IsSingleScreen { get; set; }
		public bool HasBeenShown { get; set; }

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj == null || !(obj is FileProperties))
			{
				return -1;
			}
			return this.Filename.CompareTo((obj as FileProperties).Filename);
		}

		public override string ToString()
		{
			return this.Filename;
		}

		#endregion
	}
}
