using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Goheer.EXIF;

namespace HydraPaper
{
	public static class WallPaperBuilder
	{
		public static void BuildWallPaper(JobArguments settings)
		{
			settings.FileNamesUsed.Clear();
			
			if (settings.RefreshFilesList)
			{
				settings.Files.Clear();
				UpdateFileList(settings.Files, settings.Settings.SingleScreenImagePath, true);
				UpdateFileList(settings.Files, settings.Settings.MultiScreenImagePath, false);
				settings.RefreshFilesList = false;
			}

			string wallpaperFileName = string.Empty;

			// Decide if we should do single or multi-screen wall paper
			bool singleScreen = settings.Randomizer.Next(0, 2) == 0;
			if (singleScreen)
			{
				wallpaperFileName = BuildSingleScreenWallPaper(settings);
				if (string.IsNullOrEmpty(wallpaperFileName))
				{
					wallpaperFileName = BuildMultiScreenWallPaper(settings);
				}
			}
			else
			{
				wallpaperFileName = BuildMultiScreenWallPaper(settings);
				if (string.IsNullOrEmpty(wallpaperFileName))
				{
					wallpaperFileName = BuildSingleScreenWallPaper(settings);
				}
			}
	
			SetWallpaper(wallpaperFileName);
		}


		public static void SetWallpaper(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				CSSetDesktopWallpaper.Wallpaper.SetDesktopWallpaper(fileName, CSSetDesktopWallpaper.WallpaperStyle.Tile);
			}
		}

		public static void UpdateFileList(List<FileProperties> files, string path, bool isSingleScreen)
		{
			var stopWatch = System.Diagnostics.Stopwatch.StartNew();

			List<FileInfo> fis = new List<FileInfo>();

			try
			{
				if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
				{
					fis.AddRange(new DirectoryInfo(path).GetFiles("*.jpg", SearchOption.AllDirectories));
					fis.AddRange(new DirectoryInfo(path).GetFiles("*.bmp", SearchOption.AllDirectories));
					fis.AddRange(new DirectoryInfo(path).GetFiles("*.png", SearchOption.AllDirectories));
					fis.AddRange(new DirectoryInfo(path).GetFiles("*.gif", SearchOption.AllDirectories));
					fis.AddRange(new DirectoryInfo(path).GetFiles("*.jpeg", SearchOption.AllDirectories));
				}
			}
			catch (Exception ex) { Program.DebugMessage("Exception in Update FileList: {0}", ex.Message); Program.DebugMessage(ex.StackTrace); }

			foreach (FileInfo fileInfo in fis)
			{
				files.Add(new FileProperties(fileInfo.FullName, isSingleScreen));
			}

			Program.DebugMessage($"Added {fis.Count} {(isSingleScreen ? "single screen" : "multi screen")} files");

			Program.DebugMessage($"Time to load files: {stopWatch.Elapsed.TotalSeconds}");
		}

		public static FileProperties GetFile(List<FileProperties> files, bool isSingleScreen, Random randomizer)
		{
			var fileClass = isSingleScreen ? "single screen" : "multi screen";

			var unshownFiles = files.Where(f => !f.HasBeenShown && f.IsSingleScreen == isSingleScreen);

			// If there were no unshown files reset the list before getting a file
			if (unshownFiles.Count() == 0)
			{
				Program.DebugMessage($"No unshown {fileClass} files found. Resetting the list.");

				foreach (var file in files)
				{
					if (file.IsSingleScreen == isSingleScreen)
					{
						file.HasBeenShown = false;
					}
				}

				unshownFiles = files.Where(f => !f.HasBeenShown && f.IsSingleScreen == isSingleScreen);
			}

			var selectedFile = unshownFiles.ElementAtOrDefault(randomizer.Next(0, unshownFiles.Count()));

			if (selectedFile != null)
			{
				selectedFile.HasBeenShown = true;
				Program.DebugMessage($"Selected file {selectedFile.Filename} for {fileClass}");
			}
			else
			{
				Program.DebugMessage($"No {fileClass} file found");
			}

			return selectedFile;
		}

		public static string BuildMultiScreenWallPaper(JobArguments settings)
		{

			FileProperties file = GetFile(settings.Files, false, settings.Randomizer);
			if (file == null || !File.Exists(file.Filename))
			{
				return string.Empty;
			}

			settings.FileNamesUsed.Add(file.Filename);

			// Determine the size of the wall paper (bounds of all the screens)
			int left = 0, right = 0, top = 0, bottom = 0;
			int wallPaperWidth = 0, wallPaperHeight = 0;

			foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
			{
				Rectangle bounds = screen.Bounds;
				if (left > bounds.X) left = bounds.X;
				if (top > bounds.Y) top = bounds.Y;
				if (right < (bounds.X + bounds.Width)) right = bounds.X + bounds.Width;
				if (bottom < (bounds.Y + bounds.Height)) bottom = bounds.Y + bounds.Height;
			}

			wallPaperWidth = right - left;
			wallPaperHeight = bottom - top;

			int drawOffsetX = Math.Abs(left);
			int drawOffsetY = Math.Abs(top);
			int canvasWidth = right - left + drawOffsetX;
			int canvasHeight = bottom - top + drawOffsetY;

			using (Bitmap img = new Bitmap(canvasWidth, canvasHeight))
			{
				// Create a new graphic of the correct size
				using (Graphics g = Graphics.FromImage(img))
				{
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

					// Paint the background color of each screens bounds
					g.Clear(Color.Black);

					Rectangle bounds = new Rectangle(0, 0, wallPaperWidth, wallPaperHeight);

					using (Brush b = new SolidBrush(Color.Black))
					{
						g.FillRectangle(b, bounds.X, bounds.Y, bounds.Width, bounds.Height);
					}

					try
					{
						using (Image imageFromDisk = Bitmap.FromFile(file.Filename))
						{
							RotateImage(imageFromDisk, file.Filename);

							Size adjustedSize = AdjustImage(imageFromDisk.Size, new Size(bounds.Width, bounds.Height), settings.Settings.MultiScreenImageBehavior);

							if (settings.Settings.MultiScreenImageBehavior == Common.ImageBehavior.Center)
							{
								Rectangle src = new Rectangle();
								src.Width = Math.Min(imageFromDisk.Width, bounds.Width);
								src.Height = Math.Min(imageFromDisk.Height, bounds.Height);
								src.X = (imageFromDisk.Width - src.Width) / 2;
								src.Y = (imageFromDisk.Height - src.Height) / 2;

								// Center the image in the screen region
								Rectangle dst = new Rectangle();
								dst.Width = src.Width;
								dst.Height = src.Height;
								dst.X = bounds.X + ((bounds.Width - dst.Width) / 2);
								dst.Y = bounds.Y + ((bounds.Height - dst.Height) / 2);

								// Paint the image to the graphic
								using (ImageAttributes attr = new ImageAttributes())
								{
									attr.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
									g.DrawImage(imageFromDisk, dst, src.X, src.Y, src.Width, src.Height, GraphicsUnit.Pixel, attr);
								}
							}
							else
							{
								// Center the image in the screen region
								Rectangle dst = new Rectangle();
								dst.X = bounds.X + ((bounds.Width - adjustedSize.Width) / 2);
								dst.Y = bounds.Y + ((bounds.Height - adjustedSize.Height) / 2);
								dst.Width = adjustedSize.Width;
								dst.Height = adjustedSize.Height;

								// Paint the image to the graphic
								// Paint the image to the graphic
								using (ImageAttributes attr = new ImageAttributes())
								{
									attr.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
									g.Clip = new Region(new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height));
									g.DrawImage(imageFromDisk, dst, 0, 0, imageFromDisk.Width, imageFromDisk.Height, GraphicsUnit.Pixel, attr);
									g.ResetClip();
								}

							}
						}
					}
					catch (Exception ex)
					{
						Program.DebugMessage("Exception in BuildMultiScreenWallPaper: {0}", ex.Message);
						Program.DebugMessage(ex.StackTrace);
					}

					//img.Save("c:\\stage1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

					// Move the blocks that are above and to the left of the primary to adjust for the wallpaper wrapping
					if (drawOffsetX > 0)
					{
						g.DrawImage(img,
							new Rectangle(wallPaperWidth, 0, drawOffsetX, wallPaperHeight),
							new Rectangle(0, 0, drawOffsetX, wallPaperHeight),
							GraphicsUnit.Pixel);
					}
					if (drawOffsetY > 0)
					{
						g.DrawImage(img,
							new Rectangle(drawOffsetX, wallPaperHeight, wallPaperWidth, drawOffsetY),
							new Rectangle(drawOffsetX, 0, wallPaperWidth, drawOffsetY), GraphicsUnit.Pixel);
					}

					//img.Save("c:\\stage2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

					// Copy the wallpaper section of the completed canvas
					using (Bitmap imgWallpaper = new Bitmap(wallPaperWidth, wallPaperHeight))
					{
						using (Graphics gWallpaper = Graphics.FromImage(imgWallpaper))
						{
							gWallpaper.DrawImage(img, new Rectangle(0, 0, wallPaperWidth, wallPaperHeight), new Rectangle(drawOffsetX, drawOffsetY, wallPaperWidth, wallPaperHeight), GraphicsUnit.Pixel);

							// Determine the output format
							string wallpaperFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName, "hydrapaper");
							if (CSSetDesktopWallpaper.Wallpaper.SupportJpgAsWallpaper)
							{
								wallpaperFileName += ".jpg";
								imgWallpaper.Save(wallpaperFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
							}
							else
							{
								wallpaperFileName += ".bmp";
								imgWallpaper.Save(wallpaperFileName, System.Drawing.Imaging.ImageFormat.Bmp);
							}

							return wallpaperFileName;

						}
					}
				}
			}
		}

		public static string BuildSingleScreenWallPaper(JobArguments settings)
		{
			// Determine the size of the wall paper (bounds of all the screens)
			int left = 0, right = 0, top = 0, bottom = 0;
			int wallPaperWidth = 0, wallPaperHeight = 0;

			foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
			{
				Rectangle bounds = screen.Bounds;
				if (left > bounds.X) left = bounds.X;
				if (top > bounds.Y) top = bounds.Y;
				if (right < (bounds.X + bounds.Width)) right = bounds.X + bounds.Width;
				if (bottom < (bounds.Y + bounds.Height)) bottom = bounds.Y + bounds.Height;
			}

			wallPaperWidth = right - left;
			wallPaperHeight = bottom - top;

			int drawOffsetX = Math.Abs(left);
			int drawOffsetY = Math.Abs(top);
			int canvasWidth = right - left + drawOffsetX;
			int canvasHeight = bottom - top + drawOffsetY;

			using (Bitmap img = new Bitmap(canvasWidth, canvasHeight))
			{
				// Create a new graphic of the correct size
				using (Graphics g = Graphics.FromImage(img))
				{
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

					// Paint the background color of each screens bounds
					g.Clear(Color.Black);

					foreach (Screen screen in Screen.AllScreens)
					{
						Rectangle bounds = screen.Bounds;

						using (Brush b = new SolidBrush(Color.Black))
						{
							g.FillRectangle(b, bounds.X + drawOffsetX, bounds.Y + drawOffsetY, bounds.Width, bounds.Height);
						}

						// Select an image for each screen.
						FileProperties file = GetFile(settings.Files, true, settings.Randomizer);
						if (file == null)
						{
							// There are no files to make a background with so just return
							return string.Empty;
						}

						settings.FileNamesUsed.Add(file.Filename);

						try
						{
							using (Image imageFromDisk = Bitmap.FromFile(file.Filename))
							{
								RotateImage(imageFromDisk, file.Filename);
								Size adjustedSize = AdjustImage(imageFromDisk.Size, new Size(bounds.Width, bounds.Height), settings.Settings.SingleScreenImageBehavior);

								if (settings.Settings.SingleScreenImageBehavior == Common.ImageBehavior.Center)
								{
									Rectangle src = new Rectangle();
									src.Width = Math.Min(imageFromDisk.Width, bounds.Width);
									src.Height = Math.Min(imageFromDisk.Height, bounds.Height);
									src.X = Convert.ToInt32(Math.Floor(imageFromDisk.Width / 2m - src.Width / 2m));
									src.Y = Convert.ToInt32(Math.Floor(imageFromDisk.Height / 2m - src.Height / 2m));

									// Center the image in the screen region
									Rectangle dst = new Rectangle();
									dst.X = Convert.ToInt32(Math.Floor(bounds.X + (bounds.Width / 2m - src.Width / 2m))) + drawOffsetX;
									dst.Y = Convert.ToInt32(Math.Floor(bounds.Y + (bounds.Height / 2m - src.Height / 2m))) + drawOffsetY;
									dst.Width = src.Width;
									dst.Height = src.Height;

									// Paint the image to the graphic
									using (ImageAttributes attr = new ImageAttributes())
									{
										attr.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
										g.Clip = new Region(new Rectangle(bounds.X + drawOffsetX, bounds.Y + drawOffsetY, bounds.Width, bounds.Height));
										g.DrawImage(imageFromDisk, dst, src.X, src.Y, src.Width, src.Height, GraphicsUnit.Pixel, attr);
										g.ResetClip();
									}
								}
								else
								{
									// Center the image in the screen region
									Rectangle dst = new Rectangle();
									dst.X = bounds.X + ((bounds.Width - adjustedSize.Width) / 2) + drawOffsetX;
									dst.Y = bounds.Y + ((bounds.Height - adjustedSize.Height) / 2) + drawOffsetY;
									dst.Width = adjustedSize.Width;
									dst.Height = adjustedSize.Height;

									// Paint the image to the graphic
									// Paint the image to the graphic
									using (ImageAttributes attr = new ImageAttributes())
									{
										attr.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
										g.Clip = new Region(new Rectangle(bounds.X + drawOffsetX, bounds.Y + drawOffsetY, bounds.Width, bounds.Height));
										g.DrawImage(imageFromDisk, dst, 0, 0, imageFromDisk.Width, imageFromDisk.Height, GraphicsUnit.Pixel, attr);
										g.ResetClip();
									}

								}
							}
						}
						catch (Exception ex)
						{
							Program.DebugMessage("Exception in BuildSingleScreenWallPaper: {0}", ex.Message);
							Program.DebugMessage(ex.StackTrace);
							continue;
						}
					}

					//img.Save("c:\\stage1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

					// Move the blocks that are above and to the left of the primary to adjust for the wallpaper wrapping
					if (drawOffsetX > 0)
					{
						g.DrawImage(img, 
							new Rectangle(wallPaperWidth, 0, drawOffsetX, wallPaperHeight),
							new Rectangle(0, 0, drawOffsetX, wallPaperHeight),
							GraphicsUnit.Pixel);
					}
					if (drawOffsetY > 0)
					{
						g.DrawImage(img,
							new Rectangle(drawOffsetX, wallPaperHeight, wallPaperWidth, drawOffsetY),
							new Rectangle(drawOffsetX, 0, wallPaperWidth, drawOffsetY), GraphicsUnit.Pixel);
					}


					//img.Save("c:\\stage2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
		
					// Copy the wallpaper section of the completed canvas
					using (Bitmap imgWallpaper = new Bitmap(wallPaperWidth, wallPaperHeight))
					{
						using (Graphics gWallpaper = Graphics.FromImage(imgWallpaper))
						{
							gWallpaper.DrawImage(img, new Rectangle(0,0, wallPaperWidth, wallPaperHeight), new Rectangle(drawOffsetX, drawOffsetY, wallPaperWidth, wallPaperHeight), GraphicsUnit.Pixel);

							// Determine the output format
							string wallpaperFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName, "hydrapaper");
							if (CSSetDesktopWallpaper.Wallpaper.SupportJpgAsWallpaper)
							{
								wallpaperFileName += ".jpg";
								imgWallpaper.Save(wallpaperFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
							}
							else
							{
								wallpaperFileName += ".bmp";
								imgWallpaper.Save(wallpaperFileName, System.Drawing.Imaging.ImageFormat.Bmp);
							}

							return wallpaperFileName;

						}
					}
				}
			}
		}

		// Select an image for each screen. Don't allow duplicates unless there are not enough images.
		private static string GetImageFileName(string path, List<string> exclusionList)
		{
			string fileName = string.Empty;

			List<FileInfo> files = new List<FileInfo>();
			files.AddRange(new DirectoryInfo(path).GetFiles("*.jpg", SearchOption.AllDirectories));
			files.AddRange(new DirectoryInfo(path).GetFiles("*.bmp", SearchOption.AllDirectories));
			files.AddRange(new DirectoryInfo(path).GetFiles("*.png", SearchOption.AllDirectories));
			files.AddRange(new DirectoryInfo(path).GetFiles("*.gif", SearchOption.AllDirectories));
			files.AddRange(new DirectoryInfo(path).GetFiles("*.jpeg", SearchOption.AllDirectories));

			List<string> fileNames = files.Select(f => f.FullName).ToList();

			foreach (string excludedFile in exclusionList)
			{
				fileNames.Remove(excludedFile);
			}

			if (fileNames.Count > 0)
			{
				int rndIndex = new Random().Next(0, fileNames.Count);
				fileName = fileNames[rndIndex];
			}

			return fileName;
		}

		private static Size AdjustImage(Size imageFromDisk, Size targetSize, Common.ImageBehavior behavior)
		{
			// Resize each image based on the settings for each screen
			int newWidth = imageFromDisk.Width, newHeight = imageFromDisk.Height;
			if (behavior == Common.ImageBehavior.Center)
			{
				// Do nothing, centering doesn't affect the image size
			}
			else if (behavior == Common.ImageBehavior.Stretch)
			{
				newWidth = targetSize.Width; newHeight = targetSize.Height;
			}
			else if (behavior == Common.ImageBehavior.TouchOutside)
			{
				newHeight = Convert.ToInt32(Math.Floor((decimal)targetSize.Width / ((decimal)imageFromDisk.Width / (decimal)imageFromDisk.Height)));
				newWidth = targetSize.Width;

				if (newHeight < targetSize.Height)
				{
					newWidth = Convert.ToInt32(Math.Floor((decimal)targetSize.Height / ((decimal)imageFromDisk.Height / (decimal)imageFromDisk.Width)));
					newHeight = targetSize.Height;
				}
			}
			else if (behavior == Common.ImageBehavior.TouchInside)
			{
				newHeight = Convert.ToInt32(Math.Floor((decimal)targetSize.Width / ((decimal)imageFromDisk.Width / (decimal)imageFromDisk.Height)));
				newWidth = targetSize.Width;

				if (newHeight > targetSize.Height)
				{
					newWidth = Convert.ToInt32(Math.Floor((decimal)targetSize.Height / ((decimal)imageFromDisk.Height / (decimal)imageFromDisk.Width)));
					newHeight = targetSize.Height;
				}
				
			}

			return new Size(newWidth, newHeight);
		}


		#region http://automagical.rationalmind.net/2009/08/25/correct-photo-orientation-using-exif/
		/// <summary>
		/// If the image is a JPG and has EXIF data specifying the orientation then we rotate the image so it displays properly
		/// </summary>
		/// <param name="imageFromDisk"></param>
		/// <param name="fileName"></param>
		private static void RotateImage(Image imageFromDisk, string fileName)
		{
			if (fileName.ToLower().EndsWith(".jpg") || fileName.ToLower().EndsWith(".jpeg"))
			{
				Bitmap bmp = (Bitmap)imageFromDisk;
				EXIFextractor exif = new EXIFextractor(ref bmp, "n");
				if (exif["Orientation"] != null)
				{
					RotateFlipType flipType = OrientationToFlipType(exif["Orientation"]);
					imageFromDisk.RotateFlip(flipType);
				}

			}
		}

		private static RotateFlipType OrientationToFlipType(object orientation)
		{
			switch (int.Parse(orientation.ToString()))
			{
				case 1:
					return RotateFlipType.RotateNoneFlipNone;
				case 2:
					return RotateFlipType.RotateNoneFlipX;
				case 3:
					return RotateFlipType.Rotate180FlipNone;
				case 4:
					return RotateFlipType.Rotate180FlipX;
				case 5:
					return RotateFlipType.Rotate90FlipX;
				case 6:
					return RotateFlipType.Rotate90FlipNone;
				case 7:
					return RotateFlipType.Rotate270FlipX;
				case 8:
					return RotateFlipType.Rotate270FlipNone;
				default:
					return RotateFlipType.RotateNoneFlipNone;
			}
		}
		#endregion

	}

}
