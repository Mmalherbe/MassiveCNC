using UnityEngine;
using Chilkat;
public class svgParser : MonoBehaviour
{
	public static typLine[] pData = null;
	public static int currentLine = 0;
	public static float GLOBAL_DPI = 0;
	static bool hasUnfinishedLine = false;

	#region structs

	public struct pointD
	{
		public float x;
		public float y;
		public byte noCut;
	}

	public struct typLine
	{
		public pointD[] Points;
		public int SpecialNumPoints;

		public bool Fillable; // Only works for closed paths

		public int ContainedBy; // ID to containing poly

		public float xCenter;
		public float yCenter;

		public bool Optimized;

		public byte greyLevel; // 0 to GREYLEVELS level of grey, higher is lighter

		public string LayerID;

		public string PathCode;

		public int LevelNumber; //How many levels deep is this

		public bool isDel; // Deleted on next iteration
		public static typLine CreateInstance()
		{
			typLine result = new typLine();
			result.LayerID = string.Empty;
			result.PathCode = string.Empty;
			return result;
		}
	}
	#endregion

	#region helperfunctions
	internal static string extractToken(string inPath, ref int pos)
	{

		// Exract until we get a space or a comma
		string char_Renamed = "";
		StringBuilder build = new StringBuilder();
		bool seenMinus = false;
		bool seenE = false;
		bool seenPeriod = false;
		int startPos = pos;



		while (pos <= inPath.Length)
		{
			char_Renamed = inPath[pos].ToString();

			switch (char_Renamed)
			{
				// Only accept numbers
				case ".":  // A period can be seen anywhere in the number, but if a second period is found it means we must exit 
					if (seenPeriod)
					{
						return build.ToString();
					}
					else
					{
						seenPeriod = true;
						build.Append(char_Renamed);
						pos++;
					}

					break;
				case "-":
					if (seenE)
					{
						build.Append(char_Renamed);
						pos++;
					}
					else if (seenMinus || pos > startPos)
					{
						return build.ToString();
					}
					else
					{
						// We already saw a minus sign
						seenMinus = true;
						build.Append(char_Renamed);
						pos++;
					}

					break;
				case "0":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
					build.Append(char_Renamed);
					pos++;
					//,6.192 -10e-4,12.385 
					break;
				case "e":  // Exponent 
					seenE = true;
					build.Append(char_Renamed);
					pos++;
					break;
				default:
					return build.ToString();
			}
		};
		return build.ToString();
	}
	internal static object finishLine()
	{
		if (hasUnfinishedLine)
		{
			hasUnfinishedLine = false;

			// Remove the excess
			pData[currentLine].Points = ArraysHelper.RedimPreserve(pData[currentLine].Points, new int[] { pData[currentLine].SpecialNumPoints + 1 });
		}

		return null;
	}
	internal static object newLine(string theLayer = "")
	{

		if (hasUnfinishedLine)
		{
			finishLine();
		}



		currentLine = pData.GetUpperBound(0) + 1;
		// Set up this line
		pData = ArraysHelper.RedimPreserve(pData, new int[] { currentLine + 1 });
		pData[currentLine].Points = new pointD[1];

		pData[currentLine].LayerID = theLayer;


		return null;
	}

	internal static object multiplyLineByMatrix(int polyID, float A, float b, float c, float D, float e, float f)
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

	internal static object transformLine(int lineID, string transformText)
	{

		// Parse the transform text
		int e = 0, f = 0;

		string func = "";
		string params_Renamed = "";
		string[] pSplit = null;
		float Ang = 0;



		e = (transformText.IndexOf('(') + 1);
		if (e > 0)
		{
			func = transformText.Substring(0, Mathf.Min(e - 1, transformText.Length));
			f = Strings.InStr(e + 1, transformText, ")", CompareMethod.Binary);
			if (f > 0)
			{
				params_Renamed = transformText.Substring(e, Mathf.Min(f - e - 1, transformText.Length - e));
			}

			switch (func.ToLower())
			{
				case "translate":
					// Just move everything 
					pSplit = (string[])params_Renamed.Split(',');

					// Translate is 
					// [ 1  0  tx ] 
					// [ 0  1  ty ] 
					// [ 0  0  1  ] 

					if (pSplit.GetUpperBound(0) == 0)
					{
						multiplyLineByMatrix(lineID, 1, 0, 0, 1, float.Parse(pSplit[0]), 0);
					}
					else
					{
						multiplyLineByMatrix(lineID, 1, 0, 0, 1, float.Parse(pSplit[0]), float.Parse(pSplit[1]));
					}

					break;
				case "matrix":
					pSplit = (string[])params_Renamed.Split(',');
					if (pSplit.GetUpperBound(0) == 0)
					{
						pSplit = (string[])params_Renamed.Split(' ');
					}
					multiplyLineByMatrix(lineID, float.Parse(pSplit[0]), float.Parse(pSplit[1]), float.Parse(pSplit[2]), float.Parse(pSplit[3]), float.Parse(pSplit[4]), float.Parse(pSplit[5]));

					break;
				case "rotate":

					pSplit = (string[])params_Renamed.Split(',');
					Ang = Mathf.Deg2Rad * float.Parse(pSplit[0]);

					multiplyLineByMatrix(lineID, Mathf.Cos(Ang), Mathf.Sin(Ang), -Mathf.Sin(Ang), Mathf.Cos(Ang), 0, 0);

					break;
				case "scale":  // scale(-1,-1) 
					pSplit = (string[])params_Renamed.Split(',');
					if (pSplit.GetUpperBound(0) == 0)
					{
						pSplit = (string[])params_Renamed.Split(' ');
					}
					if (pSplit.GetUpperBound(0) == 0)
					{
						// Handle shitty SVG, such as not having two parameters
						pSplit = ArraysHelper.RedimPreserve(pSplit, new int[] { 2 });
						pSplit[1] = pSplit[0];
					}
					multiplyLineByMatrix(lineID, float.Parse(pSplit[0]), 0, 0, float.Parse(pSplit[1]), 0, 0);


					break;
			}

		}

		return null;
	}
	#endregion

	#region Parsers
	internal static object parsePath(ref string inPath, string currentLayer)
	{




		// Parse an SVG path.

		string token3 = "", token1 = "", token2 = "", token4 = "";
		string token7 = "", token5 = "", token6 = "";


		bool isRelative = false;
		bool gotFirstItem = false;

		float currX = 0;
		float currY = 0;

		pointD pt0 = new pointD();
		pointD pt1 = new pointD();
		pointD pt2 = new pointD();
		pointD pt3 = new pointD();
		pointD pt4 = new pointD();
		pointD pt5 = new pointD();

		pointD ptPrevPoint = new pointD();
		bool hasPrevPoint = false;

		int lastUpdate = 0;





		float startX = 0;
		float startY = 0;

		float pInSeg = 0;
		char lastChar = char.Parse("") ;



		//M209.1,187.65c-0.3-0.2-0.7-0.4-1-0.4c-0.3,0-0.7,0.2-0.9,0.4c-0.3,0.3-0.4,0.6-0.4,0.9c0,0.4,0.1,0.7,0.4,1
		//c0.2,0.2,0.6,0.4,0.9,0.4c0.3,0,0.7-0.2,1-0.4c0.2-0.3,0.3-0.6,0.3-1C209.4,188.25,209.3,187.95,209.1,187.65z

		// Get rid of enter presses
		inPath = inPath.Replace( "\r", " ");
		inPath = inPath.Replace( "\n", " ");
		inPath = inPath.Replace( "\t", " ");


		for(int pos = 0; pos < inPath.Length; pos++)
		{
			switch (inPath[pos].ToString())
			{
				case "M":
				case "m":
				case "L":
				case "l":
				case "C":
				case "c":
				case "V":
				case "v":
				case "A":
				case "a":
				case "H":
				case "h":
				case "S":
				case "s":
				case "Z":
				case "z":
				case "q":
				case "Q":
				case "T":
				case "t":
					// Accepted character. 
					lastChar = inPath[pos]; 
					break;
				case " ":

					break;
				default:
					// No accepted, must be a continuation. 
					inPath = rebuildString(inPath, pos, lastChar);
				
					if (inPath[pos] == char.Parse("m"))
					{
						inPath = rebuildString(inPath, pos, char.Parse("l"));
						
					}  // Continuous moveto becomes lineto 
					if (inPath[pos] == char.Parse("M"))
					{
						inPath = rebuildString(inPath, pos, char.Parse("L"));
					}  // Continuous moveto becomes lineto not relative 
					pos--;
					break;
			}

			switch (inPath[pos].ToString())
			{
				case " ":  // Skip spaces 

					break;
				case "M":
				case "m":  // MOVE TO 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
					{
						isRelative = true;
					}  // Lowercase means relative co-ordinates 
					if (!gotFirstItem)
					{
						isRelative = false;
					}  //Relative not valid for first item 


					// Extract two co-ordinates 
					inPath = inPath.Replace("\r", " ");
					inPath = inPath.Replace("\n", " ");
					inPath = inPath.Replace("\t", " ");
					token1 = extractToken(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set our "current" co-ordinates to this 
					if (isRelative)
					{
						currX += float.Parse(token1);
						currY += float.Parse(token2);
					}
					else
					{
						currX = float.Parse(token1);
						currY = float.Parse(token2);
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
				case "L":
				case "l":  // LINE TO 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
					{
						isRelative = true;
					}  // Lowercase means relative co-ordinates 
					if (!gotFirstItem)
					{
						isRelative = false;
					}  //Relative not valid for first item 


					// Extract two co-ordinates 
					
					token1 = extractToken(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set our "current" co-ordinates to this 
					if (isRelative)
					{
						currX += float.Parse(token1);
						currY += float.Parse(token2);
					}
					else
					{
						currX = float.Parse(token1);
						currY = float.Parse(token2);
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
				case "V":
				case "v":  // VERTICAL LINE TO 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
					{
						isRelative = true;
					}  // Lowercase means relative co-ordinates 
					if (!gotFirstItem)
					{
						isRelative = false;
					}  //Relative not valid for first item 

					// Extract one co-ordinate 
					
					token1 = extractToken(inPath, ref pos);

					// Set our "current" co-ordinates to this 
					if (isRelative)
					{
						currY += float.Parse(token1);
					}
					else
					{
						currY = float.Parse(token1);
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
				case "H":
				case "h":  // HORIZONTAL LINE TO 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
					{
						isRelative = true;
					}  // Lowercase means relative co-ordinates 
					if (!gotFirstItem)
					{
						isRelative = false;
					}  //Relative not valid for first item 

					// Extract one co-ordinate 
					
					token1 = extractToken(inPath, ref pos);

					// Set our "current" co-ordinates to this 
					if (isRelative)
					{
						currX += float.Parse(token1);
					}
					else
					{
						currX = float.Parse(token1);
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
				case "A":
				case "a":  // PARTIAL ARC TO 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
					{
						isRelative = true;
					}  // Lowercase means relative co-ordinates 
					if (!gotFirstItem)
					{
						isRelative = false;
					}  //Relative not valid for first item 

					//(rx ry x-axis-rotation large-arc-flag sweep-flag x y)+ 

					// Radii X and Y 
					
					token1 = extractToken(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// X axis rotation 
					token3 = extractToken(inPath, ref pos);

					// Large arc flag 
					token4 = extractToken(inPath, ref pos);

					// Sweep flag 
					token5 = extractToken(inPath, ref pos);

					// X and y 
					token6 = extractToken(inPath, ref pos);
					token7 = extractToken(inPath, ref pos);

					// Start point 
					pt0.x = currX;
					pt0.y = currY;

					// Set our "current" co-ordinates to this 
					if (isRelative)
					{
						currX += float.Parse(token6);
						currY += float.Parse(token7);
					}
					else
					{
						currX = float.Parse(token6);
						currY = float.Parse(token7);
					}

					pt1.x = currX;
					pt1.y = currY;

					float tempRefParam = float.Parse(token1);
					float tempRefParam2 = float.Parse(token2);
					parseArcSegment(ref tempRefParam, ref tempRefParam2, float.Parse(token3), pt0, pt1, token4 == "1", token5 == "1");

					//pData(currentLine).PathCode = pData(currentLine).PathCode & "Partial Arc to " & currX & ", " & currY & vbCrLf 

					if (!gotFirstItem)
					{
						startX = currX;
						startY = currY;
					}
					gotFirstItem = true;
					hasPrevPoint = false;

					break;
				case "C":
				case "c":  // CURVE TO 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);


					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					pt2.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt2.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 2 
					currX = ((isRelative) ? currX : 0) + float.Parse(token1);
					currY = ((isRelative) ? currY : 0) + float.Parse(token2);
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
				case "S":
				case "s":  // CURVE TO with 3 points 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					currX = ((isRelative) ? currX : 0) + float.Parse(token1);
					currY = ((isRelative) ? currY : 0) + float.Parse(token2);
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
				case "Q":
				case "q":  // Quadratic Bezier TO with 3 points 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					currX = ((isRelative) ? currX : 0) + float.Parse(token1);
					currY = ((isRelative) ? currY : 0) + float.Parse(token2);
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
				case "T":
				case "t":  // Quadratic Bezier TO with 3 points, but use reflection of last 
					if (inPath[pos].ToString().ToLower() == inPath[pos].ToString())
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

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
				case "z":
				case "Z":

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
					Debug.Log("UNSUPPORTED PATH CODE: "+ inPath[pos].ToString());


					break;
			}

		}
		for(int pos = 0; pos < inPath.Length; pos++) 
		{
			isRelative = false;

			switch (inPath[pos].ToString())
			{
				case "M":
				case "m":
				case "L":
				case "l":
				case "C":
				case "c":
				case "V":
				case "v":
				case "A":
				case "a":
				case "H":
				case "h":
				case "S":
				case "s":
				case "Z":
				case "z":
				case "q":
				case "Q":
				case "T":
				case "t":
					// Accepted character. 
					lastChar = inPath[pos];
					break;
				case " ":

					break;
				default:
					// No accepted, must be a continuation. 
					inPath = rebuildString(inPath, pos, lastChar);
					if (inPath[pos].ToString() == "m")
					{
						inPath = rebuildString(inPath, pos, char.Parse("l"));
					}  // Continuous moveto becomes lineto 
					if (inPath[pos].ToString() == "M")
					{
						inPath = rebuildString(inPath, pos, char.Parse("L"));
					}  // Continuous moveto becomes lineto not relative 
					pos--;
					break;
			}


			switch (inPath[pos].ToString())
			{
				case " ":  // Skip spaces 

					break;
				case "M":
				case "m":  // MOVE TO 
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
						currX += float.Parse(token1);
						currY += float.Parse(token2);
					}
					else
					{
						currX = float.Parse(token1);
						currY = float.Parse(token2);
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
				case "L":
				case "l":  // LINE TO 
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
						currX += float.Parse(token1);
						currY += float.Parse(token2);
					}
					else
					{
						currX = float.Parse(token1);
						currY = float.Parse(token2);
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
				case "V":
				case "v":  // VERTICAL LINE TO 
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
						currY += float.Parse(token1);
					}
					else
					{
						currY = float.Parse(token1);
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
				case "H":
				case "h":  // HORIZONTAL LINE TO 
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
						currX += float.Parse(token1);
					}
					else
					{
						currX = float.Parse(token1);
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
				case "A":
				case "a":  // PARTIAL ARC TO 
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
						currX += float.Parse(token6);
						currY += float.Parse(token7);
					}
					else
					{
						currX = float.Parse(token6);
						currY = float.Parse(token7);
					}

					pt1.x = currX;
					pt1.y = currY;

					float tempRefParam = float.Parse(token1);
					float tempRefParam2 = float.Parse(token2);
					parseArcSegment(ref tempRefParam, ref tempRefParam2, float.Parse(token3), pt0, pt1, token4 == "1", token5 == "1");

					//pData(currentLine).PathCode = pData(currentLine).PathCode & "Partial Arc to " & currX & ", " & currY & vbCrLf 
						
					if (!gotFirstItem)
					{
						startX = currX;
						startY = currY;
					}
					gotFirstItem = true;
					hasPrevPoint = false;

					break;
				case "C":
				case "c":  // CURVE TO 
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);


					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					pt2.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt2.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 2 
					currX = ((isRelative) ? currX : 0) + float.Parse(token1);
					currY = ((isRelative) ? currY : 0) + float.Parse(token2);
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
				case "S":
				case "s":  // CURVE TO with 3 points 
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					currX = ((isRelative) ? currX : 0) + float.Parse(token1);
					currY = ((isRelative) ? currY : 0) + float.Parse(token2);
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
				case "Q":
				case "q":  // Quadratic Bezier TO with 3 points 
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					skipWhiteSpace(inPath, ref pos);
					token1 = extractToken(inPath, ref pos);
					skipWhiteSpace(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					currX = ((isRelative) ? currX : 0) + float.Parse(token1);
					currY = ((isRelative) ? currY : 0) + float.Parse(token2);
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
				case "T":
				case "t":  // Quadratic Bezier TO with 3 points, but use reflection of last 
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
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

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
				case "z":
				case "Z":

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


	internal static string rebuildString(string Text, int pos,char newChar)
	{
		string NewText = string.Empty;
		for(int i=0; i< Text.Length; i++)
		{
			if(i != pos)
			{
				NewText += Text[i];
			}
			else
			{
				NewText += newChar;
			}
		}
		return NewText;
	}

	internal static object addPoint(float x, float y, bool noCutLineSegment = false)
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
					pData[currentLine].Points = ArraysHelper.RedimPreserve(pData[currentLine].Points, new int[] { pData[currentLine].Points.GetUpperBound(0) + 5001 });
				}

			}
			else
			{
				n = pData[currentLine].Points.GetUpperBound(0) + 1;
				pData[currentLine].Points = ArraysHelper.RedimPreserve(pData[currentLine].Points, new int[] { n + 1 });
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

	#endregion
	internal static string getAttr(Chilkat.Xml attr, string attrName, string DefaultValue = "")
	{

		return attr.GetAttrValue(attrName);

	}
	internal static object parseSVG(string inFile)
	{

		Chilkat.Xml SVG = new Chilkat.Xml();
		Chilkat.Xml x = null;

		float realW = 0;
		float realH = 0;

		string[] S = null;


		pData = new typLine[1];
		currentLine = 0;

		float realDPI = 90;

		SVG.LoadXmlFile(inFile);

		if (SVG == null)
		{
			Debug.Log("Could not load SVG");
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

			realW = float.Parse((SVG.GetAttrValue("width")));
			// Read the unit
			/*
			switch (string.Replace(SVG.GetAttrValue("width"), realW.ToString(), "", 1, -1, CompareMethod.Binary).ToLower())
			{
				case "in":  // no conversion needed 
					break;
				case "mm":
				case "":  // convert from mm 
					realW /= 25.4d;
					break;
				case "cm":  // convert from cm 
					realW /= 2.54d;

					break;
			} */

			realH = float.Parse(SVG.GetAttrValue("height"));
			// Read the unit
			/*
			switch (Strings.Replace(SVG.GetAttrValue("height"), realH.ToString(), "", 1, -1, CompareMethod.Binary).ToLower())
			{
				case "in":  // no conversion needed 
					break;
				case "mm":
				case "":  // convert from mm 
					realH /= 25.4d;
					break;
				case "cm":  // convert from cm 
					realH /= 2.54d;
					break;
			}
			*/
			//MsgBox "Size in inches: " & realW & ", " & realH

			// The 'ViewBox' is how we scale an inch to a pixel.  The default is 90dpi but it may not be.

			//ttt = InputBox("Detected with: " & realW & " inches.  Change it?", "Width", realW)
			//If ttt <> "" Then
			//    realW = Val(ttt)
			//End If


			S = (string[])SVG.GetAttrValue("viewBox").Split(' ');
			if (S.GetUpperBound(0) == 3)
			{
				// Get the width in pixels
				if (realW == 0)
				{
					realDPI = 300;
				}
				else
				{
					realDPI = float.Parse(S[2]) / realW;
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

		float minX = 1000000;
		float minY = 1000000;

		// Calculate the extents
		int tempForEndVar3 = pData.GetUpperBound(0);
		for (int i = 1; i <= tempForEndVar3; i++)
		{
			int tempForEndVar4 = pData[i].Points.GetUpperBound(0);
			for (int j = 1; j <= tempForEndVar4; j++)
			{
				minX = Mathf.Min(minX, float.Parse(pData[i].Points[j].x.ToString())); 
				minY = Mathf.Min(minY, float.Parse(pData[i].Points[j].y.ToString())) ;
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


	internal static object parseSVGKids(Chilkat.Xml inEle, ref string currentLayer)
	{

		// Loop through my kids and figure out what to do!
		int beforeLine = 0;

		float cX = 0;
		float cY = 0;
		float cW = 0;
		float cH = 0;

		int beforeGroup = 0;
		string layerName = "";

		if (currentLayer == "")
		{
			currentLayer = "BLANK";
		}


		Debug.Log(("PARSING A KIDS:", currentLayer));


		Chilkat.Xml x = (Chilkat.Xml)inEle.FirstChild();
		string thePath = "";

		while (x != null)
		{

			//MsgBox X.nodeName

			switch (x.Tag.ToLower())
			{
				case "g":  // g is GROUP 
					beforeGroup = currentLine;

					// Is this group a layer? 
					layerName = getAttr(x, "inkscape:label", "");
					if (layerName == "")
					{
						if (getAttr(x, "id", "").IndexOf("layer", System.StringComparison.CurrentCultureIgnoreCase) >= 0)
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
				case "switch":  // stupid crap 
					parseSVGKids(x);

					// SHAPES 
					break;
				case "rect":
				case "path":
				case "line":
				case "polyline":
				case "circle":
				case "polygon":
				case "ellipse":
					beforeLine = currentLine;

					switch (x.Tag.ToLower())
					{
						case "rect":  // RECTANGLE 

							newLine(currentLayer);
							cX = float.Parse(getAttr(x, "x", ""));
							cY = float.Parse(getAttr(x, "y", ""));
							cW = float.Parse(getAttr(x, "width", ""));
							cH = float.Parse(getAttr(x, "height", ""));
							addPoint(cX, cY);
							addPoint(cX + cW, cY);
							addPoint(cX + cW, cY + cH);
							addPoint(cX, cY + cH);
							addPoint(cX, cY);
							finishLine();

							pData[currentLine].Fillable = true;

							break;
						case "path":

							// Parse the path. 
							thePath = getAttr(x, "d", "");
							if (x.GetAttrValue("fill") != "" && x.GetAttrValue("fill") != "none")
							{ // For some reason Illustrator doesn't close paths that are filled
								if (thePath.Length > 0)
								{
									if (thePath.Substring(Mathf.Max(thePath.Length - 1, 0)).ToLower() == "z")
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
						case "line":
							// Add this line 
							newLine(currentLayer);
							addPoint(float.Parse(getAttr(x, "x1", "")), float.Parse(getAttr(x, "y1", "")));
							addPoint(float.Parse(getAttr(x, "x2", "")), float.Parse(getAttr(x, "y2", "")));
							finishLine();

							break;
						case "polyline":
							newLine(currentLayer);
							string tempRefParam = getAttr(x, "points", "");
							parsePolyLine(ref tempRefParam);
							finishLine();

							break;
						case "polygon":
							newLine(currentLayer);
							string tempRefParam2 = getAttr(x, "points", "");
							parsePolyLine(ref tempRefParam2);
							finishLine();

							pData[currentLine].Fillable = true;


							break;
						case "circle":
							// Draw a circle. 
							newLine(currentLayer);
							parseCircle(float.Parse(getAttr(x, "cx", "")), float.Parse(getAttr(x, "cy", "")), float.Parse(getAttr(x, "r", "")));

							break;
						case "ellipse":  // Draw an ellipse 
							newLine(currentLayer);
							//   cx="245.46707" 
							//   cy = "469.48389" 
							//   rx = "13.131983" 
							//   ry="14.142136" /> 

							parseEllipse(float.Parse(getAttr(x, "cx", "")), float.Parse(getAttr(x, "cy", "")), float.Parse(getAttr(x, "rx", "")), float.Parse(getAttr(x, "ry", "")));
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
			x = (Chilkat.Xml)x.NextSibling();
		};



		return null;
	}

	internal static object parseSVGKids(Chilkat.Xml inEle)
	{
		string tempRefParam = "";
		return parseSVGKids(inEle, ref tempRefParam);
	}

}
