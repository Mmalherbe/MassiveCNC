using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace svg_parser
{
	internal static class Polygons
	{
		#region Helper


		public static Dictionary<K, V> HashtableToDictionary<K, V>(Hashtable table)
		{
			return table
			  .Cast<DictionaryEntry>()
			  .ToDictionary(kvp => (K)kvp.Key, kvp => (V)kvp.Value);
		}



		#endregion


		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		////UPGRADE_TODO: (1050) Structure POINTAPI may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		//[DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int Polygon(int hDC, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINTAPI lpPoint, int nCount);


		internal static svgparser.pointD[] lineIntersectPoly(ref svgparser.pointD a, svgparser.pointD B, int polyID)
		{



			svgparser.pointD[] result = ArraysHelper.InitializeArray<svgparser.pointD>(1);


			//Dim pointList As New Scripting.Dictionary


			//pa.push(pa[0]); ' add itself to the end?

			//result.intersects = false;
			//result.intersections=[];
			//result.start_inside=false;
			//result.end_inside=false;


			doPoly(polyID, ref a, B, ref result);

			return result;


		}

		private static object doPoly(int polyID, ref svgparser.pointD a, svgparser.pointD B, ref svgparser.pointD[] result)
		{
			svgparser.pointD D = new svgparser.pointD(), c = new svgparser.pointD(), i = new svgparser.pointD();
			int n = 0, n2 = 0;
			OrderedDictionary cl = null;

			n = svgparser.pData[polyID].Points.GetUpperBound(0); // Set n to the last item

			while (n > 0)
			{
				c = svgparser.pData[polyID].Points[n];
				if (n == 1)
				{
					D = svgparser.pData[polyID].Points[svgparser.pData[polyID].Points.GetUpperBound(0)];
				}
				else
				{
					D = svgparser.pData[polyID].Points[n - 1];
				}
				i = lineIntersectLine(ref a, B, c, D);
				if (i.x != -6666)
				{

					n2 = result.GetUpperBound(0) + 1;
					result = ArraysHelper.RedimPreserve(result, new int[] { n2 + 1 });
					result[n2] = i;
				}

				//If lineIntersectLine(A, newPoint(C.X + D.X, A.Y), C, D).X <> -6666 Then
				//    An = An + 1
				//End If
				//If lineIntersectLine(b, newPoint(C.X + D.X, b.Y), C, D).X <> -6666 Then
				//    Bn = Bn + 1
				//End If
				n--;
			};

			//If An Mod 2 = 0 Then
			//    'result.start_inside=true;
			//End If
			//If Bn Mod 2 = 0 Then
			//    'result.end_inside=true;
			//End If
			//result.centroid=new Point(cx/(pa.length-1),cy/(pa.length-1));
			//result.intersects = result.intersections.length > 0;
			//return result;

			// Do my kids
			if (svgparser.containList.ContainsKey(polyID))
			{
				//original	cl = (OrderedDictionary)svgparser.containList[polyID];
					cl = (OrderedDictionary)svgparser.containList[polyID];

				
			} // A list of polygons that I contain
			if (cl != null)
			{
				int tempForEndVar = cl.Count;
				for (int K = 1; K <= tempForEndVar; K++)
				{
					doPoly((int)cl[K - 1], ref a, B, ref result);
				}
			}
			return null;
		}

		internal static svgparser.pointD lineIntersectLine(ref svgparser.pointD a, svgparser.pointD B, svgparser.pointD e, svgparser.pointD f, bool as_seg = true)
		{
			svgparser.pointD result = new svgparser.pointD();
			svgparser.pointD ip = new svgparser.pointD();

			result.x = -6666; // Instead of returning null, we return this to indicate no intersection

			// This is a hack, but it does the job. If the line falls on one of my vertices, move it slightly, since unpredictable results occur.

			if (e.y == a.y)
			{
				a.y += 0.000001f;
			}
			if (f.y == a.y)
			{
				a.y += 0.000001f;
			}

			float a1 = B.y - a.y;
			float b1 = a.x - B.x;
			float c1 = B.x * a.y - a.x * B.y;
			float a2 = f.y - e.y;
			float b2 = e.x - f.x;
			float c2 = f.x * e.y - e.x * f.y;

			float denom = a1 * b2 - a2 * b1;
			if (denom == 0)
			{
				return result;
			}

			ip.x = (b1 * c2 - b2 * c1) / denom;
			ip.y = (a2 * c1 - a1 * c2) / denom;

			//If E.Y = A.Y Then Exit Function
			//If F.Y = A.Y Then Exit Function ' If the line goes through the end vertex, skip it, since we'll let it get caught by the start vertex

			//---------------------------------------------------
			//Do checks to see if intersection to endpoints
			//distance is longer than actual Segments.
			//Return null if it is with any.
			//---------------------------------------------------
			if (as_seg)
			{
				if (pointDistance(ip, B) > pointDistance(a, B))
				{
					return result;
				}
				if (pointDistance(ip, a) > pointDistance(a, B))
				{
					return result;
				}

				if (pointDistance(ip, f) > pointDistance(e, f))
				{
					return result;
				}
				if (pointDistance(ip, e) > pointDistance(e, f))
				{
					return result;
				}
			}

			return ip;

		}

		internal static float pointDistance(svgparser.pointD a, svgparser.pointD B)
		{
			// Return the distance between these two points
			return float.Parse(Math.Sqrt(Math.Pow(a.y - B.y, 2) + Math.Pow(a.x - B.x, 2)).ToString());
		}

		internal static svgparser.pointD newPoint(float X, float Y)
		{
			svgparser.pointD result = new svgparser.pointD();
			result.x = X;
			result.y = Y;
			return result;
		}


		internal static object removeDupes(svgparser.pointD[] pointList)
		{
			// remove duplicate points from an array of points
			//Dim pointList As New Scripting.Dictionary
			//Dim i As Long





			return null;
		}

		internal static object calcPolyCenter(int polyID, ref float X, ref float Y)
		{
			// Calculate the centerpoint of the polygon

			float cX = 0, cY = 0;
			int tempForEndVar = svgparser.pData[polyID].Points.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				cX += svgparser.pData[polyID].Points[i].x;
				cY += svgparser.pData[polyID].Points[i].y;
			}

			X = cX / svgparser.pData[polyID].Points.GetUpperBound(0);
			Y = cY / svgparser.pData[polyID].Points.GetUpperBound(0);


			return null;
		}

		internal static object flipPolyStartEnd(int polyID)
		{
			// Flip the points around.
			svgparser.pointD[] pTemp = null;

			// Store a copy of the array
			pTemp = (svgparser.pointD[])ArraysHelper.DeepCopy(svgparser.pData[polyID].Points);

			int tempForEndVar = pTemp.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				svgparser.pData[polyID].Points[pTemp.GetUpperBound(0) - i + 1] = pTemp[i];
			}

			return null;
		}


	}

}
