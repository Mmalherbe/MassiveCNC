using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using UpgradeHelpers.Helpers;
using UpgradeStubs;

namespace SVGtoGCODE
{
	internal static class SVGParse
	{

		public struct pointD
		{
			public double x;
			public double y;
			public byte noCut;
		}

		public struct typLine
		{
			public pointD[] Points;
			public int SpecialNumPoints;

			public bool Fillable; // Only works for closed paths

			public int ContainedBy; // ID to containing poly

			public double xCenter;
			public double yCenter;

			public bool Optimized;

			public byte greyLevel; // 0 to GREYLEVELS level of grey, higher is lighter

			public string LayerID;

			public string PathCode;

			public int LevelNumber; //How many levels deep is this

			public bool isDel; // Deleted on next iteration
			public static typLine CreateInstance()
			{
				typLine result = new typLine();
				result.LayerID = String.Empty;
				result.PathCode = String.Empty;
				return result;
			}
		}

		private static Hashtable _containList = null;
		internal static Hashtable containList
		{
			get
			{
				if (_containList == null)
				{
					_containList = new Hashtable();
				}
				return _containList;
			}
			set
			{
				_containList = value;
			}
		}


		public static typLine[] pData = null;
		public static int currentLine = 0;

		private static Hashtable _layerInfo = null;
		internal static Hashtable layerInfo
		{
			get
			{
				if (_layerInfo == null)
				{
					_layerInfo = new Hashtable();
				}
				return _layerInfo;
			}
			set
			{
				_layerInfo = value;
			}
		}



		public const double PI = 3.141592654d;
		public static double GLOBAL_DPI = 0;

		public static double EXPORT_EXTENTS_X = 0, EXPORT_EXTENTS_Y = 0;
		public static string LastExportPath = "";
		public static string CurrentFile = "";

		static bool hasUnfinishedLine = false;


		internal static object parseSVG(string inFile)
		{

			CHILKATXMLLib.ChilkatXml SVG = new CHILKATXMLLib.ChilkatXml();
			CHILKATXMLLib.ChilkatXml x = null;

			double realW = 0;
			double realH = 0;

			string[] S = null;


			pData = new SVGParse.typLine[1];
			currentLine = 0;

			double realDPI = 90;

			SVG.LoadXmlFile(inFile);

			if (SVG == null)
			{
				MessageBox.Show("Could not load SVG", Application.ProductName);
				return null;
			}


			//For i = 0 To SVG.childNodes.length - 1
			//    Set x = SVG.childNodes(i)
			//    If x.nodeName = "svg" Then Exit For
			//Next

			if (SVG.Tag == "svg")
			{

				//   width="8.5in"
				//   height="11in"
				//   viewBox="0 0 765.00001 990.00002"

				// Read these numbers to determine the scale of the data inside the file.
				// width and height are the real-world widths and heights
				// viewbox is how we're going to scale the numbers in the file (expressed in pixels) to the native units of this program, which is inches

				realW = Conversion.Val(SVG.GetAttrValue("width"));
				// Read the unit
				switch(Strings.Replace(SVG.GetAttrValue("width"), realW.ToString(), "", 1, -1, CompareMethod.Binary).ToLower())
				{
					case "in" :  // no conversion needed 
						break;
					case "mm" : case "" :  // convert from mm 
						realW /= 25.4d; 
						break;
					case "cm" :  // convert from cm 
						realW /= 2.54d; 
						 
						break;
				}

				realH = Conversion.Val(SVG.GetAttrValue("height"));
				// Read the unit
				switch(Strings.Replace(SVG.GetAttrValue("height"), realH.ToString(), "", 1, -1, CompareMethod.Binary).ToLower())
				{
					case "in" :  // no conversion needed 
						break;
					case "mm" : case "" :  // convert from mm 
						realH /= 25.4d; 
						break;
					case "cm" :  // convert from cm 
						realH /= 2.54d; 
						break;
				}

				//MsgBox "Size in inches: " & realW & ", " & realH

				// The 'ViewBox' is how we scale an inch to a pixel.  The default is 90dpi but it may not be.

				//ttt = InputBox("Detected with: " & realW & " inches.  Change it?", "Width", realW)
				//If ttt <> "" Then
				//    realW = Val(ttt)
				//End If


				S = (string[]) SVG.GetAttrValue("viewBox").Split(' ');
				if (S.GetUpperBound(0) == 3)
				{
					// Get the width in pixels
					if (realW == 0)
					{
						realDPI = 300;
					}
					else
					{
						realDPI = Conversion.Val(S[2]) / realW;
					}
				}


				if (realDPI == 1)
				{
					realDPI = 72;
				}

				//ttt = InputBox("Detected DPI: " & realDPI & ".  Change it?", "DPI")
				//If ttt <> "" Then
				//    realDPI = Val(ttt)
				//End If


				GLOBAL_DPI = realDPI;


				parseSVGKids(SVG);
			}

			// Scale by the DPI
			int tempForEndVar = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				int tempForEndVar2 = pData[i].Points.GetUpperBound(0);
				for (int j = 1; j <= tempForEndVar2; j++)
				{
					pData[i].Points[j].x /= realDPI;
					pData[i].Points[j].y /= realDPI;
				}
			}

			// Fix the extents

			double minX = 1000000;
			double minY = 1000000;

			// Calculate the extents
			int tempForEndVar3 = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar3; i++)
			{
				int tempForEndVar4 = pData[i].Points.GetUpperBound(0);
				for (int j = 1; j <= tempForEndVar4; j++)
				{
					minX = GeneralFunctions.Min(minX, pData[i].Points[j].x);
					minY = GeneralFunctions.Min(minY, pData[i].Points[j].y);
				}
			}


			// Now fix the points by removing space at the left and top

			int tempForEndVar5 = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar5; i++)
			{
				int tempForEndVar6 = pData[i].Points.GetUpperBound(0);
				for (int j = 1; j <= tempForEndVar6; j++)
				{
					pData[i].Points[j].x -= minX;
					pData[i].Points[j].y -= minY;
				}
			}


			return null;
		}


		internal static object parseSVGKids(CHILKATXMLLib.ChilkatXml inEle, ref string currentLayer)
		{

			// Loop through my kids and figure out what to do!
			int beforeLine = 0;

			double cX = 0;
			double cY = 0;
			double cW = 0;
			double cH = 0;

			int beforeGroup = 0;
			string layerName = "";

			if (currentLayer == "")
			{
				currentLayer = "BLANK";
			}


			Debug.WriteLine(Support.TabLayout("PARSING A KIDS:", currentLayer));


			CHILKATXMLLib.ChilkatXml x = (CHILKATXMLLib.ChilkatXml) inEle.FirstChild();
			string thePath = "";

			while(x != null)
			{

				//MsgBox X.nodeName

				switch(x.Tag.ToLower())
				{
					case "g" :  // g is GROUP 
						beforeGroup = currentLine; 
						 
						// Is this group a layer? 
						layerName = getAttr(x, "inkscape:label", ""); 
						if (layerName == "")
						{
							if (getAttr(x, "id", "").IndexOf("layer", StringComparison.CurrentCultureIgnoreCase) >= 0)
							{
								layerName = getAttr(x, "id", "");
							}
						} 
						 
						if (layerName == "")
						{
							layerName = currentLayer;
						} 
						 
						//If layerName = "" Then layerName = getAttr(x, "id", "") 
						 
						parseSVGKids(x, ref layerName); 
						 
						if (getAttr(x, "transform", "") != "")
						{
							// Transform these lines
							int tempForEndVar = currentLine;
							for (int j = beforeGroup + 1; j <= tempForEndVar; j++)
							{
								transformLine(j, getAttr(x, "transform", ""));
							}
						} 
						 
						break;
					case "switch" :  // stupid crap 
						parseSVGKids(x); 
						 
						// SHAPES 
						break;
					case "rect" : case "path" : case "line" : case "polyline" : case "circle" : case "polygon" : case "ellipse" : 
						beforeLine = currentLine; 
						 
						switch(x.Tag.ToLower())
						{
							case "rect" :  // RECTANGLE 
								 
								newLine(currentLayer); 
								cX = Conversion.Val(getAttr(x, "x", "")); 
								cY = Conversion.Val(getAttr(x, "y", "")); 
								cW = Conversion.Val(getAttr(x, "width", "")); 
								cH = Conversion.Val(getAttr(x, "height", "")); 
								addPoint(cX, cY); 
								addPoint(cX + cW, cY); 
								addPoint(cX + cW, cY + cH); 
								addPoint(cX, cY + cH); 
								addPoint(cX, cY); 
								finishLine(); 
								 
								pData[currentLine].Fillable = true; 
								 
								break;
							case "path" : 
								 
								// Parse the path. 
								thePath = getAttr(x, "d", ""); 
								if (x.GetAttrValue("fill") != "" && x.GetAttrValue("fill") != "none")
								{ // For some reason Illustrator doesn't close paths that are filled
									if (Strings.Len(thePath) > 0)
									{
										if (thePath.Substring(Math.Max(thePath.Length - 1, 0)).ToLower() == "z")
										{
											// ALready closed
										}
										else
										{
											thePath = thePath + "z";
										}
									}
								} 
								 
								parsePath(ref thePath, currentLayer); 



								 
								break;
							case "line" : 
								// Add this line 
								newLine(currentLayer); 
								addPoint(Conversion.Val(getAttr(x, "x1", "")), Conversion.Val(getAttr(x, "y1", ""))); 
								addPoint(Conversion.Val(getAttr(x, "x2", "")), Conversion.Val(getAttr(x, "y2", ""))); 
								finishLine(); 
								 
								break;
							case "polyline" : 
								newLine(currentLayer); 
								string tempRefParam = getAttr(x, "points", ""); 
								parsePolyLine(ref tempRefParam); 
								finishLine(); 
								 
								break;
							case "polygon" : 
								newLine(currentLayer); 
								string tempRefParam2 = getAttr(x, "points", ""); 
								parsePolyLine(ref tempRefParam2); 
								finishLine(); 
								 
								pData[currentLine].Fillable = true; 

								 
								break;
							case "circle" : 
								// Draw a circle. 
								newLine(currentLayer); 
								parseCircle(Conversion.Val(getAttr(x, "cx", "")), Conversion.Val(getAttr(x, "cy", "")), Conversion.Val(getAttr(x, "r", ""))); 
								 
								break;
							case "ellipse" :  // Draw an ellipse 
								newLine(currentLayer); 
								//   cx="245.46707" 
								//   cy = "469.48389" 
								//   rx = "13.131983" 
								//   ry="14.142136" /> 
								 
								parseEllipse(Conversion.Val(getAttr(x, "cx", "")), Conversion.Val(getAttr(x, "cy", "")), Conversion.Val(getAttr(x, "rx", "")), Conversion.Val(getAttr(x, "ry", ""))); 
								break;
						} 
						 
						// Shape transformations 
						if (getAttr(x, "transform", "") != "")
						{
							// Transform these lines
							int tempForEndVar2 = currentLine;
							for (int j = beforeLine + 1; j <= tempForEndVar2; j++)
							{
								transformLine(j, getAttr(x, "transform", ""));
							}
						} 
						break;
				}
				x = (CHILKATXMLLib.ChilkatXml) x.NextSibling();
			};



			return null;
		}

		internal static object parseSVGKids(CHILKATXMLLib.ChilkatXml inEle)
		{
			string tempRefParam = "";
			return parseSVGKids(inEle, ref tempRefParam);
		}

		internal static object parseCircle(double cX, double cY, double Radi)
		{

			double x = 0, y = 0;

			int rr = 2;
			if (Radi > 100)
			{
				rr = 1;
			}


			int tempForStepVar = rr;
			for (double A = 0; (tempForStepVar < 0) ? A >= 360 : A <= 360; A += tempForStepVar)
			{

				x = Math.Cos(A * (PI / 180)) * Radi + cX;
				y = Math.Sin(A * (PI / 180)) * Radi + cY;

				addPoint(x, y);


			}

			pData[currentLine].Fillable = true;

			return null;
		}


		internal static object parseEllipse(double cX, double cY, double RadiX, double RadiY)
		{

			double x = 0, y = 0;

			int rr = 2;
			if (RadiX > 100 || RadiY > 100)
			{
				rr = 1;
			}


			int tempForStepVar = rr;
			for (double A = 0; (tempForStepVar < 0) ? A >= 360 : A <= 360; A += tempForStepVar)
			{

				x = Math.Cos(A * (PI / 180)) * RadiX + cX;
				y = Math.Sin(A * (PI / 180)) * RadiY + cY;

				addPoint(x, y);

			}

			pData[currentLine].Fillable = true;

			return null;
		}

		internal static object transformLine(int lineID, string transformText)
		{

			// Parse the transform text
			int e = 0, f = 0;

			string func = "";
			string params_Renamed = "";
			string[] pSplit = null;
			double Ang = 0;



			e = (transformText.IndexOf('(') + 1);
			if (e > 0)
			{
				func = transformText.Substring(0, Math.Min(e - 1, transformText.Length));
				f = Strings.InStr(e + 1, transformText, ")", CompareMethod.Binary);
				if (f > 0)
				{
					params_Renamed = transformText.Substring(e, Math.Min(f - e - 1, transformText.Length - e));
				}

				switch(func.ToLower())
				{
					case "translate" : 
						// Just move everything 
						pSplit = (string[]) params_Renamed.Split(','); 
						 
						// Translate is 
						// [ 1  0  tx ] 
						// [ 0  1  ty ] 
						// [ 0  0  1  ] 
						 
						if (pSplit.GetUpperBound(0) == 0)
						{
							multiplyLineByMatrix(lineID, 1, 0, 0, 1, Conversion.Val(pSplit[0]), 0);
						}
						else
						{
							multiplyLineByMatrix(lineID, 1, 0, 0, 1, Conversion.Val(pSplit[0]), Conversion.Val(pSplit[1]));
						} 
						 
						break;
					case "matrix" : 
						pSplit = (string[]) params_Renamed.Split(','); 
						if (pSplit.GetUpperBound(0) == 0)
						{
							pSplit = (string[]) params_Renamed.Split(' ');
						} 
						multiplyLineByMatrix(lineID, Conversion.Val(pSplit[0]), Conversion.Val(pSplit[1]), Conversion.Val(pSplit[2]), Conversion.Val(pSplit[3]), Conversion.Val(pSplit[4]), Conversion.Val(pSplit[5])); 
						 
						break;
					case "rotate" : 
						 
						pSplit = (string[]) params_Renamed.Split(','); 
						Ang = Deg2Rad(Conversion.Val(pSplit[0])); 
						 
						multiplyLineByMatrix(lineID, Math.Cos(Ang), Math.Sin(Ang), -Math.Sin(Ang), Math.Cos(Ang), 0, 0); 
						 
						break;
					case "scale" :  // scale(-1,-1) 
						pSplit = (string[]) params_Renamed.Split(','); 
						if (pSplit.GetUpperBound(0) == 0)
						{
							pSplit = (string[]) params_Renamed.Split(' ');
						} 
						if (pSplit.GetUpperBound(0) == 0)
						{
							// Handle shitty SVG, such as not having two parameters
							pSplit = ArraysHelper.RedimPreserve(pSplit, new int[]{2});
							pSplit[1] = pSplit[0];
						} 
						multiplyLineByMatrix(lineID, Conversion.Val(pSplit[0]), 0, 0, Conversion.Val(pSplit[1]), 0, 0); 

						 
						break;
				}

			}

			return null;
		}

		internal static object multiplyLineByMatrix(int polyID, double A, double b, double c, double D, double e, double f)
		{
			// Miltiply a line/poly by a transformation matrix
			// [ A C E ]
			// [ B D F ]
			// [ 0 0 1 ]

			// http://www.w3.org/TR/SVG11/coords.html#TransformMatrixDefined
			//X1 = AX + CY + E
			//Y1 = BX + DY + F
			pointD oldPoint = new pointD();

			int tempForEndVar = pData[polyID].Points.GetUpperBound(0);
			for (int j = 1; j <= tempForEndVar; j++)
			{
				oldPoint = pData[polyID].Points[j];
				pData[polyID].Points[j].x = (A * oldPoint.x) + (c * oldPoint.y) + e;
				pData[polyID].Points[j].y = (b * oldPoint.x) + (D * oldPoint.y) + f;
			}

			return null;
		}

		internal static object parsePolyLine(ref string inLine)
		{
			// Parse a polyline
			string token1 = "", token2 = "";
			inLine = Strings.Replace(inLine, "\r", " ", 1, -1, CompareMethod.Binary);
			inLine = Strings.Replace(inLine, Constants.vbLf, " ", 1, -1, CompareMethod.Binary);

			int pos = 1;

			while(pos <= Strings.Len(inLine))
			{
				skipWhiteSpace(inLine, ref pos);
				token1 = extractToken(inLine, ref pos);
				skipWhiteSpace(inLine, ref pos);
				token2 = extractToken(inLine, ref pos);

				if (token1 != "" && token2 != "")
				{
					addPoint(Conversion.Val(token1), Conversion.Val(token2));
				}
			};


			// Close the shape.
			if (pData[currentLine].Points.GetUpperBound(0) > 0)
			{
				addPoint(pData[currentLine].Points[1].x, pData[currentLine].Points[1].y);
			}


			return null;
		}

		internal static object parsePath(ref string inPath, string currentLayer)
		{




			// Parse an SVG path.
			string char_Renamed = "";
			string token3 = "", token1 = "", token2 = "", token4 = "";
			string token7 = "", token5 = "", token6 = "";


			bool isRelative = false;
			bool gotFirstItem = false;

			double currX = 0;
			double currY = 0;

			pointD pt0 = new pointD();
			pointD pt1 = new pointD();
			pointD pt2 = new pointD();
			pointD pt3 = new pointD();
			pointD pt4 = new pointD();
			pointD pt5 = new pointD();

			pointD ptPrevPoint = new pointD();
			bool hasPrevPoint = false;

			int lastUpdate = 0;





			double startX = 0;
			double startY = 0;

			double pInSeg = 0;
			string lastChar = "";



			//M209.1,187.65c-0.3-0.2-0.7-0.4-1-0.4c-0.3,0-0.7,0.2-0.9,0.4c-0.3,0.3-0.4,0.6-0.4,0.9c0,0.4,0.1,0.7,0.4,1
			//c0.2,0.2,0.6,0.4,0.9,0.4c0.3,0,0.7-0.2,1-0.4c0.2-0.3,0.3-0.6,0.3-1C209.4,188.25,209.3,187.95,209.1,187.65z

			// Get rid of enter presses
			inPath = Strings.Replace(inPath, "\r", " ", 1, -1, CompareMethod.Binary);
			inPath = Strings.Replace(inPath, Constants.vbLf, " ", 1, -1, CompareMethod.Binary);
			inPath = Strings.Replace(inPath, "\t", " ", 1, -1, CompareMethod.Binary);

			// Start parsing
			int pos = 1;

			while(pos <= Strings.Len(inPath))
			{
				char_Renamed = inPath.Substring(pos - 1, Math.Min(1, inPath.Length - (pos - 1)));
				pos++;
				isRelative = false;

				switch(char_Renamed)
				{
					case "M" : case "m" : case "L" : case "l" : case "C" : case "c" : case "V" : case "v" : case "A" : case "a" : case "H" : case "h" : case "S" : case "s" : case "Z" : case "z" : case "q" : case "Q" : case "T" : case "t" : 
						// Accepted character. 
						lastChar = char_Renamed; 
						break;
					case " " : 
						 
						break;
					default:
						// No accepted, must be a continuation. 
						char_Renamed = lastChar; 
						if (char_Renamed == "m")
						{
							char_Renamed = "l";
						}  // Continuous moveto becomes lineto 
						if (char_Renamed == "M")
						{
							char_Renamed = "L";
						}  // Continuous moveto becomes lineto not relative 
						pos--; 
						break;
				}


				switch(char_Renamed)
				{
					case " " :  // Skip spaces 
						 
						break;
					case "M" : case "m" :  // MOVE TO 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 

						 
						// Extract two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set our "current" co-ordinates to this 
						if (isRelative)
						{
							currX += Conversion.Val(token1);
							currY += Conversion.Val(token2);
						}
						else
						{
							currX = Conversion.Val(token1);
							currY = Conversion.Val(token2);
						} 
						 
						// Start a new line, since we moved 
						//If Not isRelative Then 
						newLine(currentLayer); 
						//pData(currentLine).PathCode = Right(inPath, Len(inPath) - pos) 
						 
						// Add the start point to this line 
						addPoint(currX, currY); 

						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "Move to " & currX & ", " & currY & vbCrLf 

						 
						//If Not gotFirstItem Then 
						startX = currX;  
						startY = currY; 
						gotFirstItem = true; 
						hasPrevPoint = false; 
						 
						break;
					case "L" : case "l" :  // LINE TO 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 

						 
						// Extract two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set our "current" co-ordinates to this 
						if (isRelative)
						{
							currX += Conversion.Val(token1);
							currY += Conversion.Val(token2);
						}
						else
						{
							currX = Conversion.Val(token1);
							currY = Conversion.Val(token2);
						} 
						 
						// Add this point to the line 
						addPoint(currX, currY); 
						 
						//'pData(currentLine).PathCode = pData(currentLine).PathCode & "Line to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						hasPrevPoint = false; 
						 
						break;
					case "V" : case "v" :  // VERTICAL LINE TO 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 
						 
						// Extract one co-ordinate 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						 
						// Set our "current" co-ordinates to this 
						if (isRelative)
						{
							currY += Conversion.Val(token1);
						}
						else
						{
							currY = Conversion.Val(token1);
						} 
						 
						// Add this point to the line 
						addPoint(currX, currY); 
						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "Vertical to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						hasPrevPoint = false; 
						 
						break;
					case "H" : case "h" :  // HORIZONTAL LINE TO 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 
						 
						// Extract one co-ordinate 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						 
						// Set our "current" co-ordinates to this 
						if (isRelative)
						{
							currX += Conversion.Val(token1);
						}
						else
						{
							currX = Conversion.Val(token1);
						} 
						 
						// Add this point to the line 
						addPoint(currX, currY); 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "Horiz to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						hasPrevPoint = false; 
						 
						break;
					case "A" : case "a" :  // PARTIAL ARC TO 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 
						 
						//(rx ry x-axis-rotation large-arc-flag sweep-flag x y)+ 
						 
						// Radii X and Y 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// X axis rotation 
						skipWhiteSpace(inPath, ref pos); 
						token3 = extractToken(inPath, ref pos); 
						 
						// Large arc flag 
						skipWhiteSpace(inPath, ref pos); 
						token4 = extractToken(inPath, ref pos); 
						 
						// Sweep flag 
						skipWhiteSpace(inPath, ref pos); 
						token5 = extractToken(inPath, ref pos); 
						 
						// X and y 
						skipWhiteSpace(inPath, ref pos); 
						token6 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token7 = extractToken(inPath, ref pos); 
						 
						// Start point 
						pt0.x = currX; 
						pt0.y = currY; 
						 
						// Set our "current" co-ordinates to this 
						if (isRelative)
						{
							currX += Conversion.Val(token6);
							currY += Conversion.Val(token7);
						}
						else
						{
							currX = Conversion.Val(token6);
							currY = Conversion.Val(token7);
						} 
						 
						pt1.x = currX; 
						pt1.y = currY; 
						 
						double tempRefParam = Conversion.Val(token1); 
						double tempRefParam2 = Conversion.Val(token2); 
						parseArcSegment(ref tempRefParam, ref tempRefParam2, Conversion.Val(token3), pt0, pt1, token4 == "1", token5 == "1"); 
						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "Partial Arc to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						hasPrevPoint = false; 
						 
						break;
					case "C" : case "c" :  // CURVE TO 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 
						 
						pt0.x = currX; 
						pt0.y = currY; 
						 
						// Extract two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 0 
						pt1.x = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						pt1.y = ((isRelative) ? currY : 0) + Conversion.Val(token2); 

						 
						// Extract next two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 1 
						pt2.x = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						pt2.y = ((isRelative) ? currY : 0) + Conversion.Val(token2); 
						 
						// Extract next two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 2 
						currX = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						currY = ((isRelative) ? currY : 0) + Conversion.Val(token2); 
						pt3.x = currX; 
						pt3.y = currY; 
						 
						// 
						pInSeg = getPinSeg(pt0, pt3); 


						 
						// Run the bezier code with 4 points 
						Bezier.AddBezier(pInSeg, pt0, pt1, pt2, pt3); 
						 
						// Reflect this point about pt3 
						 
						ptPrevPoint = reflectAbout(pt2, pt3); 
						hasPrevPoint = true; 
						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "Bezier to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						 
						break;
					case "S" : case "s" :  // CURVE TO with 3 points 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 
						 
						pt0.x = currX; 
						pt0.y = currY; 
						 
						// Extract two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 0 
						pt1.x = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						pt1.y = ((isRelative) ? currY : 0) + Conversion.Val(token2); 
						 
						// Extract next two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 1 
						currX = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						currY = ((isRelative) ? currY : 0) + Conversion.Val(token2); 
						pt2.x = currX; 
						pt2.y = currY; 
						 
						pInSeg = getPinSeg(pt0, pt2); 

						 
						if (!hasPrevPoint)
						{
							// Same as pt1
							ptPrevPoint = pt1;
						} 
						 
						Bezier.AddBezier(pInSeg, pt0, ptPrevPoint, pt1, pt2); 
						 
						ptPrevPoint = reflectAbout(pt1, pt2); 
						hasPrevPoint = true; 

						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "3Bezier to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						 
						break;
					case "Q" : case "q" :  // Quadratic Bezier TO with 3 points 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 
						 
						pt0.x = currX; 
						pt0.y = currY; 
						 
						// Extract two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 0 
						pt1.x = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						pt1.y = ((isRelative) ? currY : 0) + Conversion.Val(token2); 
						 
						// Extract next two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 1 
						currX = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						currY = ((isRelative) ? currY : 0) + Conversion.Val(token2); 
						pt2.x = currX; 
						pt2.y = currY; 
						 
						pInSeg = getPinSeg(pt0, pt2); 

						 
						//If Not hasPrevPoint Then 
						//    ' Same as pt1 
						//    ptPrevPoint = pt1 
						//End If 
						 
						Bezier.AddQuadBezier(pInSeg, pt0, pt1, pt2); 
						 
						ptPrevPoint = reflectAbout(pt1, pt2); 
						hasPrevPoint = true; 
						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "3Bezier to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						 
						break;
					case "T" : case "t" :  // Quadratic Bezier TO with 3 points, but use reflection of last 
						if (char_Renamed.ToLower() == char_Renamed)
						{
							isRelative = true;
						}  // Lowercase means relative co-ordinates 
						if (!gotFirstItem)
						{
							isRelative = false;
						}  //Relative not valid for first item 
						 
						pt0.x = currX; 
						pt0.y = currY; 
						 
						// Extract two co-ordinates 
						skipWhiteSpace(inPath, ref pos); 
						token1 = extractToken(inPath, ref pos); 
						skipWhiteSpace(inPath, ref pos); 
						token2 = extractToken(inPath, ref pos); 
						 
						// Set into point 0 
						pt1.x = ((isRelative) ? currX : 0) + Conversion.Val(token1); 
						pt1.y = ((isRelative) ? currY : 0) + Conversion.Val(token2); 
						 
						pInSeg = getPinSeg(pt0, pt1); 


						 
						if (!hasPrevPoint)
						{
							// Same as pt1
							ptPrevPoint = pt0; // SHOULD NEVER HAPPEN
						} 
						 
						Bezier.AddQuadBezier(pInSeg, pt0, ptPrevPoint, pt1); 
						 
						ptPrevPoint = reflectAbout(ptPrevPoint, pt1); 
						hasPrevPoint = true; 
						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "3Bezier to " & currX & ", " & currY & vbCrLf 
						 
						if (!gotFirstItem)
						{
							startX = currX;
							startY = currY;
						} 
						gotFirstItem = true; 
						 
						break;
					case "z" : case "Z" : 
						 
						hasPrevPoint = false; 
						 
						// z means end the shape 
						// Draw a line back to start of shape 
						addPoint(startX, startY); 
						currX = startX; 
						currY = startY; 

						 
						// Since this is a closed path, mark it as fillable. 
						pData[currentLine].Fillable = true; 
						 
						//gotFirstItem = False 

						 
						//pData(currentLine).PathCode = pData(currentLine).PathCode & "End Shape" & vbCrLf 


						 
						break;
					default:
						Debug.WriteLine(Support.TabLayout("UNSUPPORTED PATH CODE: ", char_Renamed)); 

						 
						break;
				}


				if (pos > lastUpdate + 2000)
				{
					lastUpdate = pos;
					frmInterface.DefInstance.Text = "Parsing path: " + pos.ToString() + " / " + Strings.Len(inPath).ToString();
					Application.DoEvents();
				}

			};




			return null;
		}

		internal static double getPinSeg(pointD pStart, pointD pEnd)
		{
			double D = Polygons.pointDistance(pStart, pEnd) / GLOBAL_DPI;
			//MsgBox "distance: " & D

			//Select Case d
			//    Case Is > 20
			//        getPinSeg = 0.1
			//    Case Is > 10
			//        getPinSeg = 0.2
			//    Case Is > 5
			//        getPinSeg = 0.25
			//    Case Else
			//        getPinSeg = 0.3
			//End Select


			// with a resolution of 500 dpi, the curve should be split into 500 segments per inch. so a distance of 1 should be 500 segments, which is 0.002
			double segments = 250 * D;

			if (segments == 0)
			{
				segments = 1;
			}

			if (segments == 0)
			{ // a zero-length line? what's the point
				return 0.01d;
			}
			else
			{
				return GeneralFunctions.Max(0.01d, 1 / segments);

			}





		}




		internal static pointD reflectAbout(pointD ptReflect, pointD ptOrigin)
		{
			// Reflect ptReflect 180 degrees around ptOrigin


			pointD result = new pointD();
			result.x = (-(ptReflect.x - ptOrigin.x)) + ptOrigin.x;
			result.y = (-(ptReflect.y - ptOrigin.y)) + ptOrigin.y;


			return result;
		}

		internal static object parseArcSegment(ref double RX, ref double RY, double rotAng, pointD P1, pointD P2, bool largeArcFlag, bool sweepFlag)
		{

			// Parse "A" command in SVG, which is segments of an arc
			// P1 is start point
			// P2 is end point

			pointD centerPoint = new pointD();
			pointD P1Prime = new pointD();
			pointD P2Prime = new pointD();

			pointD CPrime = new pointD();
			double Q = 0;
			double c = 0;


			pointD tempPoint = new pointD();
			double tempAng = 0;
			double tempDist = 0;





			// Turn the degrees of rotation into radians
			double Theta = Deg2Rad(rotAng);

			// Calculate P1Prime
			P1Prime.x = (Math.Cos(Theta) * ((P1.x - P2.x) / 2)) + (Math.Sin(Theta) * ((P1.y - P2.y) / 2));
			P1Prime.y = ((-Math.Sin(Theta)) * ((P1.x - P2.x) / 2)) + (Math.Cos(Theta) * ((P1.y - P2.y) / 2));

			P2Prime.x = (Math.Cos(Theta) * ((P2.x - P1.x) / 2)) + (Math.Sin(Theta) * ((P2.y - P1.y) / 2));
			P2Prime.y = ((-Math.Sin(Theta)) * ((P2.x - P1.x) / 2)) + (Math.Cos(Theta) * ((P2.y - P1.y) / 2));

			double qTop = ((Math.Pow(RX, 2)) * (Math.Pow(RY, 2))) - ((Math.Pow(RX, 2)) * (Math.Pow(P1Prime.y, 2))) - ((Math.Pow(RY, 2)) * (Math.Pow(P1Prime.x, 2)));

			if (qTop < 0)
			{ // We've been given an invalid arc. Calculate the correct value.

				c = Math.Sqrt(((Math.Pow(P1Prime.y, 2)) / (Math.Pow(RY, 2))) + ((Math.Pow(P1Prime.x, 2)) / (Math.Pow(RX, 2))));

				RX *= c;
				RY *= c;

				qTop = 0;
			}

			double qBot = ((Math.Pow(RX, 2)) * (Math.Pow(P1Prime.y, 2))) + ((Math.Pow(RY, 2)) * (Math.Pow(P1Prime.x, 2)));
			if (qBot != 0)
			{
				Q = Math.Sqrt((qTop) / (qBot));
			}
			else
			{
				Q = 0;
			}
			// Q is negative
			if (largeArcFlag == sweepFlag)
			{
				Q = -Q;
			}

			// Calculate Center Prime
			CPrime.x = 0;

			if (RY != 0)
			{
				CPrime.x = Q * ((RX * P1Prime.y) / RY);
			}
			if (RX != 0)
			{
				CPrime.y = Q * (-((RY * P1Prime.x) / RX));
			}

			// Calculate center point
			centerPoint.x = ((Math.Cos(Theta) * CPrime.x) - (Math.Sin(Theta) * CPrime.y)) + ((P1.x + P2.x) / 2);
			centerPoint.y = ((Math.Sin(Theta) * CPrime.x) + (Math.Cos(Theta) * CPrime.y)) + ((P1.y + P2.y) / 2);

			// TEMPTEMP

			frmInterface.DefInstance.Zoom = 2;
			frmInterface.DefInstance.panX = 140;
			frmInterface.DefInstance.panY = 140;


			//UPGRADE_ISSUE: (2064) PictureBox method Picture1.Circle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			frmInterface.DefInstance.Picture1.Circle((float) ((centerPoint.x + frmInterface.DefInstance.panX) * frmInterface.DefInstance.Zoom), (float) ((centerPoint.y + frmInterface.DefInstance.panY) * frmInterface.DefInstance.Zoom), 10, ColorTranslator.ToOle(Color.Blue), 0, 0, 0);
			//UPGRADE_ISSUE: (2064) PictureBox method Picture1.Circle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			frmInterface.DefInstance.Picture1.Circle((float) ((P1.x + frmInterface.DefInstance.panX) * frmInterface.DefInstance.Zoom), (float) ((P1.y + frmInterface.DefInstance.panY) * frmInterface.DefInstance.Zoom), 10, ColorTranslator.ToOle(Color.Lime), 0, 0, 0);
			//UPGRADE_ISSUE: (2064) PictureBox method Picture1.Circle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			frmInterface.DefInstance.Picture1.Circle((float) ((P2.x + frmInterface.DefInstance.panX) * frmInterface.DefInstance.Zoom), (float) ((P2.y + frmInterface.DefInstance.panY) * frmInterface.DefInstance.Zoom), 10, ColorTranslator.ToOle(Color.Red), 0, 0, 0);

			Debug.WriteLine("Circle");

			// Calculate Theta1

			double Theta1 = angleFromPoint(P1Prime, CPrime);
			double ThetaDelta = angleFromPoint(P2Prime, CPrime);

			Theta1 -= PI;
			ThetaDelta -= PI;

			//Theta1 = angleFromVect(((P1Prime.X - CPrime.X) / RX), ((P1Prime.Y - CPrime.Y) / RY), (P1Prime.X - CPrime.X), (P1Prime.Y - CPrime.Y))
			//ThetaDelta = angleFromVect(((-P1Prime.X - CPrime.X) / RX), ((-P1Prime.Y - CPrime.Y) / RY), (-P1Prime.X - CPrime.X), (-P1Prime.Y - CPrime.Y))

			//Theta1 = Theta1 - (PI / 2)
			//ThetaDelta = ThetaDelta - (PI / 2)

			//If Theta1 = ThetaDelta Then ThetaDelta = ThetaDelta + (PI * 2)

			//Debug.Print Theta1


			if (sweepFlag)
			{ // Sweep is going POSITIVELY
				if (ThetaDelta < Theta1)
				{
					ThetaDelta += (PI * 2);
				}
			}
			else
			{
				// Sweep  is going NEGATIVELY
				//If ThetaDelta < 0 Then ThetaDelta = ThetaDelta + (PI * 2)
				if (ThetaDelta > Theta1)
				{
					ThetaDelta -= (PI * 2);
				}
			}


			double startAng = Theta1;
			double endAng = ThetaDelta;


			double AngStep = (PI / 180);
			if (!sweepFlag)
			{
				AngStep = -AngStep;
			} // Sweep flag indicates a positive step

			Debug.WriteLine(Support.TabLayout("Start angle", Rad2Deg(startAng).ToString(), " End angle ", Rad2Deg(endAng).ToString(), "Step ", Rad2Deg(AngStep).ToString()));

			//Theta = Deg2Rad(-40)

			// Hackhack
			//startAng = startAng + AngStep * 2


			double Ang = startAng;
			do 
			{
				// X   =   RX
				//pt4.X = (pt1.X * Cos(Ang))
				//pt4.Y = (pt1.Y * Sin(Ang))

				//pt4.X = (Cos(Theta) * pt4.X) + (-Sin(Theta) * pt4.Y)
				//pt4.Y = (Sin(Theta) * pt4.X) + (Cos(Theta) * pt4.Y)

				//         X      CX
				//pt4.X = pt4.X + pt3.X
				//pt4.Y = pt4.Y + pt3.Y

				tempPoint.x = (RX * Math.Cos(Ang)) + centerPoint.x;
				tempPoint.y = (RY * Math.Sin(Ang)) + centerPoint.y;

				tempAng = angleFromPoint(centerPoint, tempPoint) + Theta;
				tempDist = Polygons.pointDistance(centerPoint, tempPoint);

				tempPoint.x = (tempDist * Math.Cos(tempAng)) + centerPoint.x;
				tempPoint.y = (tempDist * Math.Sin(tempAng)) + centerPoint.y;





				//tempPoint.X = (Cos(Theta) * tempPoint.X) + (-Sin(Theta) * tempPoint.Y)
				//tempPoint.Y = (Sin(Theta) * tempPoint.X) + (Cos(Theta) * tempPoint.Y)


				addPoint(tempPoint.x, tempPoint.y);


				Ang += AngStep;
			}
			while(!((Ang >= endAng && AngStep > 0) || (Ang <= endAng && AngStep < 0)));

			// Add the final point

			addPoint(P2.x, P2.y);


			return null;
		}

		internal static pointD rotatePoint(pointD inPoint, double Theta, pointD centerPoint)
		{

			pointD result = new pointD();
			result = inPoint;

			result.x -= centerPoint.x;
			result.y -= centerPoint.y;

			result.x = (Math.Cos(Theta) * result.x) + ((-Math.Sin(Theta)) * result.y);
			result.y = (Math.Sin(Theta) * result.x) + (Math.Cos(Theta) * result.y);

			result.x += centerPoint.x;
			result.y += centerPoint.y;



			return result;
		}


		internal static double Rad2Deg(double inRad)
		{
			return inRad * (180 / PI);
		}

		internal static double Deg2Rad(double inDeg)
		{
			return inDeg / (180 / PI);
		}

		internal static double angleFromVect(double vTop, double vBot, double diffX, double diffY)
		{
			// Not sure if this working

			if (vBot == 0)
			{
				return (vTop > 0) ? PI / 2 : (-PI) / 2;
			}
			else if (diffX >= 0)
			{ 
				return Math.Atan(vTop / vBot);
			}
			else
			{
				return Math.Atan(vTop / vBot) - PI;
			}

		}

		internal static double angleFromPoint(pointD pCenter, pointD pPoint)
		{
			// Calculate the angle of a point relative to the center

			// Slope is rise over run
			double result = 0;
			double slope = 0;

			if (pPoint.x == pCenter.x)
			{
				// Either 90 or 270
				result = (pPoint.y > pCenter.y) ? PI / 2 : (-PI) / 2;

			}
			else if (pPoint.x > pCenter.x)
			{ 
				// 0 - 90 and 270-360
				slope = (pPoint.y - pCenter.y) / (pPoint.x - pCenter.x);
				result = Math.Atan(slope);
			}
			else
			{
				// 180-270
				slope = (pPoint.y - pCenter.y) / (pPoint.x - pCenter.x);
				result = Math.Atan(slope) + PI;
			}

			if (result < 0)
			{
				result += (PI * 2);
			}




			return result;
		}

		internal static object newLine(string theLayer = "")
		{

			if (hasUnfinishedLine)
			{
				finishLine();
			}



			currentLine = pData.GetUpperBound(0) + 1;
			// Set up this line
			pData = ArraysHelper.RedimPreserve(pData, new int[]{currentLine + 1});
			pData[currentLine].Points = new SVGParse.pointD[1];

			pData[currentLine].LayerID = theLayer;


			return null;
		}

		internal static object finishLine()
		{
			if (hasUnfinishedLine)
			{
				hasUnfinishedLine = false;

				// Remove the excess
				pData[currentLine].Points = ArraysHelper.RedimPreserve(pData[currentLine].Points, new int[]{pData[currentLine].SpecialNumPoints + 1});
			}

			return null;
		}

		internal static object addPoint(double x, double y, bool noCutLineSegment = false)
		{

			int n = 0;

			if (pData[currentLine].Points[pData[currentLine].Points.GetUpperBound(0)].x == x && pData[currentLine].Points[pData[currentLine].Points.GetUpperBound(0)].y == y && pData[currentLine].Points.GetUpperBound(0) > 0)
			{
				// No point to add
				//Debug.Print "same as last point"

			}
			else
			{

				// Once we get over 5000 points, we enter a special allocation mode.
				if (pData[currentLine].Points.GetUpperBound(0) > 5000)
				{
					hasUnfinishedLine = true;

					// Allocate in blocks of 5000 at a time.
					n = pData[currentLine].SpecialNumPoints + 1;
					if (n > pData[currentLine].Points.GetUpperBound(0))
					{
						pData[currentLine].Points = ArraysHelper.RedimPreserve(pData[currentLine].Points, new int[]{pData[currentLine].Points.GetUpperBound(0) + 5001});
					}

				}
				else
				{
					n = pData[currentLine].Points.GetUpperBound(0) + 1;
					pData[currentLine].Points = ArraysHelper.RedimPreserve(pData[currentLine].Points, new int[]{n + 1});
				}


				pData[currentLine].Points[n].x = x;
				pData[currentLine].Points[n].y = y;
				pData[currentLine].SpecialNumPoints = n;
				if (noCutLineSegment)
				{
					pData[currentLine].Points[n].noCut = 1;
				}
			}


			return null;
		}

		internal static object skipWhiteSpace(string inPath, ref int pos)
		{
			// Skip any white space.
			string char_Renamed = "";


			while(pos <= Strings.Len(inPath))
			{
				char_Renamed = inPath.Substring(pos - 1, Math.Min(1, inPath.Length - (pos - 1)));
				switch(char_Renamed)
				{
					case " " : case "," : case "\t" :  // List all white space characters here 
						// Continue 
						break;
					default:
						return null;
				}

				pos++;
			};
			return null;
		}


		internal static string extractToken(string inPath, ref int pos)
		{

			// Exract until we get a space or a comma
			string char_Renamed = "";
			StringBuilder build = new StringBuilder();
			bool seenMinus = false;
			bool seenE = false;
			bool seenPeriod = false;

			int startPos = pos;



			while(pos <= Strings.Len(inPath))
			{
				char_Renamed = inPath.Substring(pos - 1, Math.Min(1, inPath.Length - (pos - 1)));

				switch(char_Renamed)
				{
					// Only accept numbers
					case "." :  // A period can be seen anywhere in the number, but if a second period is found it means we must exit 
						if (seenPeriod)
						{
							goto exit_do;
						}
						else
						{
							seenPeriod = true;
							build.Append(char_Renamed);
							pos++;
						} 
						 
						break;
					case "-" : 
						if (seenE)
						{
							build.Append(char_Renamed);
							pos++;
						}
						else if (seenMinus || pos > startPos)
						{ 
							goto exit_do;
						}
						else
						{
							// We already saw a minus sign
							seenMinus = true;
							build.Append(char_Renamed);
							pos++;
						} 
						 
						break;
					//UPGRADE_NOTE: (7001) The following case (switch) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
					//case "." : 
						//break;
					case "0" : case "1" : case "2" : case "3" : case "4" : case "5" : case "6" : case "7" : case "8" : case "9" : 
						build.Append(char_Renamed); 
						pos++; 
						//,6.192 -10e-4,12.385 
						break;
					case "e" :  // Exponent 
						seenE = true; 
						build.Append(char_Renamed); 
						pos++; 
						break;
					default:
						goto exit_do;
				}
			};
			exit_do:
			return build.ToString();

		}

		internal static bool isNumChar(string char_Renamed)
		{
			bool result = false;
			switch(char_Renamed)
			{
				// Only accept numbers
				case "0" : case "1" : case "2" : case "3" : case "4" : case "5" : case "6" : case "7" : case "8" : case "9" : case "-" : case "." : 
					result = true; 
					break;
			}



			return result;
		}


		internal static string getAttr(CHILKATXMLLib.ChilkatXml attr, string attrName, string DefaultValue = "")
		{

			return attr.GetAttrValue(attrName);

		}

		internal static bool pointIsInPoly(int polyID, double x, double y)
		{

			// Determine if this point is inside the polygon.



			bool result = false;
			int j = 0;

			j = pData[polyID].Points.GetUpperBound(0);

			int tempForEndVar = pData[polyID].Points.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{

				if (pData[polyID].Points[i].y < y && pData[polyID].Points[j].y >= y || pData[polyID].Points[j].y < y && pData[polyID].Points[i].y >= y)
				{
					if (pData[polyID].Points[i].x + (y - pData[polyID].Points[i].y) / (pData[polyID].Points[j].y - pData[polyID].Points[i].y) * (pData[polyID].Points[j].x - pData[polyID].Points[i].x) < x)
					{
						result = !result;
					}
				}

				j = i;
			}


			//  int      i, j=polySides-1 ;
			//  boolean  oddNodes=NO      ;
			//
			//  for (i=0; i<polySides; i++) {
			//    if (polyY[i]<y && polyY[j]>=y
			//    ||  polyY[j]<y && polyY[i]>=y) {
			//      if (polyX[i]+(y-polyY[i])/(polyY[j]-polyY[i])*(polyX[j]-polyX[i])<x) {
			//        oddNodes=!oddNodes; }}
			//    j=i; }
			//
			//  return oddNodes; }


			//    Dim nPol As Long
			//    Dim i As Long, j As Long
			//
			//    Dim counter As Long
			//
			//    Dim p1 As pointD
			//    Dim p2 As pointD
			//    Dim p As pointD
			//    Dim n As Long
			//    Dim xinters As Double
			//
			//    p.X = X
			//    p.Y = Y
			//
			//
			//  'double xinters;
			//  'Point p1,p2;
			//    With pData(polyID)
			//        n = UBound(.Points)
			//        p1 = .Points(1)
			//        For i = 1 To n
			//            p2 = .Points(i Mod n)
			//
			//            If (p.Y > Min(p1.Y, p2.Y)) Then
			//                If (p.Y <= Max(p1.Y, p2.Y)) Then
			//                    If (p.X <= Max(p1.X, p2.X)) Then
			//                        If (p1.Y <> p2.Y) Then
			//                            xinters = (p.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X
			//                            If (p1.X = p2.X Or p.X <= xinters) Then counter = counter + 1
			//                        End If
			//                    End If
			//                End If
			//            End If
			//            p1 = p2
			//        Next
			//
			//    End With
			//
			//    If counter Mod 2 = 0 Then
			//        pointIsInPoly = False
			//    Else
			//        pointIsInPoly = True
			//    End If
			//
			//



			//    Dim Inside As Boolean
			//
			//    With pData(polyID)
			//        nPol = UBound(.Points) ' Number of points
			//
			//        j = nPol ' Starts at the last point
			//        For i = 1 To nPol
			//            If .Points(j).Y - .Points(i).Y > 0 Then
			//                If ((((.Points(i).Y <= Y) And (Y < .Points(j).Y)) Or _
			//'                    ((.Points(j).Y <= Y) And (Y < .Points(i).Y))) And _
			//'                    (X < (.Points(j).X - .Points(i).X) * (Y - .Points(i).Y) / (.Points(j).Y - .Points(i).Y) + .Points(i).X)) Then
			//                        Inside = Not Inside
			//                End If
			//            End If
			//            j = i
			//        Next
			//    End With
			//
			//    pointIsInPoly = Inside

			//int pnpoly(int npol, float *xp, float *yp, float x, float y)
			//    {
			//      int i, j, c = 0;
			//      for (i = 0, j = npol-1; i < npol; j = i++) {
			//        if ((((yp[i] <= y) && (y < yp[j])) ||
			//             ((yp[j] <= y) && (y < yp[i]))) &&
			//            (x < (xp[j] - xp[i]) * (y - yp[i]) / (yp[j] - yp[i]) + xp[i]))
			//          c = !c;
			//      }
			//      return c;
			//    }


			return result;
		}

		internal static object getPolyBounds(int polyID, ref double minX, ref double minY, ref double maxX, ref double maxY)
		{


			minX = 1000000;
			minY = 1000000;
			maxX = 0;
			maxY = 0;

			// Calculate the extents
			int tempForEndVar = pData[polyID].Points.GetUpperBound(0);
			for (int j = 1; j <= tempForEndVar; j++)
			{
				minX = GeneralFunctions.Min(minX, pData[polyID].Points[j].x);
				minY = GeneralFunctions.Min(minY, pData[polyID].Points[j].y);
				maxX = GeneralFunctions.Max(maxX, pData[polyID].Points[j].x);
				maxY = GeneralFunctions.Max(maxY, pData[polyID].Points[j].y);
			}


			return null;
		}

		internal static object getExtents(ref double maxX, ref double maxY, ref double minX, ref double minY)
		{

			bool setMin = false;

			// Calculate the extents
			int tempForEndVar = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				int tempForEndVar2 = pData[i].Points.GetUpperBound(0);
				for (int j = 1; j <= tempForEndVar2; j++)
				{
					if (setMin)
					{
						minX = GeneralFunctions.Min(minX, pData[i].Points[j].x);
						minY = GeneralFunctions.Min(minY, pData[i].Points[j].y);
					}
					else
					{
						setMin = true;
						minX = pData[i].Points[j].x;
						minY = pData[i].Points[j].y;
					}
					maxX = GeneralFunctions.Max(maxX, pData[i].Points[j].x);
					maxY = GeneralFunctions.Max(maxY, pData[i].Points[j].y);
				}
			}


			return null;
		}

		internal static object getExtents(ref double maxX, ref double maxY, ref double minX)
		{
			double tempRefParam2 = 0;
			return getExtents(ref maxX, ref maxY, ref minX, ref tempRefParam2);
		}

		internal static object getExtents(ref double maxX, ref double maxY)
		{
			double tempRefParam3 = 0;
			double tempRefParam4 = 0;
			return getExtents(ref maxX, ref maxY, ref tempRefParam3, ref tempRefParam4);
		}

		internal static bool canPolyFitInside(int smallPoly, int bigPoly)
		{
			// See if smallPoly will fit inside bigPoly

			// In theory, if all of smallPoly's points are inside bigPoly, then the whole poly is inside bigpoly.
			bool result = false;
			int tempForEndVar = pData[smallPoly].Points.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				if (!pointIsInPoly(bigPoly, pData[smallPoly].Points[i].x, pData[smallPoly].Points[i].y))
				{
					// This point is outside.
					return result;
				}
				else
				{
					result = true;
				}
			}



			return result;
		}

		internal static double getPolyArea(int polyID)
		{
			// Get the area of this polygon
			double minX = 0, maxX = 0;
			double minY = 0, maxY = 0;

			getPolyBounds(polyID, ref minX, ref minY, ref maxX, ref maxY);

			// For now, we are just using the bounding box. Todo: proper area calculation
			return (maxX - minX) * (maxY - minY);

		}

		internal static bool pointIsInPolyWithContain(int polyID, double x, double y)
		{

			// Checks if the point is or isn't in the poly and deals with contained poly's also
			OrderedDictionary cl = null;
			if (containList.ContainsKey(polyID))
			{
				cl = (OrderedDictionary) containList[polyID];
			} // A list of polygons that I contain

			bool isIn = pointIsInPoly(polyID, x, y);

			// Check if it's in any of my kids. If so, it could be that it's NOT inside me.
			if (cl != null)
			{
				int tempForEndVar = cl.Count;
				for (int i = 1; i <= tempForEndVar; i++)
				{
					if (pointIsInPolyWithContain((int) cl[i - 1], x, y))
					{
						// It's in my kid.
						return false;
					}
				}
			}

			return isIn;



		}

		internal static void rasterDocument(double yStep, string currentLayer)
		{

			double minX = 0, maxX = 0, maxY = 0, minY = 0;
			pointD[] totalResult = null;
			pointD[] result = null;
			int n = 0;
			int i = 0;
			bool goingRight = false;

			getExtents(ref maxX, ref maxY, ref minX, ref minY);

			// Here's how this works:
			// We draw a line from left to right, and then right to left, through the entire document. All shapes.
			// We create a giant list of all the places where it intersects.
			// And we take that and create a single line with many on/off points.

			double y = minY;

			while(y < maxY)
			{

				totalResult = ArraysHelper.InitializeArray<pointD>(1);

				int tempForEndVar = pData.GetUpperBound(0);
				for (int p = 1; p <= tempForEndVar; p++)
				{
					if (pData[p].ContainedBy == 0 && pData[p].Fillable)
					{



						// Draw a line from the X left to the X right, and fill in every second line segment.
						SVGParse.pointD tempRefParam = Polygons.newPoint(minX - 50, y);
						result = (pointD[]) Polygons.lineIntersectPoly(ref tempRefParam, Polygons.newPoint(maxX + 50, y), p);


						if (result.GetUpperBound(0) > 0)
						{
							// Copy into TotalResult
							n = totalResult.GetUpperBound(0);
							totalResult = ArraysHelper.RedimPreserve(totalResult, new int[]{n + result.GetUpperBound(0) + 1});
							int tempForEndVar2 = result.GetUpperBound(0);
							for (i = 1; i <= tempForEndVar2; i++)
							{
								totalResult[n + i] = result[i];
							}
						}

					}
				}

				if (totalResult.GetUpperBound(0) > 0)
				{

					newLine(currentLayer);

					orderArray(totalResult, goingRight);
					goingRight = !goingRight; // TEMP

					i = 1;
					// Add a beginning point
					addPoint(totalResult[i].x + ((goingRight) ? -0.5d : 0.5d), totalResult[i].y, true);

					while(i <= totalResult.GetUpperBound(0))
					{
						// Start point
						addPoint(totalResult[i].x, totalResult[i].y, i % 2 == 0);
						i++;
					};
					// And an end point
					addPoint(totalResult[i - 1].x + ((goingRight) ? 0.5d : -0.5d), totalResult[i - 1].y, true);

				}

				y += yStep;
				//    frmInterface.Caption = "Progress : " & Round(y / maxY * 100) & " %"
				//    DoEvents
				//'End If


			};




		}

		internal static void rasterLinePoly(int lineID, double yStep, string currentLayer)
		{

			// Fill this polygon with raster lines from top to bottom

			double maxX = 0, maxY = 0;
			double minX = 0, minY = 0;
			double x = 0;
			pointD[] result = null;

			int i = 0;
			pointD lastPoint = new pointD();
			string cap = frmInterface.DefInstance.Text;



			bool goingRight = false; // The laser moves either left or right. Alternate directions smartly.

			//yStep = 0.008

			// Get the bounds of this shape.

			getPolyBounds(lineID, ref minX, ref minY, ref maxX, ref maxY);

			double y = minY;

			while(y < maxY)
			{

				// Draw a line from the X left to the X right, and fill in every second line segment.
				SVGParse.pointD tempRefParam = Polygons.newPoint(-10, y);
				result = (pointD[]) Polygons.lineIntersectPoly(ref tempRefParam, Polygons.newPoint(maxX + 50, y), lineID);

				if (result.GetUpperBound(0) > 0)
				{

					orderArray(result, goingRight);
					goingRight = !goingRight;
					i = 1;

					while(i <= result.GetUpperBound(0))
					{


						// Start point
						if (i + 1 <= result.GetUpperBound(0))
						{
							newLine(currentLayer);
							addPoint(result[i].x, result[i].y);
							addPoint(result[i + 1].x, result[i + 1].y);
						}

						i += 2;
					};
				}
				//TEMP
				//yStep = yStep * 1.05
				y += yStep;

				//If CLng(Y) Mod 10 = 0 Then
				//frmInterface.Caption = "Progress : " & Round(y / maxY * 100) & " %"
				Application.DoEvents();
				//End If

			};

		}

		internal static pointD[] lineThroughPolygon(int polyID, pointD startPoint, pointD endPoint)
		{

			// Return an array of line segments to draw with this line
			pointD[] out_Renamed = null;
			pointD[] draw = null;
			OrderedDictionary cl = null;
			if (containList.ContainsKey(polyID))
			{
				cl = (OrderedDictionary) containList[polyID];
			} // A list of polygons that I contain


			pointD[] result = (pointD[]) Polygons.lineIntersectPoly(ref startPoint, endPoint, polyID);


			out_Renamed = ArraysHelper.RedimPreserve(out_Renamed, new int[]{out_Renamed.GetUpperBound(0) + 2});
			out_Renamed = ArraysHelper.RedimPreserve(out_Renamed, new int[]{out_Renamed.GetUpperBound(0) + 3});
			out_Renamed = ArraysHelper.RedimPreserve(out_Renamed, new int[]{out_Renamed.GetUpperBound(0) + 3});
			out_Renamed = ArraysHelper.InitializeArray<pointD>(1);
			out_Renamed = ArraysHelper.InitializeArray<pointD>(2);
			if (result.GetUpperBound(0) == 0)
			{ //No intersections

				// Return just the segment unchanged
				out_Renamed[0] = startPoint;
				out_Renamed[1] = endPoint;
			}
			else
			{
				// Build a new set of lines based on the result.

				// Order the points from left to right
				orderArray(result, true);

				// THIS array should be odd!

				out_Renamed[0] = startPoint;
				int tempForEndVar = result.GetUpperBound(0);
				for (int i = 1; i <= tempForEndVar; i += 2)
				{
					if (i + 1 <= result.GetUpperBound(0))
					{

						// Check the kids of this shape.
						if (cl != null)
						{
							for (int K = 1; K <= 1; K++)
							{ //cl.count
								draw = (pointD[]) lineThroughPolygon((int) cl[K - 1], result[i], result[i + 1]);

								// Add this
								int tempForEndVar3 = draw.GetUpperBound(0);
								for (int k2 = 0; k2 <= tempForEndVar3; k2 += 2)
								{
									if (k2 + 1 <= draw.GetUpperBound(0))
									{
										out_Renamed[out_Renamed.GetUpperBound(0) - 1] = draw[k2];
										out_Renamed[out_Renamed.GetUpperBound(0)] = draw[k2 + 1];
									}
								}
							}
						}
						else
						{
							// Add two points
							out_Renamed[out_Renamed.GetUpperBound(0) - 1] = result[i];
							out_Renamed[out_Renamed.GetUpperBound(0)] = result[i + 1];
						}


					}
				}
				// Last point
				out_Renamed[out_Renamed.GetUpperBound(0)] = endPoint;
			}


			return out_Renamed;

		}

		internal static object orderArray(pointD[] inRes, bool Ascending)
		{

			// Order the return array of points.
			double b = 0;
			bool sorted = false;
			do 
			{
				sorted = false;
				int tempForEndVar = inRes.GetUpperBound(0) - 1;
				for (int i = 1; i <= tempForEndVar; i++)
				{

					if ((inRes[i].x > inRes[i + 1].x && !Ascending) || (inRes[i].x < inRes[i + 1].x && Ascending))
					{
						// swap
						b = inRes[i].x;
						inRes[i].x = inRes[i + 1].x;
						inRes[i + 1].x = b;
						sorted = true;
					}
				}
			}
			while(sorted);

			return null;
		}

		internal static object sortByLayers()
		{

			bool sorted = false;
			typLine bb = typLine.CreateInstance();

			do 
			{
				sorted = false;
				int tempForEndVar = pData.GetUpperBound(0) - 1;
				for (int i = 1; i <= tempForEndVar; i++)
				{
					if (String.CompareOrdinal(pData[i].LayerID, pData[i + 1].LayerID) > 0)
					{
						sorted = true;
						bb = pData[i + 1];
						pData[i + 1] = pData[i];
						pData[i] = bb;

					}
				}
			}
			while(sorted);

			return null;
		}

		internal static object mergeConnectedLines()
		{

			int j = 0;
			int iCount = 0;
			bool doMerge = false;
			bool doFlip = false;
			bool didMerge = false;

			// Looks for polygons that begin/end exactly at the beginning/end of another polygon and merges them into one polygon.

			int tempForEndVar = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				pData[i].Optimized = false;
			}

			// Step 2: Loop through the unoptimized polygons
			do 
			{
				didMerge = false;
				int tempForEndVar2 = pData.GetUpperBound(0) - 1;
				for (int i = 1; i <= tempForEndVar2; i++)
				{


					if (!pData[i].Optimized)
					{
						iCount = pData[i].Points.GetUpperBound(0);

						frmInterface.DefInstance.Text = "Optimizing " + i.ToString() + " / " + pData.GetUpperBound(0).ToString();
						if (i % 50 == 0)
						{
							Application.DoEvents();
						}

						doMerge = false;
						int tempForEndVar3 = pData.GetUpperBound(0);
						for (j = 1; j <= tempForEndVar3; j++)
						{
							if (j != i && pData[j].LayerID == pData[i].LayerID)
							{
								if (pData[i].Points[iCount].x == pData[j].Points[1].x && pData[i].Points[iCount].y == pData[j].Points[1].y)
								{

									// OK, this shape starts where my shape ends.
									Debug.WriteLine(Support.TabLayout("SHAPE " + i.ToString() + " AND " + j.ToString() + " X: ", pData[i].Points[iCount].x.ToString(), pData[j].Points[1].x.ToString()));
									Debug.WriteLine(Support.TabLayout("SHAPE " + i.ToString() + " AND " + j.ToString() + " Y: ", pData[i].Points[iCount].y.ToString(), pData[j].Points[1].y.ToString()));

									doMerge = true;
									doFlip = false;
									break;
								}

								if (pData[i].Points[iCount].x == pData[j].Points[pData[j].Points.GetUpperBound(0)].x && pData[i].Points[iCount].y == pData[j].Points[pData[j].Points.GetUpperBound(0)].y)
								{
									// OK, this shape ends where my shape ends.
									doMerge = true;
									doFlip = true; // Since its the end that matched, we need to flip it first.
									break;
								}
							}
						}

						if (doMerge)
						{
							Debug.WriteLine(Support.TabLayout("MERGING SHAPE ", j.ToString(), "INTO ", i.ToString()));
							didMerge = true;
							if (doFlip)
							{ // Flip it around first.
								Polygons.flipPolyStartEnd(j);
							}

							// Merge the points from j into i
							pData[i].Points = ArraysHelper.RedimPreserve(pData[i].Points, new int[]{iCount + pData[j].Points.GetUpperBound(0) + 1});

							int tempForEndVar4 = pData[j].Points.GetUpperBound(0);
							for (int n = 1; n <= tempForEndVar4; n++)
							{
								pData[i].Points[iCount + n] = pData[j].Points[n];
							}
							// Delete shape j since we don't need it anymore
							int tempForEndVar5 = pData.GetUpperBound(0) - 1;
							for (int n = j; n <= tempForEndVar5; n++)
							{
								pData[n] = pData[n + 1];
							}
							pData = ArraysHelper.RedimPreserve(pData, new int[]{pData.GetUpperBound(0)});

							// Then start the loop again.
							Debug.WriteLine(Support.TabLayout("COUNT IS NOW ", pData.GetUpperBound(0).ToString()));
							break; // Start the loop again
						}
						else
						{
							// Alright we're done with this one
							pData[i].Optimized = true;
						}
					}
				}
			}
			while(didMerge); // Continue looping until there's no more merging

			// Finally, look for polygons that have a start and end point at the same co-ordinate and mark them as fillable.
			int tempForEndVar6 = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar6; i++)
			{
				if (pData[i].Points[1].x == pData[i].Points[pData[i].Points.GetUpperBound(0)].x && pData[i].Points[1].y == pData[i].Points[pData[i].Points.GetUpperBound(0)].y)
				{

					// End of shape matches start
					// Therefore it is fillable.
					pData[i].Fillable = true;

				}
			}

			return null;
		}

		internal static object optimizePolys()
		{



			double dist = 0;
			double bestDist = 0;
			int bestLine = 0;
			bool bestIsEnd = false; // Is the best match actually the END of another line?


			// Run through the list of polygons. Order them so that when we reach the end of one,
			// we immediately find the nearest next line.

			// Step 1: Mark all of the polygons as "unordered"


			int tempForEndVar = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				pData[i].Optimized = false;
			}


			// Step 2: Loop through the unoptimized polygons
			int tempForEndVar2 = pData.GetUpperBound(0) - 1;
			for (int i = 1; i <= tempForEndVar2; i++)
			{
				if (!pData[i].Optimized)
				{

					frmInterface.DefInstance.Text = "Optimizing " + i.ToString() + " / " + pData.GetUpperBound(0).ToString();
					if (i % 50 == 0)
					{
						Application.DoEvents();
					}

					// Find the next polygon that ends nearest this one.
					bestDist = 10000000;
					bestLine = 0;


					int tempForEndVar3 = pData.GetUpperBound(0);
					for (int j = 1; j <= tempForEndVar3; j++)
					{
						if (j != i && !pData[j].Optimized && pData[j].LayerID == pData[i].LayerID)
						{
							// Calculate the distance
							dist = Polygons.pointDistance(pData[i].Points[pData[i].Points.GetUpperBound(0)], pData[j].Points[1]);
							if (dist < bestDist)
							{
								bestDist = dist;
								bestLine = j;
								bestIsEnd = false;
							}

							// Try the End of the line, since the line can be flipped if this makes more sense
							dist = Polygons.pointDistance(pData[i].Points[pData[i].Points.GetUpperBound(0)], pData[j].Points[pData[j].Points.GetUpperBound(0)]);
							if (dist < bestDist)
							{
								bestDist = dist;
								bestLine = j;
								bestIsEnd = true;
							}

						}
					}

					// Now we know which line is best to go NEXT.
					// So, move this line so that it is the next line after this one.
					if (bestLine > 0)
					{

						if (bestIsEnd)
						{
							// We've got to flip the line around, since it's END point is closest to our end.
							Polygons.flipPolyStartEnd(bestLine);
						}

						// For now, we just swap the desired line with the next one.
						SwapLine(ref pData[i + 1], ref pData[bestLine]);


					}

					//Mark ourselves as optimized
					pData[i].Optimized = true;

				}
			}

			return null;
		}

		internal static void SwapLine(ref typLine A, ref typLine b)
		{
			typLine c = typLine.CreateInstance();
			A = b;
			b = c;

		}

		internal static object exportGCODE(string outFile, double feedRate, bool PlungeZ, bool PPIMode, int PPIVal, bool LoopMode, int Loops, double RaiseDist)
		{


			// Export GCODE!
			StringBuilder t = new StringBuilder();




			int f = FileSystem.FreeFile();
			// Draw the lines.

			if (FileSystem.Dir(outFile, FileAttribute.Normal) != "")
			{
				File.Delete(outFile);
			}
			FileSystem.FileOpen(f, outFile, OpenMode.Append, OpenAccess.Default, OpenShare.Default, -1);


			// Get the extents


			bool isDefocused = false;
			bool wasDefocused = false;

			int cutCount = 0; // Defocusde cuts cut the same thing many times





			double maxX = EXPORT_EXTENTS_X;
			double maxY = EXPORT_EXTENTS_Y;


			// Make it 5 inches high
			int scalar = 1;
			//scalar = 0.01


			// Go to the corners
			FileSystem.PrintLine(f, "G20 (Units are in Inches)");
			FileSystem.PrintLine(f, "F" + StringsHelper.Format(feedRate, "0.00000"));
			FileSystem.PrintLine(f, "G61 (Go to exact corners)"); // Added Sep 21, 2016

			if (PPIMode)
			{
				FileSystem.PrintLine(f, "S" + PPIVal.ToString() + " (PPI mode with this many pulses per inch)");
			}

			if (LoopMode)
			{

				FileSystem.PrintLine(f, "#201 = " + Loops.ToString() + " (number of passes)");
				FileSystem.PrintLine(f, "#200 = " + StringsHelper.Format(RaiseDist * 0.0393701d, "0.000000") + " (move the bed up incrementally by this much in inches)");
				FileSystem.PrintLine(f, "#300 = 0 (bed movement distance storage variable)");
				FileSystem.PrintLine(f, "#100 = 1 (layer number storage variable)");

				FileSystem.PrintLine(f, "G1 W0.00000 (make sure bed is 0.0000 before you cut first pass)");
				FileSystem.PrintLine(f, "o101 WHILE [#100 LE #201] (the number of passes is that the number after LE, LE = less or equal to)");

			}


			// Turn on the spindle
			//Print #f, "M3 S1"

			//Print #F, "G1 X0 Y0"
			//Print #F, "G1 X" & Round(maxX * scalar, 5) & " Y0"
			//Print #F, "G1 X" & Round(maxX * scalar, 5) & " Y" & Round(maxY * scalar, 5)
			//Print #F, "G1 X0 Y" & Round(maxY * scalar, 5)

			string tLayer = "---";

			int tempForEndVar = pData.GetUpperBound(0);
			bool lastCutting = false;
			for (int i = 1; i <= tempForEndVar; i++)
			{
				if (pData[i].Points.GetUpperBound(0) > 0)
				{
					// Set the feed rate.
					//greyLevel = .greyLevel / GREYLEVELS
					//Print #f, "F" & CLng((maxFeedRate - minFeedRate) * greyLevel) + minFeedRate

					if (pData[i].LayerID != "Cut Boxes")
					{

						if (tLayer != pData[i].LayerID)
						{

							wasDefocused = isDefocused;
							isDefocused = false;
							if (layerInfo.ContainsKey(pData[i].LayerID))
							{

								if (ReflectionHelper.Invoke<bool>(layerInfo[pData[i].LayerID], "Exists", new object[]{"pausebefore"}))
								{
									FileSystem.PrintLine(f, "(MSG,Change Laser Power!)");
									FileSystem.PrintLine(f, "M0");
								}

								// Are we defocused on this layer?
								if (ReflectionHelper.Invoke<bool>(layerInfo[pData[i].LayerID], "Exists", new object[]{"defocused"}))
								{
									isDefocused = true;

									// Bring it down
									FileSystem.PrintLine(1, "F100 (Increated feed rate for defocused cuts)");
									FileSystem.PrintLine(1, "G0 W-" + ReflectionHelper.GetPrimitiveValue<string>(((Array) layerInfo[pData[i].LayerID]).GetValue(Convert.ToInt32(Double.Parse("defocused")))));

								}

							}

							if (wasDefocused && !isDefocused)
							{
								// Bring the W back up
								FileSystem.PrintLine(1, "G0 W0");
								// Reset the feed rate
								FileSystem.PrintLine(f, "F" + StringsHelper.Format(feedRate, "0.00000"));
							}

							tLayer = pData[i].LayerID;
						}


						lastCutting = false;
						cutCount = 1;
						if (isDefocused)
						{
							cutCount = 20;
						}


						int tempForEndVar2 = cutCount;
						for (int cuts = 1; cuts <= tempForEndVar2; cuts++)
						{

							int tempForEndVar3 = pData[i].Points.GetUpperBound(0);
							for (int j = 1; j <= tempForEndVar3; j++)
							{

								if (j == 1)
								{ // First point, just GO there.
									FileSystem.PrintLine(f, "G0 X" + StringsHelper.Format(pData[i].Points[j].x * scalar, "0.00000") + " Y" + StringsHelper.Format((maxY - pData[i].Points[j].y) * scalar, "0.00000"));
									//Print #f, "G1 z-0.0010"

									// Turn on the spindle
									if (PPIMode)
									{
										FileSystem.PrintLine(f, "M3");
									}
									else
									{
										FileSystem.PrintLine(f, "M3 S1");
									}
									//Print #f, "G0 Z -0.0100"
								}
								else
								{
									t = new StringBuilder("G1 X" + StringsHelper.Format(pData[i].Points[j].x * scalar, "0.00000") + " Y" + StringsHelper.Format((maxY - pData[i].Points[j].y) * scalar, "0.00000"));

									// Are we CUTTING to this point, or not?
									if (lastCutting && pData[i].Points[j - 1].noCut == 1)
									{

										if (PlungeZ)
										{
											FileSystem.PrintLine(f, "G0 Z 0.2");
										}
										else
										{
											t.Append(" M63 P0"); // STOP cutting
										}


										lastCutting = false;
									}
									else if (!lastCutting && pData[i].Points[j - 1].noCut == 0)
									{ 

										if (PlungeZ)
										{
											FileSystem.PrintLine(f, "G0 Z -0.5");
										}
										else
										{
											t.Append(" M62 P0"); // START cutting
										}

										lastCutting = true;
									}
									FileSystem.PrintLine(f, t.ToString());
								}
							}

							if (isDefocused)
							{
								// Run the same line backwards again
								for (int j = pData[i].Points.GetUpperBound(0); j >= 1; j--)
								{
									if (j == pData[i].Points.GetUpperBound(0))
									{ // First point, just GO there.
										FileSystem.PrintLine(f, "G0 X" + StringsHelper.Format(pData[i].Points[j].x * scalar, "0.00000") + " Y" + StringsHelper.Format((maxY - pData[i].Points[j].y) * scalar, "0.00000"));
									}
									else
									{
										t = new StringBuilder("G1 X" + StringsHelper.Format(pData[i].Points[j].x * scalar, "0.00000") + " Y" + StringsHelper.Format((maxY - pData[i].Points[j].y) * scalar, "0.00000"));
										if (lastCutting && pData[i].Points[j - 1].noCut == 1)
										{
											t.Append(" M63 P0"); // STOP cutting
											lastCutting = false;
										}
										else if (!lastCutting && pData[i].Points[j - 1].noCut == 0)
										{ 
											t.Append(" M62 P0"); // START cutting
											lastCutting = true;
										}
										FileSystem.PrintLine(f, t.ToString());
									}
								}
							}
						}

						//Print #F, "G0 Z0.0010"
						// Turn off the spindle
						FileSystem.PrintLine(f, "M5");
						if (PlungeZ)
						{
							FileSystem.PrintLine(f, "G0 Z 0.2");
						}

						//Print #f, "G0 Z 0.0100"

						//Print #f, "G1 Z0.0010"
						FileSystem.PrintLine(f, "");
					}

				}

			}

			FileSystem.PrintLine(f, "M5");
			if (PlungeZ)
			{
				FileSystem.PrintLine(f, "G0 Z 0.2");
			}

			if (LoopMode)
			{
				FileSystem.PrintLine(f, "#300 = [#200*#100]");
				FileSystem.PrintLine(f, "G1 W#300 (move the bed up according to the layer its on)");
				FileSystem.PrintLine(f, "#100 = [#100+1] (add one to the layer counter)");
				FileSystem.PrintLine(f, "o101 ENDWHILE");
			}

			FileSystem.PrintLine(f, "M30");
			FileSystem.FileClose(f);

			return null;
		}

		internal static object MoveLayerToEnd(string LayerID)
		{
			// Make a new list of just the lines not in this layer, then put these at the end

			typLine[] pNew = null;
			pNew = ArraysHelper.InitializeArray<typLine>(1);
			int j = 0;
			int n = 0;
			int tempForEndVar = pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				if (pData[i].LayerID == LayerID)
				{
					// Put this aside
					n = pNew.GetUpperBound(0) + 1;
					pNew = ArraysHelper.RedimPreserve(pNew, new int[]{n + 1});
					pNew[n] = pData[i];
				}
				else
				{
					j++;
					pData[j] = pData[i];
				}
			}

			// Now add to end
			int tempForEndVar2 = n;
			for (int i = 1; i <= tempForEndVar2; i++)
			{
				j++;
				pData[j] = pNew[i];
			}

			// All done


			return null;
		}
	}
}