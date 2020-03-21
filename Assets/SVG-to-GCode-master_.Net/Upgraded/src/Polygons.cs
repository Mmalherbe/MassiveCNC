using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using UpgradeHelpers.Helpers;

namespace SVGtoGCODE
{
	internal static class Polygons
	{



		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		////UPGRADE_TODO: (1050) Structure POINTAPI may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		//[DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int Polygon(int hDC, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINTAPI lpPoint, int nCount);


		internal static SVGParse.pointD[] lineIntersectPoly(ref SVGParse.pointD a, SVGParse.pointD B, int polyID)
		{



			SVGParse.pointD[] result = ArraysHelper.InitializeArray<SVGParse.pointD>(1);


			//Dim pointList As New Scripting.Dictionary


			//pa.push(pa[0]); ' add itself to the end?

			//result.intersects = false;
			//result.intersections=[];
			//result.start_inside=false;
			//result.end_inside=false;


			doPoly(polyID, ref a, B, ref result);

			return result;


		}

		private static object doPoly(int polyID, ref SVGParse.pointD a, SVGParse.pointD B, ref SVGParse.pointD[] result)
		{
			SVGParse.pointD D = new SVGParse.pointD(), c = new SVGParse.pointD(), i = new SVGParse.pointD();
			int n = 0, n2 = 0;
			OrderedDictionary cl = null;

			n = SVGParse.pData[polyID].Points.GetUpperBound(0); // Set n to the last item

			while(n > 0)
			{
				c = SVGParse.pData[polyID].Points[n];
				if (n == 1)
				{
					D = SVGParse.pData[polyID].Points[SVGParse.pData[polyID].Points.GetUpperBound(0)];
				}
				else
				{
					D = SVGParse.pData[polyID].Points[n - 1];
				}
				i = lineIntersectLine(ref a, B, c, D);
				if (i.x != -6666)
				{

					n2 = result.GetUpperBound(0) + 1;
					result = ArraysHelper.RedimPreserve(result, new int[]{n2 + 1});
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
			if (SVGParse.containList.ContainsKey(polyID))
			{
				cl = (OrderedDictionary) SVGParse.containList[polyID];
			} // A list of polygons that I contain
			if (cl != null)
			{
				int tempForEndVar = cl.Count;
				for (int K = 1; K <= tempForEndVar; K++)
				{
					doPoly((int) cl[K - 1], ref a, B, ref result);
				}
			}
			return null;
		}

		internal static SVGParse.pointD lineIntersectLine(ref SVGParse.pointD a, SVGParse.pointD B, SVGParse.pointD e, SVGParse.pointD f, bool as_seg = true)
		{
			SVGParse.pointD result = new SVGParse.pointD();
			SVGParse.pointD ip = new SVGParse.pointD();

			result.x = -6666; // Instead of returning null, we return this to indicate no intersection

			// This is a hack, but it does the job. If the line falls on one of my vertices, move it slightly, since unpredictable results occur.

			if (e.y == a.y)
			{
				a.y += 0.000001d;
			}
			if (f.y == a.y)
			{
				a.y += 0.000001d;
			}

			double a1 = B.y - a.y;
			double b1 = a.x - B.x;
			double c1 = B.x * a.y - a.x * B.y;
			double a2 = f.y - e.y;
			double b2 = e.x - f.x;
			double c2 = f.x * e.y - e.x * f.y;

			double denom = a1 * b2 - a2 * b1;
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

		internal static double pointDistance(SVGParse.pointD a, SVGParse.pointD B)
		{
			// Return the distance between these two points
			return Math.Sqrt(Math.Pow(a.y - B.y, 2) + Math.Pow(a.x - B.x, 2));
		}

		internal static SVGParse.pointD newPoint(double X, double Y)
		{
			SVGParse.pointD result = new SVGParse.pointD();
			result.x = X;
			result.y = Y;
			return result;
		}


		internal static object removeDupes(SVGParse.pointD[] pointList)
		{
			// remove duplicate points from an array of points
			//Dim pointList As New Scripting.Dictionary
			//Dim i As Long





			return null;
		}

		internal static object calcPolyCenter(int polyID, ref double X, ref double Y)
		{
			// Calculate the centerpoint of the polygon

			double cX = 0, cY = 0;
			int tempForEndVar = SVGParse.pData[polyID].Points.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				cX += SVGParse.pData[polyID].Points[i].x;
				cY += SVGParse.pData[polyID].Points[i].y;
			}

			X = cX / SVGParse.pData[polyID].Points.GetUpperBound(0);
			Y = cY / SVGParse.pData[polyID].Points.GetUpperBound(0);


			return null;
		}

		internal static object flipPolyStartEnd(int polyID)
		{
			// Flip the points around.
			SVGParse.pointD[] pTemp = null;

			// Store a copy of the array
			pTemp = (SVGParse.pointD[]) ArraysHelper.DeepCopy(SVGParse.pData[polyID].Points);

			int tempForEndVar = pTemp.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				SVGParse.pData[polyID].Points[pTemp.GetUpperBound(0) - i + 1] = pTemp[i];
			}

			return null;
		}

		internal static object addFillPolies(ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINTAPI[] polyFills, int polyID)
		{

			int n = 0;

			int tempForEndVar = SVGParse.pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				if (SVGParse.pData[i].ContainedBy == polyID && SVGParse.pData[i].Fillable)
				{

					n = polyFills.GetUpperBound(0);
					polyFills = ArraysHelper.RedimPreserve(polyFills, new int[]{n + SVGParse.pData[i].Points.GetUpperBound(0) + 1});

					int tempForEndVar2 = SVGParse.pData[i].Points.GetUpperBound(0);
					for (int j = 1; j <= tempForEndVar2; j++)
					{
						polyFills[j + n].X = Convert.ToInt32((SVGParse.pData[i].Points[j].x + frmInterface.DefInstance.panX) * frmInterface.DefInstance.Zoom);
						polyFills[j + n].Y = Convert.ToInt32((SVGParse.pData[i].Points[j].y + frmInterface.DefInstance.panY) * frmInterface.DefInstance.Zoom);
					}

					addFillPolies(ref polyFills, i);
				}
			}

			return null;
		}
	}
}