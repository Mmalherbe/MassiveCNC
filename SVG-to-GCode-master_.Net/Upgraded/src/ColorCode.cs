using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.IO;
using UpgradeHelpers.Helpers;

namespace SVGtoGCODE
{
	internal static class ColorCode
	{

		// Routines for converting colours between HSB and RGB, etc.

		public struct typRGB
		{
			public short R;
			public short G;
			public short b;
		}

		public struct typXYZ
		{
			public double X;
			public double Y;
			public double z;
		}

		public struct typLAB
		{
			public double L;
			public double A;
			public double b;
		}

		public struct typHSL
		{
			public double H;
			public double S;
			public double L;
		}

		public struct typCMYK
		{
			public int c;
			public int M;
			public int Y;
			public int K;
		}

		public struct typPantone
		{
			public string PantoneName;
			//UPGRADE_ISSUE: (2068) LoadPictureConstants object was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2068
			public int Color;
			public static typPantone CreateInstance()
			{
				typPantone result = new typPantone();
				result.PantoneName = String.Empty;
				return result;
			}
		}

		public static typPantone[] pantoneList = null;

		internal static string makeHTMLColor(int c)
		{
			// Turns the colour into HTML HEX code.

			int R = c % 256;
			int G = (c / 256) % 256;
			int b = c / 256 / 256;

			return fixZeros(R.ToString("X")) + fixZeros(G.ToString("X")) + fixZeros(b.ToString("X"));
		}

		internal static string fixZeros(string inSt)
		{
			// Adds a 0 to the front if needed.
			string result = "";
			result = inSt;
			if (Strings.Len(result) == 1)
			{
				result = "0" + result;
			}
			return result;
		}

		internal static typRGB convertVBtoRGB(int c)
		{
			// Convert vb color to RGB

			typRGB result = new typRGB();
			result.R = (short) (c % 256);
			result.G = (short) ((c / 256) % 256);
			result.b = (short) (c / 256 / 256);

			return result;
		}


		internal static typHSL RGBtoHSL(typRGB inRGB)
		{

			typHSL result = new typHSL();
			double nTemp = 0;
			double lMin = 0;
			double lMax = 0;

			double H = 0, S = 0;

			double R = inRGB.R;
			double G = inRGB.G;
			double b = inRGB.b;


			if (R > G)
			{
				if (R > b)
				{
					lMax = R;
				}
				else
				{
					lMax = b;
				}
			}
			else
			{
				if (G > b)
				{
					lMax = G;
				}
				else
				{
					lMax = b;
				}
			}

			if (R < G)
			{
				if (R < b)
				{
					lMin = R;
				}
				else
				{
					lMin = b;
				}
			}
			else
			{
				if (G < b)
				{
					lMin = G;
				}
				else
				{
					lMin = b;
				}
			}

			double lDelta = lMax - lMin;

			double L = (lMax * 100) / 255;

			if (lMax > 0)
			{
				S = (lDelta / lMax) * 100;
				if (lDelta > 0)
				{
					if (lMax == R)
					{
						nTemp = (G - b) / lDelta;
					}
					else if (lMax == G)
					{ 
						nTemp = 2 + (b - R) / lDelta;
					}
					else
					{
						nTemp = 4 + (R - G) / lDelta;
					}

					H = nTemp * 60;
					if (H < 0)
					{
						H += 360;
					}
				}
			}

			result.H = Math.Floor(H);
			result.S = Math.Floor(S);
			result.L = Math.Floor(L);

			return result;
		}


		internal static typRGB HSLtoRGB(typHSL inHSL)
		{


			typRGB result = new typRGB();
			double G = 0, R = 0, b = 0;

			double nH = ((inHSL.H == 360) ? 0 : inHSL.H) / 60;
			double nS = inHSL.S / 100;
			double nB = inHSL.L / 100;

			double lH = Math.Floor(nH);
			double nF = nH - lH;
			double nP = nB * (1 - nS);
			double nQ = nB * (1 - nS * nF);
			double nT = nB * (1 - nS * (1 - nF));

			if (lH == 0)
			{
				R = nB * 255;
				G = nT * 255;
				b = nP * 255;
			}
			else if (lH == 1)
			{ 
				R = nQ * 255;
				G = nB * 255;
				b = nP * 255;
			}
			else if (lH == 2)
			{ 
				R = nP * 255;
				G = nB * 255;
				b = nT * 255;
			}
			else if (lH == 3)
			{ 
				R = nP * 255;
				G = nQ * 255;
				b = nB * 255;
			}
			else if (lH == 4)
			{ 
				R = nT * 255;
				G = nP * 255;
				b = nB * 255;
			}
			else if (lH == 5)
			{ 
				R = nB * 255;
				G = nP * 255;
				b = nQ * 255;
			}
			else
			{
				R = (nB * 255) / 100;
				G = R;
				b = R;
			}

			result.R = Convert.ToByte(GeneralFunctions.Min(GeneralFunctions.Max(R, 0), 255));
			result.G = Convert.ToByte(GeneralFunctions.Min(GeneralFunctions.Max(G, 0), 255));
			result.b = Convert.ToByte(GeneralFunctions.Min(GeneralFunctions.Max(b, 0), 255));
			return result;
		}

		internal static int convertRGBToVB(typRGB inRGB)
		{
			return ColorTranslator.ToOle(Color.FromArgb(inRGB.R, inRGB.G, inRGB.b));
		}

		internal static typXYZ RGBtoXYZ(typRGB inRGB)
		{
			// Convert RGB color space to XYZ

			typXYZ result = new typXYZ();
			double var_R = (inRGB.R / 255d); //Where R = 0 ÷ 255
			double var_G = (inRGB.G / 255d); //Where G = 0 ÷ 255
			double var_B = (inRGB.b / 255d); //Where B = 0 ÷ 255

			if (var_R > 0.04045d)
			{
				var_R = Math.Pow((var_R + 0.055d) / 1.055d, 2.4d);
			}
			else
			{
				var_R /= 12.92d;
			}
			if (var_G > 0.04045d)
			{
				var_G = Math.Pow((var_G + 0.055d) / 1.055d, 2.4d);
			}
			else
			{
				var_G /= 12.92d;
			}
			if (var_B > 0.04045d)
			{
				var_B = Math.Pow((var_B + 0.055d) / 1.055d, 2.4d);
			}
			else
			{
				var_B /= 12.92d;
			}

			var_R *= 100;
			var_G *= 100;
			var_B *= 100;

			//Observer. = 2°, Illuminant = D65
			result.X = var_R * 0.4124d + var_G * 0.3576d + var_B * 0.1805d;
			result.Y = var_R * 0.2126d + var_G * 0.7152d + var_B * 0.0722d;
			result.z = var_R * 0.0193d + var_G * 0.1192d + var_B * 0.9505d;

			return result;
		}

		internal static typLAB XYZtoLAB(typXYZ inXYZ)
		{
			// Convert XYZ color space to Hunter-Lab
			typLAB result = new typLAB();
			result.L = 10 * Math.Sqrt(inXYZ.Y);

			if (inXYZ.Y != 0)
			{
				result.A = 17.5d * (((1.02d * inXYZ.X) - inXYZ.Y) / Math.Sqrt(inXYZ.Y));
			}
			if (inXYZ.Y != 0)
			{
				result.b = 7 * ((inXYZ.Y - (0.847d * inXYZ.z)) / Math.Sqrt(inXYZ.Y));
			}

			return result;
		}

		internal static object loadPantone(string inFile)
		{
			string G = "";
			int f = 0;
			int n = 0;
			string[] X = null;
			string[] X2 = null;
			string c = "";

			pantoneList = ArraysHelper.InitializeArray<typPantone>(1);

			if (GeneralFunctions.myDir(inFile) != "")
			{


				G = new String(' ', (int) (new FileInfo(inFile)).Length);
				f = FileSystem.FreeFile();

				FileSystem.FileOpen(f, inFile, OpenMode.Binary, OpenAccess.Default, OpenShare.Default, -1);
				//UPGRADE_WARNING: (2080) Get was upgraded to FileGet and has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2080
				FileSystem.FileGet(f, ref G, -1);
				FileSystem.FileClose(f);

				// Split and translate
				X = (string[]) G.Split(Environment.NewLine[0]);

				foreach (string X_item in X)
				{

					if (X_item != "")
					{
						X2 = (string[]) X_item.Split(new string[]{"\t"}, StringSplitOptions.None); //split by tabs

						n = pantoneList.GetUpperBound(0) + 1;
						pantoneList = ArraysHelper.RedimPreserve(pantoneList, new int[]{n + 1});

						c = X2[1].Trim();
						if (Strings.Len(c) > 6)
						{
							c = c.Substring(Math.Max(c.Length - 6, 0));
						}

						pantoneList[n].PantoneName = X2[0].Trim();
						pantoneList[n].Color = GeneralFunctions.makeVBColor(ref c);
					}
				}
			}

			GeneralFunctions.addToLog(pantoneList.GetUpperBound(0), " pantone colors loaded");


			return null;
		}

		internal static typCMYK RGBtoCMYK(typRGB inRGB)
		{


			typCMYK result = new typCMYK();
			double c = 1 - (inRGB.R / 255);
			double M = 1 - (inRGB.G / 255);
			double Y = 1 - (inRGB.b / 255);
			double K = GeneralFunctions.Min(c, GeneralFunctions.Min(M, Y));

			c = GeneralFunctions.Min(1, GeneralFunctions.Max(0, c - K));
			M = GeneralFunctions.Min(1, GeneralFunctions.Max(0, M - K));
			Y = GeneralFunctions.Min(1, GeneralFunctions.Max(0, Y - K));
			K = GeneralFunctions.Min(1, GeneralFunctions.Max(0, K));

			result.c = Convert.ToInt32(c * 100);
			result.M = Convert.ToInt32(M * 100);
			result.Y = Convert.ToInt32(Y * 100);
			result.K = Convert.ToInt32(K * 100);

			return result;
		}

		internal static typRGB CMYKtoRGB(typCMYK inCMYK)
		{
			typRGB result = new typRGB();
			result.R = (short) (((1 - (inCMYK.K / 100)) * (1 - (inCMYK.c / 100))) * 255);
			result.G = (short) (((1 - (inCMYK.K / 100)) * (1 - (inCMYK.M / 100))) * 255);
			result.b = (short) (((1 - (inCMYK.K / 100)) * (1 - (inCMYK.Y / 100))) * 255);
			return result;
		}

		internal static int textColorBasedOnBackground(int inC)
		{
			// Should the text color be white or black based on the brightness of the background
			typRGB myR = convertVBtoRGB(inC);
			typLAB myL = XYZtoLAB(RGBtoXYZ(myR));
			typHSL myH = RGBtoHSL(myR);
			return ColorTranslator.ToOle((myH.L < 60 || myL.L < 62) ? Color.White : Color.Black);
		}

		internal static string CMYKtoHex(typCMYK inCMYK)
		{
			return fixZeros(inCMYK.c.ToString("X")) + fixZeros(inCMYK.M.ToString("X")) + fixZeros(inCMYK.Y.ToString("X")) + fixZeros(inCMYK.K.ToString("X"));
		}

		internal static typCMYK HextoCMYK(ref string inHex)
		{
			// Turns 64646464 into the CMYK color

			typCMYK result = new typCMYK();
			if (Strings.Len(inHex) < 8)
			{
				inHex = Strings.Replace(new String(' ', 8 - Strings.Len(inHex)), " ", "0", 1, -1, CompareMethod.Binary) + inHex;
			}

			if (Strings.Len(inHex) == 8)
			{
				result.c = Convert.ToInt32(Conversion.Val("&H" + inHex.Substring(0, Math.Min(2, inHex.Length))));
				result.M = Convert.ToInt32(Conversion.Val("&H" + inHex.Substring(2, Math.Min(2, inHex.Length - 2))));
				result.Y = Convert.ToInt32(Conversion.Val("&H" + inHex.Substring(4, Math.Min(2, inHex.Length - 4))));
				result.K = Convert.ToInt32(Conversion.Val("&H" + inHex.Substring(6, Math.Min(2, inHex.Length - 6))));
			}

			return result;
		}
	}
}