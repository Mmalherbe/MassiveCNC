using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace svg_parser
{
	internal static class Bezier
	{





		// Parametric functions for drawing a degree 3 Bezier curve.
		private static float bezX(float t, float x0, float x1, float X2, float x3)
		{
			return (float)(x0 * Math.Pow(1 - t, 3) + x1 * 3 * t * Math.Pow(1 - t, 2) + X2 * 3 * Math.Pow(t, 2) * (1 - t) + x3 * Math.Pow(t, 3));
		}

		private static float bezY(float t, float y0, float y1, float y2, float y3)
		{
			return (float)(y0 * Math.Pow(1 - t, 3) + y1 * 3 * t * Math.Pow(1 - t, 2) + y2 * 3 * Math.Pow(t, 2) * (1 - t) + y3 * Math.Pow(t, 3));
		}

		// Draw the Bezier curve.

		internal static void AddBezier(float dt, svgparser.pointD pt0, svgparser.pointD pt1, svgparser.pointD pt2, svgparser.pointD pt3)
		{
			// Draw the curve.
			float x0 = 0, y0 = 0;

			float t = 0;
			float x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
			float y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
			t += dt;

			svgparser.addPoint(x1, y1);

			while (t < 1d)
			{
				x0 = x1;
				y0 = y1;
				x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
				y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
				//gr.Line (x0, y0)-(x1, y1)

				svgparser.addPoint(x1, y1);


				t += dt;
			};

			// Connect to the final point.
			t = 1f;
			x0 = x1;
			y0 = y1;
			x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
			y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
			//gr.Line (x0, y0)-(x1, y1)

			svgparser.addPoint(x1, y1);

		}

		internal static void AddQuadBezier(float dt, svgparser.pointD pt0, svgparser.pointD pt1, svgparser.pointD pt2)
		{


			//Protected i
			//Protected.f t, t1, A, b, c, D
			float t1 = 0;
			float c = 0, A = 0, b = 0;

			float t = 0;

			while (t < 1f)
			{
				t1 = 1f - t;
				A = float.Parse(Math.Pow(t1, 2).ToString());
				b = 2f * t * t1;
				c = float.Parse(Math.Pow(t, 2).ToString());

				svgparser.addPoint(A * pt0.x + b * pt1.x + c * pt2.x, A * pt0.y + b * pt1.y + c * pt2.y);
				t += dt;
			};

			// One more at t = 1
			t = 1;
			t1 = 1f - t;
			A = float.Parse(Math.Pow(t1, 2).ToString());
			b = 2f * t * t1;
			c = float.Parse(Math.Pow(t, 2).ToString());

			svgparser.addPoint(A * pt0.x + b * pt1.x + c * pt2.x, A * pt0.y + b * pt1.y + c * pt2.y);


		}
	}

}
