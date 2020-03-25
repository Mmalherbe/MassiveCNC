using UnityEngine;
using System.Collections;
using Hashtable = System.Collections.Hashtable;
using Assets.Scripts.Classes;
using System.Text;
using System.Runtime.InteropServices;
 
#if !UNITY_EDITOR_LINUX
public class svgParser : MonoBehaviour
{
 
	public static typLine[] pData = null;
	public static int currentLine = 0;
	public static float GLOBAL_DPI = 0;
	static bool hasUnfinishedLine = false;
	#region structs and hashtables
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
	internal static string rebuildString(string Text, int pos, char newChar)
	{
		string NewText = string.Empty;
		for (int i = 0; i < Text.Length; i++)
		{
			if (i != pos)
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

	internal static float getPinSeg(pointD pStart, pointD pEnd)
	{
		float D = Polygons.pointDistance(pStart, pEnd) / GLOBAL_DPI;
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
		float segments = 250 * D;

		if (segments == 0)
		{
			segments = 1;
		}

		if (segments == 0)
		{ // a zero-length line? what's the point
			return 0.01f;
		}
		else
		{
			return Mathf.Max(0.01f, 1 / segments);

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

	internal static string getAttr(Xml attr, string attrName, string DefaultValue = "")
	{

		return attr.GetAttrValue(attrName);

	}

	internal static float angleFromPoint(pointD pCenter, pointD pPoint)
	{
		// Calculate the angle of a point relative to the center

		// Slope is rise over run
		float result = 0;
		float slope = 0;

		if (pPoint.x == pCenter.x)
		{
			// Either 90 or 270
			result = (pPoint.y > pCenter.y) ? Mathf.PI / 2 : (-Mathf.PI) / 2;

		}
		else if (pPoint.x > pCenter.x)
		{
			// 0 - 90 and 270-360
			slope = (pPoint.y - pCenter.y) / (pPoint.x - pCenter.x);
			result = Mathf.Atan(slope);
		}
		else
		{
			// 180-270
			slope = (pPoint.y - pCenter.y) / (pPoint.x - pCenter.x);
			result = Mathf.Atan(slope) + Mathf.PI;
		}

		if (result < 0)
		{
			result += (Mathf.PI * 2);
		}




		return result;
	}

	internal static float Deg2Rad(float inDeg)
	{
		return inDeg / (180 / Mathf.PI);
	}
	internal static string extractToken(string inPath, ref int pos)
	{

		// Exract until we get a space or a comma
		string char_Renamed = "";
		Chilkat.StringBuilder build = new Chilkat.StringBuilder();
		bool seenMinus = false;
		bool seenE = false;
		bool seenPeriod = false;
		int startPos = pos;

		inPath = inPath.Replace("\r", " ");
		inPath = inPath.Replace("\n", " ");
		inPath = inPath.Replace("\t", " ");

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

					doMerge = false;
					int tempForEndVar3 = pData.GetUpperBound(0);
					for (j = 1; j <= tempForEndVar3; j++)
					{
						if (j != i && pData[j].LayerID == pData[i].LayerID)
						{
							if (pData[i].Points[iCount].x == pData[j].Points[1].x && pData[i].Points[iCount].y == pData[j].Points[1].y)
							{


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
						Debug.Log(("MERGING SHAPE "+ j.ToString()+ "INTO "+ i.ToString()));
						didMerge = true;
						if (doFlip)
						{ // Flip it around first.
							Polygons.flipPolyStartEnd(j);
						}

						// Merge the points from j into i
						pData[i].Points = ArraysHelper.RedimPreserve(pData[i].Points, new int[] { iCount + pData[j].Points.GetUpperBound(0) + 1 });

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
						pData = ArraysHelper.RedimPreserve(pData, new int[] { pData.GetUpperBound(0) });

						// Then start the loop again.
						Debug.Log(("COUNT IS NOW "+ pData.GetUpperBound(0).ToString()));
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
		while (didMerge); // Continue looping until there's no more merging

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
			//f = Strings.InStr(e + 1, transformText, ")", CompareMethod.Binary);
			f = transformText.Substring(e).IndexOf(char.Parse(")"));
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


	internal static object parseCircle(float cX, float cY, float Radi)
	{

		float x = 0, y = 0;

		int rr = 2;
		if (Radi > 100)
		{
			rr = 1;
		}


		int tempForStepVar = rr;
		for (float A = 0; (tempForStepVar < 0) ? A >= 360 : A <= 360; A += tempForStepVar)
		{

			x = Mathf.Cos(A * (Mathf.PI / 180)) * Radi + cX;
			y = Mathf.Sin(A * (Mathf.PI / 180)) * Radi + cY;

			addPoint(x, y);


		}

		pData[currentLine].Fillable = true;

		return null;
	}


	internal static object parseArcSegment(ref float RX, ref float RY, float rotAng, pointD P1, pointD P2, bool largeArcFlag, bool sweepFlag)
	{

		// Parse "A" command in SVG, which is segments of an arc
		// P1 is start point
		// P2 is end point

		pointD centerPoint = new pointD();
		pointD P1Prime = new pointD();
		pointD P2Prime = new pointD();

		pointD CPrime = new pointD();
		float Q = 0;
		float c = 0;


		pointD tempPoint = new pointD();
		float tempAng = 0;
		float tempDist = 0;





		// Turn the degrees of rotation into radians
		float Theta = Deg2Rad(rotAng);

		// Calculate P1Prime
		P1Prime.x = (Mathf.Cos(Theta) * ((P1.x - P2.x) / 2)) + (Mathf.Sin(Theta) * ((P1.y - P2.y) / 2));
		P1Prime.y = ((-Mathf.Sin(Theta)) * ((P1.x - P2.x) / 2)) + (Mathf.Cos(Theta) * ((P1.y - P2.y) / 2));

		P2Prime.x = (Mathf.Cos(Theta) * ((P2.x - P1.x) / 2)) + (Mathf.Sin(Theta) * ((P2.y - P1.y) / 2));
		P2Prime.y = ((-Mathf.Sin(Theta)) * ((P2.x - P1.x) / 2)) + (Mathf.Cos(Theta) * ((P2.y - P1.y) / 2));

		float qTop = ((Mathf.Pow(RX, 2)) * (Mathf.Pow(RY, 2))) - ((Mathf.Pow(RX, 2)) * (Mathf.Pow(P1Prime.y, 2))) - ((Mathf.Pow(RY, 2)) * (Mathf.Pow(P1Prime.x, 2)));

		if (qTop < 0)
		{ // We've been given an invalid arc. Calculate the correct value.

			c = Mathf.Sqrt(((Mathf.Pow(P1Prime.y, 2)) / (Mathf.Pow(RY, 2))) + ((Mathf.Pow(P1Prime.x, 2)) / (Mathf.Pow(RX, 2))));

			RX *= c;
			RY *= c;

			qTop = 0;
		}

		float qBot = ((Mathf.Pow(RX, 2)) * (Mathf.Pow(P1Prime.y, 2))) + ((Mathf.Pow(RY, 2)) * (Mathf.Pow(P1Prime.x, 2)));
		if (qBot != 0)
		{
			Q = Mathf.Sqrt((qTop) / (qBot));
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
		centerPoint.x = ((Mathf.Cos(Theta) * CPrime.x) - (Mathf.Sin(Theta) * CPrime.y)) + ((P1.x + P2.x) / 2);
		centerPoint.y = ((Mathf.Sin(Theta) * CPrime.x) + (Mathf.Cos(Theta) * CPrime.y)) + ((P1.y + P2.y) / 2);


		// Calculate Theta1

		float Theta1 = angleFromPoint(P1Prime, CPrime);
		float ThetaDelta = angleFromPoint(P2Prime, CPrime);

		Theta1 -= Mathf.PI;
		ThetaDelta -= Mathf.PI;

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
				ThetaDelta += (Mathf.PI * 2);
			}
		}
		else
		{
			// Sweep  is going NEGATIVELY
			//If ThetaDelta < 0 Then ThetaDelta = ThetaDelta + (PI * 2)
			if (ThetaDelta > Theta1)
			{
				ThetaDelta -= (Mathf.PI * 2);
			}
		}


		float startAng = Theta1;
		float endAng = ThetaDelta;


		float AngStep = (Mathf.PI / 180);
		if (!sweepFlag)
		{
			AngStep = -AngStep;
		} // Sweep flag indicates a positive step

		
		//Theta = Deg2Rad(-40)

		// Hackhack
		//startAng = startAng + AngStep * 2


		float Ang = startAng;
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

			tempPoint.x = (RX * Mathf.Cos(Ang)) + centerPoint.x;
			tempPoint.y = (RY * Mathf.Sin(Ang)) + centerPoint.y;

			tempAng = angleFromPoint(centerPoint, tempPoint) + Theta;
			tempDist = Polygons.pointDistance(centerPoint, tempPoint);

			tempPoint.x = (tempDist * Mathf.Cos(tempAng)) + centerPoint.x;
			tempPoint.y = (tempDist * Mathf.Sin(tempAng)) + centerPoint.y;





			//tempPoint.X = (Cos(Theta) * tempPoint.X) + (-Sin(Theta) * tempPoint.Y)
			//tempPoint.Y = (Sin(Theta) * tempPoint.X) + (Cos(Theta) * tempPoint.Y)


			addPoint(tempPoint.x, tempPoint.y);


			Ang += AngStep;
		}
		while (!((Ang >= endAng && AngStep > 0) || (Ang <= endAng && AngStep < 0)));

		// Add the final point

		addPoint(P2.x, P2.y);


		return null;
	}



	internal static object parsePolyLine(ref string inLine)
	{
		// Parse a polyline
		string token1 = "", token2 = "";
		inLine = inLine.Replace("\r", " ");
		inLine = inLine.Replace("\n", " ");
		inLine = inLine.Replace("\t", " ");
		int pos = 1;
		for(int i =0; i < inLine.Length;i++)
		{
			token1 = extractToken(inLine, ref pos);
			token2 = extractToken(inLine, ref pos);

			if (token1 != "" && token2 != "")
			{
				addPoint(float.Parse(token1), float.Parse(token2));
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
		inPath = inPath.Replace("\r", " ");
		inPath = inPath.Replace("\n", " ");
		inPath = inPath.Replace("\t", " ");



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
		char lastChar = char.Parse("");



		//M209.1,187.65c-0.3-0.2-0.7-0.4-1-0.4c-0.3,0-0.7,0.2-0.9,0.4c-0.3,0.3-0.4,0.6-0.4,0.9c0,0.4,0.1,0.7,0.4,1
		//c0.2,0.2,0.6,0.4,0.9,0.4c0.3,0,0.7-0.2,1-0.4c0.2-0.3,0.3-0.6,0.3-1C209.4,188.25,209.3,187.95,209.1,187.65z

		// Get rid of enter presses
		inPath = inPath.Replace("\r", " ");
		inPath = inPath.Replace("\n", " ");
		inPath = inPath.Replace("\t", " ");


		for (int pos = 0; pos < inPath.Length; pos++)
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
					inPath = inPath.Replace("\r", " ");
					inPath = inPath.Replace("\n", " ");
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

					token1 = extractToken(inPath, ref pos);

					token2 = extractToken(inPath, ref pos);

					// Set into point 0 
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);


					// Extract next two co-ordinates 

					token1 = extractToken(inPath, ref pos);

					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					pt2.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt2.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 

					token1 = extractToken(inPath, ref pos);

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
					
					token1 = extractToken(inPath, ref pos);

					token2 = extractToken(inPath, ref pos);

					// Set into point 0 
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 

					token1 = extractToken(inPath, ref pos);

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
					token1 = extractToken(inPath, ref pos);

					token2 = extractToken(inPath, ref pos);

					// Set into point 0 
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 

					token1 = extractToken(inPath, ref pos);

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

					token1 = extractToken(inPath, ref pos);

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
					Debug.Log("UNSUPPORTED PATH CODE: " + inPath[pos].ToString());


					break;
			}

		}
		for (int pos = 0; pos < inPath.Length; pos++)
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
					
					token1 = extractToken(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 0 
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);


					// Extract next two co-ordinates 
					token1 = extractToken(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 1 
					pt2.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt2.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					token1 = extractToken(inPath, ref pos);
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
					
					token1 = extractToken(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 0 
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					token1 = extractToken(inPath, ref pos);
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
					token1 = extractToken(inPath, ref pos);
					token2 = extractToken(inPath, ref pos);

					// Set into point 0 
					pt1.x = ((isRelative) ? currX : 0) + float.Parse(token1);
					pt1.y = ((isRelative) ? currY : 0) + float.Parse(token2);

					// Extract next two co-ordinates 
					token1 = extractToken(inPath, ref pos);
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
					
					token1 = extractToken(inPath, ref pos);
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
					Debug.Log(("Line 1782: UNSUPPORTED PATH CODE: ", inPath[pos].ToString()));


					break;
			}


			if (pos > lastUpdate + 2000)
			{
				lastUpdate = pos;
			}

		};




		return null;
	}




	internal static object parseSVG(string inFile)
	{
		inFile = inFile.Replace("\r", " ");
		inFile = inFile.Replace("\n", " ");
		inFile = inFile.Replace("\t", " ");

		Chilkat.Xml SVG = new Chilkat.Xml();
		Chilkat.Xml x = null;

		float realW = 0;
		float realH = 0;

		string[] S = null;


		pData = new typLine[1];
		currentLine = 0;

		float realDPI = 90;
		

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
				minY = Mathf.Min(minY, float.Parse(pData[i].Points[j].y.ToString()));
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
	internal static object parseEllipse(float cX, float cY, float RadiX, float RadiY)
	{

		float x = 0, y = 0;

		int rr = 2;
		if (RadiX > 100 || RadiY > 100)
		{
			rr = 1;
		}


		int tempForStepVar = rr;
		for (float A = 0; (tempForStepVar < 0) ? A >= 360 : A <= 360; A += tempForStepVar)
		{

			x = Mathf.Cos(A * (Mathf.PI / 180)) * RadiX + cX;
			y = Mathf.Sin(A * (Mathf.PI / 180)) * RadiY + cY;

			addPoint(x, y);

		}

		pData[currentLine].Fillable = true;

		return null;
	}


	internal static object parseSVGKids(Xml inEle, ref string currentLayer)
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

		for (int i = 0; i < inEle.NumChildren; i++)
		{
			//XMLNode x = (Chilkat.Xml)inEle.FirstChild();
			Chilkat.Xml x = inEle.GetChild(i) as Chilkat.Xml;
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

			};
		}


		return null;
	}

	internal static object parseSVGKids(Chilkat.Xml  inEle)
	{
		string tempRefParam = "";
		return parseSVGKids(inEle, ref tempRefParam);
	}

}
#endregion
#endif