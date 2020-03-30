/* MassiveCNC Playground. An Unity3D based framework for controller CNC-based machines.
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
*/using System;
using System.Collections.Generic;
using static Assets.Scripts.Dimensions;
using Assets.Scripts.ImageProcessor;
using Color = UnityEngine.Color;
using System.Drawing;
using Point = System.Windows.Point;
using UnityEngine;

namespace Assets.Scripts
{
    class modalGroup
    {
        public byte motionMode;           // G0, G1, G2, G3, //G38.2, G38.3, G38.4, G38.5, G80
        public byte coordinateSystem;     // G54, G55, G56, G57, G58, G59
        public byte planeSelect;          // G17, G18, G19
        public byte distanceMode;         // G90, G91
        public byte feedRateMode;         // G93, G94
        public byte unitsMode;            // G20, G21
        public byte programMode;          // M0, M1, M2, M30
        public byte spindleState;         // M3, M4, M5
        public byte coolantState;         // M7, M8, M9
        public byte tool;                 // T
        public int spindleSpeed;          // S
        public int feedRate;              // F
        public int mWord;
        public int pWord;
        public int oWord;
        public int lWord;
        public bool containsG2G3;
        public bool ismachineCoordG53;
        public bool isdistanceModeG90;
        public bool containsG91;

        public modalGroup()     // reset state
        { reset(); }

        public void reset()
        {
            motionMode = 0;             // G0, G1, G2, G3, G38.2, G38.3, G38.4, G38.5, G80
            coordinateSystem = 54;      // G54, G55, G56, G57, G58, G59
            planeSelect = 17;           // G17, G18, G19
            distanceMode = 90;          // G90, G91
            feedRateMode = 94;          // G93, G94
            unitsMode = 21;             // G20, G21
            programMode = 0;            // M0, M1, M2, M30
            spindleState = 5;           // M3, M4, M5
            coolantState = 9;           // M7, M8, M9
            tool = 0;                   // T
            spindleSpeed = 0;           // S
            feedRate = 0;               // F
            mWord = 0;
            pWord = 0;
            oWord = 0;
            lWord = 1;
            containsG2G3 = false;
            ismachineCoordG53 = false;
            isdistanceModeG90 = true;
            containsG91 = false;
        }
        public void resetSubroutine()
        {
            mWord = 0;
            pWord = 0;
            oWord = 0;
            lWord = 1;
        }
    }
    class gcodeByLine
    {   // ModalGroups
        public int lineNumber;          // line number in fCTBCode
        public int figureNumber;
        public string codeLine;         // copy of original gcode line
        public byte motionMode;         // G0,1,2,3
        public bool isdistanceModeG90;  // G90,91
        public bool ismachineCoordG53;  // don't apply transform to machine coordinates
        public bool isSubroutine;
        public bool isSetCoordinateSystem;  // don't process x,y,z if set coordinate system

        public byte spindleState;       // M3,4,5
        public byte coolantState;       // M7,8,9
        public int spindleSpeed;        // actual spindle spped
        public int feedRate;            // actual feed rate
        public float? x, y, z, a, b, c, u, v, w, i, j; // current parameters
        public xyzabcuvwPoint actualPos;      // accumulates position
        public float alpha;            // angle between old and this position
        public float distance;         // distance to specific point
        public string otherCode;
        public string info;
        public struct xyzabcuvwPoint
        {
            public float X, Y, Z, A, B, C, U, V, W;
            public xyzabcuvwPoint(xyzPoint tmp)
            { X = tmp.X; Y = tmp.Y; Z = tmp.Z; A = tmp.A; B = 0; C = 0; U = 0; V = 0; W = 0; }

            public static explicit operator xyPoint(xyzabcuvwPoint tmp)
            { return new xyPoint(tmp.X, tmp.Y); }
            public static explicit operator xyArcPoint(xyzabcuvwPoint tmp)
            { return new xyArcPoint(tmp.X, tmp.Y, 0, 0, 0); }
        }

        public gcodeByLine()
        { resetAll(); }
        public gcodeByLine(gcodeByLine tmp)
        {
            resetAll();
            lineNumber = tmp.lineNumber; figureNumber = tmp.figureNumber; codeLine = tmp.codeLine;
            motionMode = tmp.motionMode; isdistanceModeG90 = tmp.isdistanceModeG90; ismachineCoordG53 = tmp.ismachineCoordG53;
            isSubroutine = tmp.isSubroutine; spindleState = tmp.spindleState; coolantState = tmp.coolantState;
            spindleSpeed = tmp.spindleSpeed; feedRate = tmp.feedRate;
            x = tmp.x; y = tmp.y; z = tmp.z; i = tmp.i; j = tmp.j; a = tmp.a; b = tmp.b; c = tmp.c; u = tmp.u; v = tmp.v; w = tmp.w;
            actualPos = tmp.actualPos; distance = tmp.distance; alpha = tmp.alpha;
            isSetCoordinateSystem = tmp.isSetCoordinateSystem; otherCode = tmp.otherCode;
        }

        public string listData()
        { return string.Format("{0} mode {1} figure {2}\r", lineNumber, motionMode, figureNumber); }

        /// <summary>
        /// Reset coordinates and set G90, M5, M9
        /// </summary>
        public void resetAll()
        {
            lineNumber = 0; figureNumber = 0; codeLine = "";
            motionMode = 0; isdistanceModeG90 = true; ismachineCoordG53 = false; isSubroutine = false;
            isSetCoordinateSystem = false; spindleState = 5; coolantState = 9; spindleSpeed = 0; feedRate = 0;

            actualPos.X = 0; actualPos.Y = 0; actualPos.Z = 0; actualPos.A = 0; actualPos.B = 0; actualPos.C = 0;
            actualPos.U = 0; actualPos.V = 0; actualPos.W = 0;
            distance = -1; otherCode = ""; info = ""; alpha = 0;

            x = y = z = a = b = c = u = v = w = i = j = null;

            resetCoordinates();
        }
        public void resetAll(xyzPoint tmp)
        {
            resetAll();
            actualPos = new xyzabcuvwPoint(tmp);
        }
        /// <summary>
        /// Reset coordinates
        /// </summary>
        public void resetCoordinates()
        {
            x = null; y = null; z = null; a = null; b = null; c = null; u = null; v = null; w = null; i = null; j = null;
        }
        public void presetParsing(int lineNr, string line)
        {
            resetCoordinates();
            ismachineCoordG53 = false; isSubroutine = false;
            otherCode = "";
            lineNumber = lineNr;
            codeLine = line;
        }

        /// <summary>
        /// parse gcode line
        /// </summary>
        public void parseLine(int lineNr, string line, ref modalGroup modalState)
        {
            presetParsing(lineNr, line);
            char cmd = '\0';
            string num = "";
            bool comment = false;
            float value = 0;
            line = line.ToUpper().Trim();
            isSetCoordinateSystem = false;
            #region parse
            if (!(line.StartsWith("$") || line.StartsWith("("))) //do not parse grbl comments
            {
                try
                {
                    foreach (char c in line)
                    {
                        if (c == ';')                                   // comment?
                            break;
                        if (c == '(')                                   // comment starts
                            comment = true;
                        if (!comment)
                        {
                            if (Char.IsLetter(c))                       // if char is letter
                            {
                                if (cmd != '\0')                        // and command is set
                                {
                                    if (float.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out value))
                                        parseGCodeToken(cmd, value, ref modalState);
                                }
                                cmd = c;                                // char is a command
                                num = "";
                            }
                            else if (Char.IsNumber(c) || c == '.' || c == '-')  // char is not letter but number
                            {
                                num += c;
                            }
                        }
                        if (c == ')')                                   // comment ends
                            comment = false;
                    }
                    if (cmd != '\0')                                    // finally after for-each process final command and number
                    {
                        if (float.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out value))
                            parseGCodeToken(cmd, value, ref modalState);
                    }
                }
                catch { }
            }
            #endregion
            if (isSetCoordinateSystem)
                resetCoordinates();
        }

        /// <summary>
        /// fill current gcode line structure
        /// </summary>
        private void parseGCodeToken(char cmd, float value, ref modalGroup modalState)
        {
            switch (Char.ToUpper(cmd))
            {
                case 'X':
                    x = value;
                    break;
                case 'Y':
                    y = value;
                    break;
                case 'Z':
                    z = value;
                    break;
                case 'A':
                    a = value;
                    break;
                case 'B':
                    b = value;
                    break;
                case 'C':
                    c = value;
                    break;
                case 'U':
                    u = value;
                    break;
                case 'V':
                    v = value;
                    break;
                case 'W':
                    w = value;
                    break;
                case 'I':
                    i = value;
                    break;
                case 'J':
                    j = value;
                    break;
                case 'F':
                    modalState.feedRate = feedRate = (int)value;
                    break;
                case 'S':
                    modalState.spindleSpeed = spindleSpeed = (int)value;
                    break;
                case 'G':
                    if (value <= 3)                                 // Motion Mode 0-3 c
                    {
                        modalState.motionMode = motionMode = (byte)value;
                        if (value >= 2)
                            modalState.containsG2G3 = true;
                    }
                    else
                    {
                        otherCode += "G" + ((int)value).ToString() + " ";
                    }

                    if (value == 10)
                    { isSetCoordinateSystem = true; }

                    else if ((value == 20) || (value == 21))             // Units Mode
                    { modalState.unitsMode = (byte)value; }

                    else if (value == 53)                                // move in machine coord.
                    { ismachineCoordG53 = true; }

                    else if ((value >= 54) && (value <= 59))             // Coordinate System Select
                    { modalState.coordinateSystem = (byte)value; }

                    else if (value == 90)                                // Distance Mode
                    { modalState.distanceMode = (byte)value; modalState.isdistanceModeG90 = true; }
                    else if (value == 91)
                    {
                        modalState.distanceMode = (byte)value; modalState.isdistanceModeG90 = false;
                        modalState.containsG91 = true;
                    }
                    else if ((value == 93) || (value == 94))             // Feed Rate Mode
                    { modalState.feedRateMode = (byte)value; }
                    break;
                case 'M':
                    if ((value < 3) || (value == 30))                   // Program Mode 0, 1 ,2 ,30
                    { modalState.programMode = (byte)value; }
                    else if (value >= 3 && value <= 5)                   // Spindle State
                    { modalState.spindleState = spindleState = (byte)value; }
                    else if (value >= 7 && value <= 9)                   // Coolant State
                    { modalState.coolantState = coolantState = (byte)value; }
                    modalState.mWord = (byte)value;
                    if ((value < 3) || (value > 9))
                        otherCode += "M" + ((int)value).ToString() + " ";
                    break;
                case 'T':
                    modalState.tool = (byte)value;
                    otherCode += "T" + ((int)value).ToString() + " ";
                    break;
                case 'P':
                    modalState.pWord = (int)value;
                    otherCode += "P" + value.ToString() + " ";
                    break;
                case 'O':
                    modalState.oWord = (int)value;
                    break;
                case 'L':
                    modalState.lWord = (int)value;
                    break;
            }
            isdistanceModeG90 = modalState.isdistanceModeG90;
        }
    };
    class coordByLine
    {
        public int lineNumber;          // line number in fCTBCode
        public int figureNumber;
        public xyPoint actualPos;       // accumulates position
        public float alpha;            // angle between old and this position
        public float distance;         // distance to specific point
        public bool isArc;

        public coordByLine(int line, int figure, xyPoint p, float a, bool isarc)
        { lineNumber = line; figureNumber = figure; actualPos = p; alpha = a; distance = -1; isArc = isarc; }

        public coordByLine(int line, int figure, xyPoint p, float a, float dist)
        { lineNumber = line; figureNumber = figure; actualPos = p; alpha = a; distance = dist; isArc = false; }

        public void calcDistance(xyPoint tmp)
        {
            xyPoint delta = new xyPoint(tmp - actualPos);
            distance = Mathf.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
        }
    }



    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };

    public struct sConvert
    {
        public string msg;
        public string text;
        public grblState state;
        public Color color;
    };
    struct ArcProperties
    {
        public float angleStart, angleEnd, angleDiff, radius;
        public xyPoint center;
    };

    public class XYEventArgs : EventArgs
    {
        private float angle, scale;
        private xyPoint point;
        string command;
        public XYEventArgs(float a, float s, xyPoint p, string cmd)
        {
            angle = a;
            scale = s;
            point = p;
            command = cmd;
        }
        public XYEventArgs(float a, float x, float y, string cmd)
        {
            angle = a;
            point.X = x;
            point.Y = y;
            command = cmd;
        }
        public float Angle
        { get { return angle; } }
        public float Scale
        { get { return scale; } }
        public xyPoint Point
        { get { return point; } }
        public float PosX
        { get { return point.X; } }
        public float PosY
        { get { return point.Y; } }
        public string Command
        { get { return command; } }
    }

    public class XYZEventArgs : EventArgs
    {
        private float? posX, posY, posZ;
        string command;
        public XYZEventArgs(float? x, float? y, string cmd)
        {
            posX = x;
            posY = y;
            posZ = null;
            command = cmd;
        }
        public XYZEventArgs(float? x, float? y, float? z, string cmd)
        {
            posX = x;
            posY = y;
            posZ = z;
            command = cmd;
        }
        public float? PosX
        { get { return posX; } }
        public float? PosY
        { get { return posY; } }
        public float? PosZ
        { get { return posZ; } }
        public string Command
        { get { return command; } }
    }
    public class Dimensions
    {
        public float minx, maxx, miny, maxy, minz, maxz;
        public float dimx, dimy, dimz;

        public Dimensions()
        { resetDimension(); }
        public void setDimensionXYZ(float? x, float? y, float? z)
        {
            if (x != null) { setDimensionX((float)x); }
            if (y != null) { setDimensionY((float)y); }
            if (z != null) { setDimensionZ((float)z); }
        }
        public void setDimensionXY(float? x, float? y)
        {
            if (x != null) { setDimensionX((float)x); }
            if (y != null) { setDimensionY((float)y); }
        }
        public void setDimensionX(float value)
        {
            minx = Mathf.Min(minx, value);
            maxx = Mathf.Max(maxx, value);
            dimx = maxx - minx;
        }
        public void setDimensionY(float value)
        {
            miny = Mathf.Min(miny, value);
            maxy = Mathf.Max(maxy, value);
            dimy = maxy - miny;
        }
        public void setDimensionZ(float value)
        {
            minz = Mathf.Min(minz, value);
            maxz = Mathf.Max(maxz, value);
            dimz = maxz - minz;
        }

        public float getArea()
        { return dimx * dimy; }

        // calculate min/max dimensions of a circle
        public void setDimensionCircle(float x, float y, float radius, float start, float delta)
        {
            float end = start + delta;
            if (delta > 0)
            {
                for (float i = start; i < end; i += 5)
                {
                    setDimensionX(x + radius * Mathf.Cos(i / 180 * Mathf.PI));
                    setDimensionY(y + radius * Mathf.Sin(i / 180 * Mathf.PI));
                }
            }
            else
            {
                for (float i = start; i > end; i -= 5)
                {
                    setDimensionX(x + radius * Mathf.Cos(i / 180 * Mathf.PI));
                    setDimensionY(y + radius * Mathf.Sin(i / 180 * Mathf.PI));
                }
            }

        }

        public void resetDimension()
        {
            minx = float.MaxValue;
            miny = float.MaxValue;
            minz = float.MaxValue;
            maxx = float.MinValue;
            maxy = float.MinValue;
            maxz = float.MinValue;
            dimx = 0;
            dimy = 0;
            dimz = 0;
        }
        public struct xyPoint
        {
            public float X, Y;
            public xyPoint(float x, float y)
            { X = x; Y = y; }
            public xyPoint(Point xy)
            { X = float.Parse(xy.X.ToString()); Y = float.Parse(xy.Y.ToString()); }
            public xyPoint(xyPoint tmp)
            { X = tmp.X; Y = tmp.Y; }

            public xyPoint(xyzPoint tmp)
            { X = tmp.X; Y = tmp.Y; }
            public static explicit operator xyPoint(Point tmp)
            { return new xyPoint(tmp); }
            public static explicit operator xyPoint(xyzPoint tmp)
            { return new xyPoint(tmp); }
            public static explicit operator xyPoint(xyArcPoint tmp)
            { return new xyPoint(tmp.X, tmp.Y); }

            public Point ToPoint()
            { return new Point((int)X, (int)Y); }

            //       public static explicit operator System.Windows.Point(xyPoint tmp) => new System.Windows.Point(tmp.X,tmp.Y);

            public float DistanceTo(xyPoint anotherPoint)
            {
                float distanceCodeX = X - anotherPoint.X;
                float distanceCodeY = Y - anotherPoint.Y;
                return Mathf.Sqrt(distanceCodeX * distanceCodeX + distanceCodeY * distanceCodeY);
            }
            public float AngleTo(xyPoint anotherPoint)
            {
                float distanceX = anotherPoint.X - X;
                float distanceY = anotherPoint.Y - Y;
                float radius = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);
                if (radius == 0) { return 0; }
                float cosinus = distanceX / radius;
                if (cosinus > 1) { cosinus = 1; }
                if (cosinus < -1) { cosinus = -1; }
                float angle = 180 * (float)(Mathf.Acos(cosinus) / Mathf.PI);
                if (distanceY > 0) { angle = -angle; }
                return angle;
            }

            // Overload + operator 
            public static xyPoint operator +(xyPoint b, xyPoint c)
            {
                xyPoint a = new xyPoint();
                a.X = b.X + c.X;
                a.Y = b.Y + c.Y;
                return a;
            }
            // Overload - operator 
            public static xyPoint operator -(xyPoint b, xyPoint c)
            {
                xyPoint a = new xyPoint();
                a.X = b.X - c.X;
                a.Y = b.Y - c.Y;
                return a;
            }
            // Overload * operator 
            public static xyPoint operator *(xyPoint b, float c)
            {
                xyPoint a = new xyPoint();
                a.X = b.X * c;
                a.Y = b.Y * c;
                return a;
            }
            // Overload / operator 
            public static xyPoint operator /(xyPoint b, float c)
            {
                xyPoint a = new xyPoint();
                a.X = b.X / c;
                a.Y = b.Y / c;
                return a;
            }
        };
        public class pState
        {
            public bool changed = false;
            public int motion = 0;           // {G0,G1,G2,G3,G38.2,G80} 
            public int feed_rate = 94;       // {G93,G94} 
            public int units = 21;           // {G20,G21} 
            public int distance = 90;        // {G90,G91} 
                                             // uint8_t distance_arc; // {G91.1} NOTE: Don't track. Only default supported. 
            public int plane_select = 17;    // {G17,G18,G19} 
                                             // uint8_t cutter_comp;  // {G40} NOTE: Don't track. Only default supported. 
            public float tool_length = 0;       // {G43.1,G49} 
            public int coord_select = 54;    // {G54,G55,G56,G57,G58,G59} 
                                             // uint8_t control;      // {G61} NOTE: Don't track. Only default supported. 
            public int program_flow = 0;    // {M0,M1,M2,M30} 
            public int coolant = 9;         // {M7,M8,M9} 
            public int spindle = 5;         // {M3,M4,M5} 
            public bool toolchange = false;
            public int tool = 0;            // tool number
            public float FR = 0;           // feedrate
            public float SS = 0;           // spindle speed
            public bool TLOactive = false;// Tool length offset

            public void reset()
            {
                motion = 0; plane_select = 17; units = 21;
                coord_select = 54; distance = 90; feed_rate = 94;
                program_flow = 0; coolant = 9; spindle = 5;
                toolchange = false; tool = 0; FR = 0; SS = 0;
                TLOactive = false; tool_length = 0;
                changed = false;
            }

        };

        public struct xyArcPoint
        {
            public float X, Y, CX, CY;
            public byte mode;
            public xyArcPoint(float x, float y, float cx, float cy, byte m)
            {
                X = x; Y = y; CX = cx; CY = cy; mode = m;
            }
            public xyArcPoint(xyPoint tmp)
            {
                X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
            }
            public xyArcPoint(Point tmp)
            {
                X = float.Parse(tmp.X.ToString()); Y = float.Parse(tmp.Y.ToString()); CX = 0; CY = 0; mode = 0;
            }
            public xyArcPoint(xyzPoint tmp)
            {
                X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
            }
         
            public static explicit operator xyArcPoint(xyzPoint tmp)
            {
                return new xyArcPoint(tmp);
            }
            public static explicit operator xyArcPoint(xyPoint tmp)
            {
                return new xyArcPoint(tmp);
            }
        }

        public xyPoint getCenter()
        {
            float cx = minx + ((maxx - minx) / 2);
            float cy = miny + ((maxy - miny) / 2);
            return new xyPoint(cx, cy);
        }

        // return string with dimensions
        public String getMinMaxString()
        {
            string x = String.Format("X:{0,8:####0.000} |{1,8:####0.000}\r\n", minx, maxx);
            string y = String.Format("Y:{0,8:####0.000} |{1,8:####0.000}\r\n", miny, maxy);
            string z = String.Format("Z:{0,8:####0.000} |{1,8:####0.000}", minz, maxz);
            if ((minx == float.MaxValue) || (maxx == float.MinValue))
                x = "X: unknown | unknown\r\n";
            if ((miny == float.MaxValue) || (maxy == float.MinValue))
                y = "Y: unknown | unknown\r\n";
            if ((minz == float.MaxValue) || (maxz == float.MinValue))
                z = "";// z = "Z: unknown | unknown";
            return "    Min.   | Max.\r\n" + x + y + z;
        }

        public struct xyzPoint
        {
            public float X, Y, Z, A, B, C;
            public xyzPoint(float x, float y, float z, float a = 0)
            { X = x; Y = y; Z = z; A = a; B = 0; C = 0; }
            // Overload + operator 
            public static xyzPoint operator +(xyzPoint b, xyzPoint c)
            {
                xyzPoint a = new xyzPoint();
                a.X = b.X + c.X;
                a.Y = b.Y + c.Y;
                a.Z = b.Z + c.Z;
                a.A = b.A + c.A;
                a.B = b.B + c.B;
                a.C = b.C + c.C;
                return a;
            }
            public static xyzPoint operator -(xyzPoint b, xyzPoint c)
            {
                xyzPoint a = new xyzPoint();
                a.X = b.X - c.X;
                a.Y = b.Y - c.Y;
                a.Z = b.Z - c.Z;
                a.A = b.A - c.A;
                a.B = b.B - c.B;
                a.C = b.C - c.C;
                return a;
            }
            public static bool AlmostEqual(xyzPoint a, xyzPoint b)
            {
                //     return (Mathf.Abs(a.X - b.X) <= grbl.resolution) && (Mathf.Abs(a.Y - b.Y) <= grbl.resolution) && (Mathf.Abs(a.Z - b.Z) <= grbl.resolution);
                return (gcode.isEqual(a.X, b.X) && gcode.isEqual(a.Y, b.Y) && gcode.isEqual(a.Z, b.Z));
            }

            public static class grbl
            {       // need to have global access to this data?
                public static xyzPoint posWCO = new xyzPoint(0, 0, 0);
                public static xyzPoint posWork = new xyzPoint(0, 0, 0);
                public static xyzPoint posMachine = new xyzPoint(0, 0, 0);
                public static bool posChanged = true;
                public static bool wcoChanged = true;

                public static bool isVersion_0 = true;  // note if grbl version <=0.9 or >=1.1
                        private static sConvert[] statusConvert = new sConvert[10];

                public static int axisCount = 0;
                public static bool axisA = false;       // axis A available?
                public static bool axisB = false;       // axis B available?
                public static bool axisC = false;       // axis C available?
                public static bool axisUpdate = false;  // update of GUI needed
                public static int RX_BUFFER_SIZE = 127; // grbl buffer size inside Arduino
                public static int pollInterval = 200;

                public static bool grblSimulate = false;
                private static Dictionary<int, float> settings = new Dictionary<int, float>();    // keep $$-settings
                private static Dictionary<string, xyzPoint> coordinates = new Dictionary<string, xyzPoint>();    // keep []-settings

                private static xyPoint _posMarker = new xyPoint(0, 0);
                private static float _posMarkerAngle = 0;
                private static xyPoint _posMarkerOld = new xyPoint(0, 0);
                public static xyPoint posMarker
                {
                    get
                    { return _posMarker; }
                    set
                    {
                        _posMarkerOld = _posMarker;
                        _posMarker = value;
                    }
                }
                public static xyPoint posMarkerOld
                {
                    get
                    { return _posMarkerOld; }
                    set
                    { _posMarkerOld = value; }
                }
                public static float posMarkerAngle
                {
                    get
                    { return _posMarkerAngle; }
                    set
                    { _posMarkerAngle = value; }
                }

                // Trace, Debug, Info, Warn, Error, Fatal
                //     private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

                // private mState machineState = new mState();     // Keep info about Bf, Ln, FS, Pn, Ov, A;
                //  private pState mParserState = new pState();     // keep info about last M and G settings
                //        public static pState parserState;
                //        public static bool isVers0 = true;
                //        public List<string> GRBLSettings = new List<string>();  // keep $$ settings

                public static float resolution = 0.000001f;

                public static Dictionary<string, string> messageAlarmCodes = new Dictionary<string, string>();
                public static Dictionary<string, string> messageErrorCodes = new Dictionary<string, string>();
                public static Dictionary<string, string> messageSettingCodes = new Dictionary<string, string>();

                public static void init()   // initialize lists
                {

                    //    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };
                    statusConvert[0].msg = "Idle"; statusConvert[0].text = ("grblIdle"); statusConvert[0].state = grblState.idle;
                    statusConvert[1].msg = "Run"; statusConvert[1].text = ("grblRun"); statusConvert[1].state = grblState.run; 
                    statusConvert[2].msg = "Hold"; statusConvert[2].text = ("grblHold"); statusConvert[2].state = grblState.hold; 
                    statusConvert[3].msg = "Jog"; statusConvert[3].text = ("grblJog"); statusConvert[3].state = grblState.jog; 
                    statusConvert[4].msg = "Alarm"; statusConvert[4].text = ("grblAlarm"); statusConvert[4].state = grblState.alarm; 
                    statusConvert[5].msg = "Door"; statusConvert[5].text = ("grblDoor"); statusConvert[5].state = grblState.door; 
                    statusConvert[6].msg = "Check"; statusConvert[6].text = ("grblCheck"); statusConvert[6].state = grblState.check; 
                    statusConvert[7].msg = "Home"; statusConvert[7].text = ("grblHome"); statusConvert[7].state = grblState.home; 
                    statusConvert[8].msg = "Sleep"; statusConvert[8].text = ("grblSleep"); statusConvert[8].state = grblState.sleep; 
                    statusConvert[9].msg = "Probe"; statusConvert[9].text = ("grblProbe"); statusConvert[9].state = grblState.probe; 

                    settings.Clear();
                    coordinates.Clear();
                }

                // store grbl settings https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#grbl-settings
                public static void setSettings(int id, string value)
                {
                    float tmp = 0;
                    if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp))
                    {
                        if (settings.ContainsKey(id))
                            settings[id] = tmp;
                        else
                            settings.Add(id, tmp);
                    }
                }
                public static float getSetting(int key)
                {
                    if (settings.ContainsKey(key))
                        return settings[key];
                    else
                        return -1;
                }

                // store gcode parameters https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#---view-gcode-parameters
                public static void setCoordinates(string id, string value, string info)
                {
                    xyzPoint tmp = new xyzPoint();
                    string allowed = "PRBG54G55G56G57G58G59G28G30G92TLO";
                    if (allowed.Contains(id))
                    {
                        getPosition("abc:" + value, ref tmp);   // parse string [PRB:-155.000,-160.000,-28.208:1]
                        if (coordinates.ContainsKey(id))
                            coordinates[id] = tmp;
                        else
                            coordinates.Add(id, tmp);

                        if ((info.Length > 0) && (id == "PRB"))
                        {
                            xyzPoint tmp2 = new xyzPoint();
                            tmp2 = coordinates["PRB"];
                            tmp2.A = info == "1" ? 1 : 0;
                            coordinates["PRB"] = tmp2;
                        }
                    }
                }

                public static string displayCoord(string key)
                {
                    if (coordinates.ContainsKey(key))
                    {
                        if (key == "TLO")
                            return String.Format("                  {0,8:###0.000}", coordinates[key].Z);
                        else
                        {
                            string coordString = String.Format("{0,8:###0.000} {1,8:###0.000} {2,8:###0.000}", coordinates[key].X, coordinates[key].Y, coordinates[key].Z);
                            if (axisA) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].A);
                            if (axisB) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].B);
                            if (axisC) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].C);
                            return coordString;
                        }
                    }
                    else
                        return "no data";
                }
                public static xyzPoint getCoord(string key)
                {
                    if (coordinates.ContainsKey(key))
                        return coordinates[key];
                    return new xyzPoint();
                }

                public static bool getPRBStatus()
                {
                    if (coordinates.ContainsKey("PRB"))
                    { return (coordinates["PRB"].A == 0.0) ? false : true; }
                    return false;
                }

                private static void setMessageString(ref Dictionary<string, string> myDict, string resource)
                {
                    string[] tmp = resource.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    foreach (string s in tmp)
                    {
                        string[] col = s.Split(',');
                        string message = col[col.Length - 1].Trim('"');
                        myDict.Add(col[0].Trim('"'), message);
                    }
                }

                /// <summary>
                /// parse single gcode line to set parser state
                /// </summary>
                private static bool getTLO = false;
                public static void updateParserState(string line, ref pState myParserState)
                {
                    char cmd = '\0';
                    string num = "";
                    bool comment = false;
                    float value = 0;
                    getTLO = false;
                    myParserState.changed = false;

                    if (!(line.StartsWith("$") || line.StartsWith("("))) //do not parse grbl commands
                    {
                        try
                        {
                            foreach (char c in line)
                            {
                                if (c == ';')
                                    break;
                                if (c == '(')
                                    comment = true;
                                if (!comment)
                                {
                                    if (Char.IsLetter(c))
                                    {
                                        if (cmd != '\0')
                                        {
                                            value = 0;
                                            if (num.Length > 0)
                                            {
                                                try { value = float.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo); }
                                                catch { }
                                            }
                                            try { setParserState(cmd, value, ref myParserState); }
                                            catch { }
                                        }
                                        cmd = c;
                                        num = "";
                                    }
                                    else if (Char.IsNumber(c) || c == '.' || c == '-')
                                    { num += c; }
                                }
                                if (c == ')')
                                { comment = false; }
                            }
                            if (cmd != '\0')
                            {
                                try { setParserState(cmd, float.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo), ref myParserState); }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }

                /// <summary>
                /// set parser state
                /// </summary>
                private static void setParserState(char cmd, float value, ref pState myParserState)
                {
                    //            myParserState.changed = false;
                    switch (Char.ToUpper(cmd))
                    {
                        case 'G':
                            if (value <= 3)
                            {
                                myParserState.motion = (byte)value;
                                break;
                            }
                            if ((value >= 17) && (value <= 19))
                                myParserState.plane_select = (byte)value;
                            else if ((value == 20) || (value == 21))
                                myParserState.units = (byte)value;
                            else if ((value >= 43) && (value < 44))
                            { myParserState.TLOactive = true; getTLO = true; }
                            else if (value == 49)
                                myParserState.TLOactive = false;
                            else if ((value >= 54) && (value <= 59))
                                myParserState.coord_select = (byte)value;
                            else if ((value == 90) || (value == 91))
                                myParserState.distance = (byte)value;
                            else if ((value == 93) || (value == 94))
                                myParserState.feed_rate = (byte)value;
                            myParserState.changed = true;
                            //                    MessageBox.Show("set parser state "+cmd + "  " + value.ToString()+ "  "+ myParserState.TLOactive.ToString());
                            break;
                        case 'M':
                            if ((value <= 2) || (value == 30))
                                myParserState.program_flow = (byte)value;    // M0, M1 pause, M2, M30 stop
                            else if ((value >= 3) && (value <= 5))
                                myParserState.spindle = (byte)value;    // M3, M4 start, M5 stop
                            else if ((value >= 7) && (value <= 9))
                                myParserState.coolant = (byte)value;    // M7, M8 on   M9 coolant off
                            else if (value == 6)
                                myParserState.toolchange = true;
                            myParserState.changed = true;
                            break;
                        case 'F':
                            myParserState.FR = value;
                            myParserState.changed = true;
                            break;
                        case 'S':
                            myParserState.SS = value;
                            myParserState.changed = true;
                            break;
                        case 'T':
                            myParserState.tool = (byte)value;
                            myParserState.changed = true;
                            break;
                        case 'Z':
                            if (getTLO)
                                myParserState.tool_length = value;
                            break;
                    }
                }
                // check https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#g---view-gcode-parser-state
                public static int[] unknownG = { 41, 64, 81, 83 };
                public static grblState parseStatus(string status)    // {idle, run, hold, home, alarm, check, door}
                {
                    for (int i = 0; i < statusConvert.Length; i++)
                    {
                        if (status.StartsWith(statusConvert[i].msg))     // status == statusConvert[i].msg
                            return statusConvert[i].state;
                    }
                    return grblState.unknown;
                }
                public static string statusToText(grblState state)
                {
                    for (int i = 0; i < statusConvert.Length; i++)
                    {
                        if (state == statusConvert[i].state)
                        {
                            if (CNC_Settings.grblTranslateMessage)
                                return statusConvert[i].text;
                            else
                                return statusConvert[i].state.ToString();
                        }
                    }
                    return "Unknown";
                }
               
                public static void getPosition(string text, ref xyzPoint position)
                {
                    string[] dataField = text.Split(':');
                    string[] dataValue = dataField[1].Split(',');
                    //            axisA = false; axisB = false; axisC = false;
                    axisCount = 0;
                    if (dataValue.Length == 1)
                    {
                        float.TryParse(dataValue[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Z);
                        position.X = 0;
                        position.Y = 0;
                    }
                    if (dataValue.Length > 2)
                    {
                        float.TryParse(dataValue[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.X);
                        float.TryParse(dataValue[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Y);
                        float.TryParse(dataValue[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Z);
                        axisCount = 3;
                    }
                    if (dataValue.Length > 3)
                    {
                        float.TryParse(dataValue[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.A);
                        axisA = true; axisCount++;
                    }
                    if (dataValue.Length > 4)
                    {
                        float.TryParse(dataValue[4], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.B);
                        axisB = true; axisCount++;
                    }
                    if (dataValue.Length > 5)
                    {
                        float.TryParse(dataValue[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.C);
                        axisC = true; axisCount++;
                    }
                    //axisA = true; axisB = true; axisC = true;     // for test only
                }

                public static string getSetting(string msgNr)
                {
                    string msg = " no information found '" + msgNr + "'";
                    try { msg = grbl.messageSettingCodes[msgNr]; }
                    catch { }
                    return msg;
                }
                public static string getError(string rxString)
                {
                    string[] tmp = rxString.Split(':');
                    string msg = " no information found for error-nr. '" + tmp[1] + "'";
                    try
                    {
                        if (messageErrorCodes.ContainsKey(tmp[1].Trim()))
                        {
                            msg = grbl.messageErrorCodes[tmp[1].Trim()];
                            int errnr = Convert.ToInt16(tmp[1].Trim());
                            if ((errnr >= 32) && (errnr <= 34))
                                msg += "\r\n\r\nPossible reason: scale down of GCode with G2/3 commands.\r\nSolution: use more decimal places.";
                        }
                    }
                    catch { }
                    return msg;
                }
                public static bool errorBecauseOfBadCode(string rxString)
                {
                    string[] tmp = rxString.Split(':');
                    try
                    {
                        int[] notByGCode = { 3, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16, 17, 18, 19 };
                        int errnr = Convert.ToInt16(tmp[1].Trim());
                        if (Array.Exists(notByGCode, element => element == errnr))
                            return false;
                        else
                            return true;
                    }
                    catch { }
                    return true;
                }
                public static string getAlarm(string rxString)
                {
                    string[] tmp = rxString.Split(':');
                    string msg = " no information found for alarm-nr. '" + tmp[1] + "'";
                    try
                    {
                        if (messageAlarmCodes.ContainsKey(tmp[1].Trim()))
                            msg = grbl.messageAlarmCodes[tmp[1].Trim()];
                    }
                    catch { }
                    return msg;
                }
                public static string getRealtime(int id)
                {
                    switch (id)
                    {
                        case 24:
                            return "Soft-Reset";
                        case '?':
                            return "Status Report Query";
                        case '~':
                            return "Cycle Start / Resume";
                        case '!':
                            return "Feed Hold";
                        case 132:
                            return "Safety Door";
                        case 133:
                            return "Jog Cancel";
                        case 144:
                            return "Set 100% of programmed feed rate.";
                        case 145:
                            return "Feed Rate increase 10%";
                        case 146:
                            return "Feed Rate decrease 10%";
                        case 147:
                            return "Feed Rate increase 1%";
                        case 148:
                            return "Feed Rate decrease 1%";
                        case 149:
                            return "Set to 100% full rapid rate.";
                        case 150:
                            return "Set to 50% of rapid rate.";
                        case 151:
                            return "Set to 25% of rapid rate.";
                        case 153:
                            return "Set 100% of programmed spindle speed";
                        case 154:
                            return "Spindle Speed increase 10%";
                        case 155:
                            return "Spindle Speed decrease 10%";
                        case 156:
                            return "Spindle Speed increase 1%";
                        case 157:
                            return "Spindle Speed decrease 1%";
                        case 158:
                            return "Toggle Spindle Stop";
                        case 160:
                            return "Toggle Flood Coolant";
                        case 161:
                            return "Toggle Mist Coolant";
                        default:
                            return "unknown setting " + id.ToString();
                    }
                }
            }
            public string Print(bool singleLines, bool full = false)
            {
                bool ctrl4thUse = CNC_Settings.ctrl4thUse;
                string ctrl4thName = CNC_Settings.ctrl4thName;

                if (!full)
                {
                    if (ctrl4thUse || grbl.axisA)
                        if (singleLines)
                            return string.Format("X={0,9:0.000}\rY={1,9:0.000}\rZ={2,9:0.000}\r{3}={4,9:0.000}", X, Y, Z, ctrl4thName, A);
                        else
                            return string.Format("X={0,9:0.000}  Y={1,9:0.000}  Z={2,9:0.000}\r{3}={4,9:0.000}", X, Y, Z, ctrl4thName, A);

                    else
                        if (singleLines)
                        return string.Format("X={0,9:0.000}\rY={1,9:0.000}\rZ={2,9:0.000}", X, Y, Z);
                    else
                        return string.Format("X={0,9:0.000} Y={1,9:0.000} Z={2,9:0.000}", X, Y, Z);
                }
                else
                {
                    if (singleLines)
                        return string.Format("X={0,9:0.000}\rY={1,9:0.000}\rZ={2,9:0.000}\rA={3,9:0.000}\rB={4,9:0.000}\rC={5,9:0.000}", X, Y, Z, A, B, C);
                    else
                        return string.Format("X={0,9:0.000} Y={1,9:0.000} Z={2,9:0.000}\rA={3,9:0.000} B={4,9:0.000} C={5,9:0.000}", X, Y, Z, A, B, C);
                }
            }

        };



        public bool withinLimits(xyzPoint actualMachine, xyzPoint actualWorld)
        {
            return (withinLimits(actualMachine, minx - actualWorld.X, miny - actualWorld.Y) && withinLimits(actualMachine, maxx - actualWorld.X, maxy - actualWorld.Y));
        }
        public bool withinLimits(xyzPoint actualMachine, float tstx, float tsty)
        {
            float minlx = (float)CNC_Settings.machineLimitsHomeX;
            float maxlx = minlx + (float)CNC_Settings.machineLimitsRangeX;
            float minly = (float)CNC_Settings.machineLimitsHomeY;
            float maxly = minly + (float)CNC_Settings.machineLimitsRangeY;
            tstx += actualMachine.X;
            tsty += actualMachine.Y;
            if ((tstx < minlx) || (tstx > maxlx))
                return false;
            if ((tsty < minly) || (tsty > maxly))
                return false;
            return true;
        }
    }
    class gcodeMath
    {
        private static float precision = 0.00001f;

        public static bool isEqual(Point a,Point b)
        { return ((Mathf.Abs(float.Parse(a.X.ToString()) - float.Parse(b.X.ToString())) < precision) && (Mathf.Abs(float.Parse(a.Y.ToString()) - float.Parse(b.Y.ToString())) < precision)); }
        public static bool isEqual(xyPoint a, xyPoint b)
        { return ((Mathf.Abs(a.X - b.X) < precision) && (Mathf.Abs(a.Y - b.Y) < precision)); }

        public static float distancePointToPoint(Point a, Point b)
        {
           float aX = float.Parse(a.X.ToString());
            float bX = float.Parse(b.X.ToString());
            float aY = float.Parse(a.Y.ToString());
            float bY = float.Parse(b.Y.ToString());
            return Mathf.Sqrt(((aX - bX) * (aX - bX)) + ((aY - bY) * (aY - bY))); }

        public static ArcProperties getArcMoveProperties(xyPoint pOld, xyPoint pNew, float? I, float? J, bool isG2)
        {
            ArcProperties tmp = getArcMoveAngle(pOld, pNew, I, J);
            if (!isG2) { tmp.angleDiff = Mathf.Abs(tmp.angleEnd - tmp.angleStart + 2 * Mathf.PI); }
            if (tmp.angleDiff > (2 * Mathf.PI)) { tmp.angleDiff -= (2 * Mathf.PI); }
            if (tmp.angleDiff < (-2 * Mathf.PI)) { tmp.angleDiff += (2 * Mathf.PI); }

            if ((pOld.X == pNew.X) && (pOld.Y == pNew.Y))
            {
                if (isG2) { tmp.angleDiff = -2 * Mathf.PI; }
                else { tmp.angleDiff = 2 * Mathf.PI; }
            }
            return tmp;
        }

        public static ArcProperties getArcMoveAngle(xyPoint pOld, xyPoint pNew, float? I, float? J)
        {
            ArcProperties tmp;
            if (I == null) { I = 0; }
            if (J == null) { J = 0; }
            float i = (float)I;
            float j = (float)J;
            tmp.radius = Mathf.Sqrt(i * i + j * j);  // get radius of circle
            tmp.center.X = pOld.X + i;
            tmp.center.Y = pOld.Y + j;
            tmp.angleStart = tmp.angleEnd = tmp.angleDiff = 0;
            if (tmp.radius == 0)
                return tmp;

            float cos1 = i / tmp.radius;
            if (cos1 > 1) cos1 = 1;
            if (cos1 < -1) cos1 = -1;
            tmp.angleStart = Mathf.PI - Mathf.Acos(cos1);
            if (j > 0) { tmp.angleStart = -tmp.angleStart; }

            float cos2 = (tmp.center.X - pNew.X) / tmp.radius;
            if (cos2 > 1) cos2 = 1;
            if (cos2 < -1) cos2 = -1;
            tmp.angleEnd = Mathf.PI - Mathf.Acos(cos2);
            if ((tmp.center.Y - pNew.Y) > 0) { tmp.angleEnd = -tmp.angleEnd; }

            tmp.angleDiff = tmp.angleEnd - tmp.angleStart - 2 * Mathf.PI;
            return tmp;
        }

        public static float getAlpha(Point pOld, float P2x, float P2y)
        { return getAlpha(float.Parse(pOld.X.ToString()), float.Parse(pOld.Y.ToString()), P2x, P2y); }
        public static float getAlpha(Point pOld, Point pNew)
        { return getAlpha(float.Parse(pOld.X.ToString()), float.Parse(pOld.Y.ToString()), float.Parse(pNew.X.ToString()), float.Parse(pNew.Y.ToString())); }
        public static float getAlpha(xyPoint pOld, xyPoint pNew)
        { return getAlpha(pOld.X, pOld.Y, pNew.X, pNew.Y); }
        public static float getAlpha(float P1x, float P1y, float P2x, float P2y)
        {
            float s = 1, a = 0;
            float dx = P2x - P1x;
            float dy = P2y - P1y;
            if (dx == 0)
            {
                if (dy > 0)
                    a = Mathf.PI / 2;
                else
                    a = 3 * Mathf.PI / 2;
                if (dy == 0)
                    return 0;
            }
            else if (dy == 0)
            {
                if (dx > 0)
                    a = 0;
                else
                    a = Mathf.PI;
                if (dx == 0)
                    return 0;
            }
            else
            {
                s = dy / dx;
                a = Mathf.Atan(s);
                if (dx < 0)
                    a += Mathf.PI;
            }
            return a;
        }

        public static float cutAngle = 0, cutAngleLast = 0, angleOffset = 0;
        public static void resetAngles()
        { angleOffset = cutAngle = cutAngleLast = 0.0f; }
        public static float getAngle(Point a, Point b, float offset, int dir)
        { return monitorAngle(getAlpha(a, b) + offset, dir); }
        private static float monitorAngle(float angle, int direction)		// take care of G2 cw G3 ccw direction
        {
            float diff = angle - cutAngleLast + angleOffset;
            if (direction == 2)
            { if (diff > 0) { angleOffset -= 2 * Mathf.PI; } }    // clock wise, more negative
            else if (direction == 3)
            { if (diff < 0) { angleOffset += 2 * Mathf.PI; } }    // counter clock wise, more positive
            else
            {
                if (diff > Mathf.PI)
                    angleOffset -= 2 * Mathf.PI;
                if (diff < -Mathf.PI)
                    angleOffset += 2 * Mathf.PI;
            }
            angle += angleOffset;
            return angle;
        }


    }
}
