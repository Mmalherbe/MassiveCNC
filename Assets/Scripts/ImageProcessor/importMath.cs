﻿/* MassiveCNC Playground. An Unity3D based framework for controller CNC-based machines.
    Created and altered by Max Malherbe.
    
    Originally created by Sven Hasemann, altered and rewritten by me.

    Origibal Project : GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
//using System.Drawing;
using System.Windows;
using System.Windows.Media;



namespace Assets.Scripts.ImageProcessor
{
    class importMath
    {
       

        /// <summary>
        /// Calculate Path-Arc-Command - Code from https://github.com/vvvv/SVG/blob/master/Source/Paths/SvgArcSegment.cs
        /// </summary>
        public static void calcArc(float StartX, float StartY, float RadiusX, float RadiusY,
            float Angle, float Size, float Sweep, float EndX, float EndY, Action<Point, string> moveTo)
        {
           
            if (RadiusX == 0.0f && RadiusY == 0.0f)
            {                return;
            }
            double sinPhi = Math.Sin(Angle * Math.PI / 180.0);
            double cosPhi = Math.Cos(Angle * Math.PI / 180.0);
            double x1dash = cosPhi * (StartX - EndX) / 2.0 + sinPhi * (StartY - EndY) / 2.0;
            double y1dash = -sinPhi * (StartX - EndX) / 2.0 + cosPhi * (StartY - EndY) / 2.0;
            double root;
            double numerator = RadiusX * RadiusX * RadiusY * RadiusY - RadiusX * RadiusX * y1dash * y1dash - RadiusY * RadiusY * x1dash * x1dash;
            float rx = RadiusX;
            float ry = RadiusY;
            if (numerator < 0.0)
            {
                float s = (float)Math.Sqrt(1.0 - numerator / (RadiusX * RadiusX * RadiusY * RadiusY));

                rx *= s;
                ry *= s;
                root = 0.0;
            }
            else
            {
                root = ((Size == 1 && Sweep == 1) || (Size == 0 && Sweep == 0) ? -1.0 : 1.0) * Math.Sqrt(numerator / (RadiusX * RadiusX * y1dash * y1dash + RadiusY * RadiusY * x1dash * x1dash));
            }
            double cxdash = root * rx * y1dash / ry;
            double cydash = -root * ry * x1dash / rx;
            double cx = cosPhi * cxdash - sinPhi * cydash + (StartX + EndX) / 2.0;
            double cy = sinPhi * cxdash + cosPhi * cydash + (StartY + EndY) / 2.0;
            double theta1 = CalculateVectorAngle(1.0, 0.0, (x1dash - cxdash) / rx, (y1dash - cydash) / ry);
            double dtheta = CalculateVectorAngle((x1dash - cxdash) / rx, (y1dash - cydash) / ry, (-x1dash - cxdash) / rx, (-y1dash - cydash) / ry);
            if (Sweep == 0 && dtheta > 0)
            {
                dtheta -= 2.0 * Math.PI;
            }
            else if (Sweep == 1 && dtheta < 0)
            {
                dtheta += 2.0 * Math.PI;
            }
            int segments = (int)Math.Ceiling((double)Math.Abs(dtheta / (Math.PI / 2.0)));
            double delta = dtheta / segments;
            double t = 8.0 / 3.0 * Math.Sin(delta / 4.0) * Math.Sin(delta / 4.0) / Math.Sin(delta / 2.0);

            double startX = StartX;
            double startY = StartY;

            for (int i = 0; i < segments; ++i)
            {
                double cosTheta1 = Math.Cos(theta1);
                double sinTheta1 = Math.Sin(theta1);
                double theta2 = theta1 + delta;
                double cosTheta2 = Math.Cos(theta2);
                double sinTheta2 = Math.Sin(theta2);

                double endpointX = cosPhi * rx * cosTheta2 - sinPhi * ry * sinTheta2 + cx;
                double endpointY = sinPhi * rx * cosTheta2 + cosPhi * ry * sinTheta2 + cy;

                double dx1 = t * (-cosPhi * rx * sinTheta1 - sinPhi * ry * cosTheta1);
                double dy1 = t * (-sinPhi * rx * sinTheta1 + cosPhi * ry * cosTheta1);

                double dxe = t * (cosPhi * rx * sinTheta2 + sinPhi * ry * cosTheta2);
                double dye = t * (sinPhi * rx * sinTheta2 - cosPhi * ry * cosTheta2);

                points = new Point[4];
                points[0] = new Point(startX, startY);
                points[1] = new Point((startX + dx1), (startY + dy1));
                points[2] = new Point((endpointX + dxe), (endpointY + dye));
                points[3] = new Point(endpointX, endpointY);
                var b = GetBezierApproximation(points, (int)CNC_Settings.importBezierLineSegmentsCnt);
                if(b == null) { return; }
                for (int k = 1; k < b.Points.Count; k++)
                    moveTo(b.Points[k], "arc"); //svgMoveTo(b.Points[k], "arc");

                theta1 = theta2;
                startX = (float)endpointX;
                startY = (float)endpointY;
            }
        }
        private static double CalculateVectorAngle(double ux, double uy, double vx, double vy)
        {
            double ta = Math.Atan2(uy, ux);
            double tb = Math.Atan2(vy, vx);
            if (tb >= ta)
            { return tb - ta; }
            return Math.PI * 2 - (ta - tb);
        }


        public static void calcQuadraticBezier(Point P0, Point c2, Point c3, Action<Point, string> moveTo, string cmt)
        {
            Vector qp1 = new Vector(); qp1 = ((Vector)c2 - (Vector)P0); qp1 *= (2d / 3d); qp1 += (Vector)P0;      // float qpx1 = (cx2 - lastX) * 2 / 3 + lastX;     // shorten control points to 2/3 length to use 
            Vector qp2 = new Vector(); qp2 = ((Vector)c2 - (Vector)c3); qp2 *= (2d / 3d); qp2 += (Vector)c3;      // float qpx2 = (cx2 - cx3) * 2 / 3 + cx3;
            Point[] points = new Point[4];
            points[0] = P0;             // new Point(lastX, lastY);
            points[1] = (Point)qp1;     // new Point(qpx1, qpy1);
            points[2] = (Point)qp2;     // new Point(qpx2, qpy2);
            points[3] = c3;             // new Point(cx3, cy3);
            var b = GetBezierApproximation(points, (int)CNC_Settings.importBezierLineSegmentsCnt);
            for (int i = 1; i < b.Points.Count; i++)
                moveTo(b.Points[i], cmt);
        }

        public static void calcCubicBezier(Point P0, Point c1, Point c2, Point c3, Action<Point, string> moveTo, string cmt)
        {
            Point[] points = new Point[4];
            points[0] = P0;             // new Point(lastX, lastY);
            points[1] = c1;
            points[2] = c2;
            points[3] = c3;             // new Point(cx3, cy3);
            var b = GetBezierApproximation(points, (int)CNC_Settings.importBezierLineSegmentsCnt);
            for (int i = 1; i < b.Points.Count; i++)
                moveTo(b.Points[i], cmt);
        }

        /// <summary>
        /// Calculate Bezier line segments
        /// from http://stackoverflow.com/questions/13940983/how-to-draw-bezier-curve-by-several-points
        /// </summary>
        private static Point[] points;
        public static PolyLineSegment GetBezierApproximation(Point[] controlPoints, int outputSegmentCount)
        {
            try
            {
                Point[] points = new Point[outputSegmentCount + 1];
                for (int i = 0; i <= outputSegmentCount; i++)
                {
                    float t = (float)i / outputSegmentCount;
                    points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
                }
                return new PolyLineSegment(points, true);
            }
            catch(InvalidProgramException e) {
                Console.WriteLine(e.ToString());
                return null;
            }
        
        }
        private static Point GetBezierPoint(float t, Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            double x = (1 - t) * P0.X + t * P1.X;
            return new Point(x, (1 - t) * P0.Y + t * P1.Y);
        }

    }


}
