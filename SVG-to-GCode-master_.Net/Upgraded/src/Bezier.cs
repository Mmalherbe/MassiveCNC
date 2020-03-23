using System;
using System.Drawing;
using System.Windows.Forms;
using UpgradeStubs;

namespace SVGtoGCODE
{
	internal static class Bezier
	{





		// Parametric functions for drawing a degree 3 Bezier curve.
		private static double bezX(double t, double x0, double x1, double X2, double x3)
		{
			return (float) (x0 * Math.Pow(1 - t, 3) + x1 * 3 * t * Math.Pow(1 - t, 2) + X2 * 3 * Math.Pow(t, 2) * (1 - t) + x3 * Math.Pow(t, 3));
		}

		private static double bezY(double t, double y0, double y1, double y2, double y3)
		{
			return (float) (y0 * Math.Pow(1 - t, 3) + y1 * 3 * t * Math.Pow(1 - t, 2) + y2 * 3 * Math.Pow(t, 2) * (1 - t) + y3 * Math.Pow(t, 3));
		}

		// Draw the Bezier curve.
		internal static void DrawBezier(PictureBox gr, double dt, SVGParse.pointD pt0, SVGParse.pointD pt1, SVGParse.pointD pt2, SVGParse.pointD pt3)
		{
			// Debugging code commented out.
			// Draw the control lines.
			//UPGRADE_ISSUE: (2064) PictureBox property gr.ForeColor was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			gr.setForeColor(Color.Red);
			using (Graphics g = gr.CreateGraphics())
			{

				g.DrawLine(new Pen(new Color()), Convert.ToInt32(pt0.x), Convert.ToInt32(pt0.y), Convert.ToInt32(pt1.x), Convert.ToInt32(pt1.y));
			}
			using (Graphics g = gr.CreateGraphics())
			{

				g.DrawLine(new Pen(new Color()), Convert.ToInt32(pt1.x), Convert.ToInt32(pt1.y), Convert.ToInt32(pt2.x), Convert.ToInt32(pt2.y));
			}
			using (Graphics g = gr.CreateGraphics())
			{

				g.DrawLine(new Pen(new Color()), Convert.ToInt32(pt2.x), Convert.ToInt32(pt2.y), Convert.ToInt32(pt3.x), Convert.ToInt32(pt3.y));
			}

			//UPGRADE_ISSUE: (2064) PictureBox property gr.ForeColor was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			gr.setForeColor(Color.Black);


			// Draw the curve.
			double x0 = 0;
			double y0 = 0;

			double t = 0;
			double x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
			double y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
			t += dt;

			while(t < 1d)
			{
				x0 = x1;
				y0 = y1;
				x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
				y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
				using (Graphics g = gr.CreateGraphics())
				{

					g.DrawLine(new Pen(new Color()), Convert.ToInt32(x0), Convert.ToInt32(y0), Convert.ToInt32(x1), Convert.ToInt32(y1));
				}
				t += dt;
			};

			// Connect to the final point.
			t = 1d;
			x0 = x1;
			y0 = y1;
			x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
			y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
			using (Graphics g = gr.CreateGraphics())
			{

				g.DrawLine(new Pen(new Color()), Convert.ToInt32(x0), Convert.ToInt32(y0), Convert.ToInt32(x1), Convert.ToInt32(y1));
			}
		}

		internal static void AddBezier(double dt, SVGParse.pointD pt0, SVGParse.pointD pt1, SVGParse.pointD pt2, SVGParse.pointD pt3)
		{
			// Draw the curve.
			double x0 = 0, y0 = 0;

			double t = 0;
			double x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
			double y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
			t += dt;

			SVGParse.addPoint(x1, y1);

			while(t < 1d)
			{
				x0 = x1;
				y0 = y1;
				x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
				y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
				//gr.Line (x0, y0)-(x1, y1)

				SVGParse.addPoint(x1, y1);


				t += dt;
			};

			// Connect to the final point.
			t = 1d;
			x0 = x1;
			y0 = y1;
			x1 = bezX(t, pt0.x, pt1.x, pt2.x, pt3.x);
			y1 = bezY(t, pt0.y, pt1.y, pt2.y, pt3.y);
			//gr.Line (x0, y0)-(x1, y1)

			SVGParse.addPoint(x1, y1);

		}

		internal static void AddQuadBezier(double dt, SVGParse.pointD pt0, SVGParse.pointD pt1, SVGParse.pointD pt2)
		{


			//Protected i
			//Protected.f t, t1, A, b, c, D
			double t1 = 0;
			double c = 0, A = 0, b = 0;

			double t = 0;

			while(t < 1d)
			{
				t1 = 1d - t;
				A = Math.Pow(t1, 2);
				b = 2d * t * t1;
				c = Math.Pow(t, 2);

				SVGParse.addPoint(A * pt0.x + b * pt1.x + c * pt2.x, A * pt0.y + b * pt1.y + c * pt2.y);
				t += dt;
			};

			// One more at t = 1
			t = 1;
			t1 = 1d - t;
			A = Math.Pow(t1, 2);
			b = 2d * t * t1;
			c = Math.Pow(t, 2);

			SVGParse.addPoint(A * pt0.x + b * pt1.x + c * pt2.x, A * pt0.y + b * pt1.y + c * pt2.y);


		}
	}
}