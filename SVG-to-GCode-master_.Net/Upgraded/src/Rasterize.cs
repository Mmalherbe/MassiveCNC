using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using UpgradeHelpers.Helpers;
using UpgradeStubs;

namespace SVGtoGCODE
{
	internal static class Rasterize
	{


		public const int GREYLEVELS = 8;


		// Modules to control rasterization

		internal static object rasterFile(string inFile)
		{
			// Raster this file with beta greyscale support

			int X = 0, Y = 0;

			int c = 0;
			ColorCode.typRGB crr = new ColorCode.typRGB();
			int grey = 0;

			// How many levels of greyscale are there? Let's do GREYLEVELS.
			int lastColor = 0;


			PictureBox p = frmInterface.DefInstance.Picture2;

			p.Image = Image.FromFile(inFile);

			// Get the width and height and calculate a scaler
			int pW = p.Width * 15;
			int pH = p.Height * 15;

			// Desired width: 5 inches
			double scalar = 5 / ((double) pW);

			// Now scan the picture from left to right and build the lines
			int tempForEndVar = pH;
			for (double RY = 0; RY <= tempForEndVar; RY += 0.5d)
			{
				Y = Convert.ToInt32(RY);
				SVGParse.newLine();
				SVGParse.addPoint(0, RY * scalar);
				lastColor = -1;

				int tempForEndVar2 = pW;
				for (X = 0; X <= tempForEndVar2; X++)
				{
					// Get the color of this point
					//UPGRADE_ISSUE: (2064) PictureBox method p.Point was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					c = p.Point(X, Y);

					// Convert to greyscale
					crr = ColorCode.convertVBtoRGB(c);
					grey = ((crr.R + crr.G + crr.b) / 3) * (GREYLEVELS / 255); // Convert to 0 to GREYLEVELS

					//UPGRADE_ISSUE: (2064) PictureBox method Picture1.PSet was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					frmInterface.DefInstance.Picture1.PSet(X, Y, ColorTranslator.ToOle(Color.FromArgb(grey * (255 / ((int) GREYLEVELS)), grey * (255 / ((int) GREYLEVELS)), grey * (255 / ((int) GREYLEVELS)))));


					// Draw a line to this point
					if (grey != lastColor)
					{
						if (lastColor != -1)
						{
							SVGParse.addPoint(X * scalar, RY * scalar);
							SVGParse.pData[SVGParse.currentLine].greyLevel = (byte) lastColor;

							SVGParse.newLine();
							SVGParse.addPoint(X * scalar, RY * scalar);
						}

						lastColor = grey;
					}

				}

				// Finish last line
				if (lastColor != -1)
				{
					SVGParse.addPoint((X - 1) * scalar, RY * scalar);
					SVGParse.pData[SVGParse.currentLine].greyLevel = (byte) lastColor;
				}
			}

			Debug.WriteLine("dpone");


			return null;
		}
	}
}