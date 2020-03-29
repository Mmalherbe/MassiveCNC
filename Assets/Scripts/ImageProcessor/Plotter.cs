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
*/using Assets.Scripts;
using Assets.Scripts.ImageProcessor;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using System.Windows;
using static Assets.Scripts.Dimensions;
using Assets.Scripts.FontProcessor;
using System.Drawing;
using Point = System.Windows.Point;

public static class Plotter
{
    private static bool loggerTrace = false;        //true;

    struct FigureCheck
    {
        public int lastIndexStart;
        public int lastIndexEnd;
        public int codeLength;
        public int figureNr;
    };

    private const int gcodeStringMax = 260;                 // max amount of tools
    private static int gcodeStringIndex = 0;                // index for stringBuilder-Array
    private static int gcodeStringIndexOld = 0;             // detect change in index
    private static StringBuilder[] gcodeString = new StringBuilder[gcodeStringMax];
    private static StringBuilder finalGcodeString = new StringBuilder();
    private static FigureCheck[] lastFigureStart = new FigureCheck[gcodeStringMax];
    private static Dimensions[] gcodeDimension = new Dimensions[gcodeStringMax];

    private static bool penIsDown = false;
    private static bool comments = false;
    private static bool pauseBeforePath = false;
    private static bool pauseBeforePenDown = false;
    private static bool groupObjects = false;
    private static int sortOption = 0;
    private static bool sortInvert = false;
    private static int amountOfTools = 0;
    private static bool gcodeUseSpindle = false;
    private static bool gcodeReduce = false;            // if true remove G1 commands if distance is < limit
    private static float gcodeReduceVal = .1f;          // limit when to remove G1 commands
    private static int lastSetGroup = -1;
    private static bool gcodeTangEnable = false;
    private static string gcodeTangName = "C";
    private static double gcodeTangTurn = 360;

    private static Point lastGC, lastSetGC;             // store last position
    private static bool isStartPathIsPending = false;
    private static Point posStartPath;
    private static double posStartAngle = 0;

    public static int PathCount { get; set; } = 0;
    public static int PathToolNr { get; set; } = 0;
    public static bool IsPathReduceOk { get; set; } = false;
    public static bool IsPathAvoidG23 { get; set; } = false;
    public static bool IsPathFigureEnd { get; set; } = true;
    public static string PathId { get; set; } = "";
    public static string PathName { get; set; } = "";
    private static string pathColor = "";
    public static string PathColor
    {
        get { return pathColor; }
        set
        {
            pathColor = value;
            if ((pathColor.Length == 3) && (System.Text.RegularExpressions.Regex.IsMatch(pathColor, @"\A\b[0-9a-fA-F]+\b\Z")))
            {
                char[] tmp = new char[6];
                tmp[0] = tmp[1] = pathColor[0];
                tmp[2] = tmp[3] = pathColor[1];
                tmp[4] = tmp[5] = pathColor[2];
                pathColor = new string(tmp);
            }
        }
    }
    public static string PathComment { get; set; } = "";
    public static double[] PathDashArray { get; set; } = { };

    public static string DocTitle { get; set; } = "";
    public static string DocDescription { get; set; } = "";

    // Trace, Debug, Info, Warn, Error, Fatal


    public static void StartCode()
    {
        cncLogger.RealTimeLog("startCode()");
        pauseBeforePath = CNC_Settings.importPauseBeforePath;
        pauseBeforePenDown = CNC_Settings.importPauseBeforePenDown;
        sortInvert = CNC_Settings.importGroupSortInvert;
        gcodeReduce = CNC_Settings.importRemoveShortMovesEnable;
        gcodeReduceVal = (float)CNC_Settings.importRemoveShortMovesLimit;
        gcodeTangEnable = CNC_Settings.importGCTangentialEnable;
        gcodeTangName = CNC_Settings.importGCTangentialAxis;
        gcodeTangTurn = (double)CNC_Settings.importGCTangentialTurn;
        lastSetGroup = -1;
        penIsDown = false;

        gcodeMath.resetAngles();
        posStartAngle = 0;

        isStartPathIsPending = false;

        gcodeStringIndex = 0;
        gcodeStringIndexOld = -1;
        for (int i = 0; i < gcodeStringMax; i++)        // hold gcode snippes for later sorting
        {
            gcodeString[i] = new StringBuilder();
            gcodeString[i].Clear();
            gcodeDimension[i] = new Dimensions();
        }
        finalGcodeString.Clear();

        PathCount = 0;
        PathToolNr = 0;
        IsPathReduceOk = false;
        IsPathAvoidG23 = false;
        IsPathFigureEnd = true;
        PathId = "";
        PathName = "";
        pathColor = "";
        PathComment = "";
        DocTitle = "";
        DocDescription = "";

      
        gcode.setup();                              // initialize GCode creation (get stored settings for export)

    }

    /// <summary>
    /// Set start tag, move to beginning of path via G0, finish old path
    /// </summary>
    public static void StartPath(Point coordxy, string cmt)
    {
        if (!comments) { cmt = ""; }

        if (loggerTrace) cncLogger.RealTimeLog(" StartPath at X"+ coordxy.X + " Y" + coordxy.Y + " "+ cmt);

        if ((gcodeStringIndex != gcodeStringIndexOld) || (lastGC != coordxy))    // only if change in position, do pen-up -down
        {
            PenUp(cmt + " in  StartPath");
            if (!IsPathFigureEnd)
            { SetFigureEndTag(PathCount); }
            IsPathFigureEnd = true;

            string attributeId = (PathId.Length > 0) ? string.Format(" Id=\"{0}\"", PathId) : "";
            string attributeColor = (pathColor.Length > 0) ? string.Format(" Color=\"#{0}\"", pathColor) : "";
            string attributeToolNr = string.Format(" ToolNr=\"{0}\"", PathToolNr);

            // set XML comment (<Figure...
            string xml = string.Format("{0} {1}{2}{3}{4}> ", xmlMarker.figureStart, (++PathCount), attributeId, attributeColor, attributeToolNr);
            lastFigureStart[gcodeStringIndex].lastIndexStart = gcodeString[gcodeStringIndex].Length;
            lastFigureStart[gcodeStringIndex].figureNr = PathCount;
            Comment(xml);
            lastFigureStart[gcodeStringIndex].lastIndexEnd = gcodeString[gcodeStringIndex].Length;
            lastFigureStart[gcodeStringIndex].codeLength = lastFigureStart[gcodeStringIndex].lastIndexEnd - lastFigureStart[gcodeStringIndex].lastIndexStart;
            if (loggerTrace) cncLogger.RealTimeLog( xml);

            if (comments && (PathName.Length > 0)) { Comment(PathName); }

            if (pauseBeforePath && !pauseBeforePenDown) { InsertPause("Pause before path"); }
            IsPathFigureEnd = false;

            gcodeMath.cutAngle = gcodeMath.getAngle(lastGC, coordxy, 0, 0); // get and store position
            isStartPathIsPending = true;                  // and angle of desired
            posStartPath = coordxy;                     // start-point
            posStartAngle = gcodeMath.cutAngle;                   // Apply G0 on Pen-down, when needed (in Arc or MoveTo)
            if (loggerTrace) cncLogger.RealTimeLog("   StartPath get angle for x "+ coordxy.X + " y" + coordxy.Y + " a=" + (coordxy.Y, 180 * posStartAngle / Math.PI).ToString()+ "");
        }
        lastGC = coordxy;
        lastSetGC = coordxy;
        IsPathReduceOk = false;
        gcodeStringIndexOld = gcodeStringIndex;
    }

    public static void SetFigureEndTag(int nr)
    {
        if (gcodeString[gcodeStringIndex].Length == (lastFigureStart[gcodeStringIndex].lastIndexEnd))     // no code generated
        {
            gcodeString[gcodeStringIndex].Remove(lastFigureStart[gcodeStringIndex].lastIndexStart, lastFigureStart[gcodeStringIndex].codeLength);
            if (loggerTrace) cncLogger.RealTimeLog("Code removed figure "+ lastFigureStart[gcodeStringIndex].figureNr);
        }
        else
        {
            string xml = string.Format("{0} {1}>", xmlMarker.figureEnd, nr);    //string.Format("{0} nr=\"{1}\" >", xmlMarker.figureEnd, nr);
            Comment(xml);
            if (loggerTrace) cncLogger.RealTimeLog(xml);
        }
    }

    /// <summary>
    /// Finish path
    /// </summary>
    public static void StopPath(string cmt)
    {
        if (loggerTrace) cncLogger.RealTimeLog("  StopPath "+ cmt);

        if (gcodeReduce)
        {
            if (loggerTrace) cncLogger.RealTimeLog("   StopPath get angle");
            gcodeMath.cutAngle = getAngle(lastSetGC, lastGC, 0, 0);

            if (!gcodeMath.isEqual(lastSetGC, lastGC))        //(lastSetGC.X != lastGC.X) || (lastSetGC.Y != lastGC.Y)) // restore last skipped point for accurat G2/G3 use
            {
                if (loggerTrace) cncLogger.RealTimeLog("   StopPath get angle - restore point");
                gcodeMath.cutAngle = getAngle(lastGC, lastSetGC, 0, 0);
                processTangentialAxis(gcodeMath.cutAngleLast, gcodeMath.cutAngle);
                MoveToDashed(lastGC, cmt);
                lastSetGC = lastGC;
            }
        }
        PenUp(cmt + " Stop path");
    }

    /// <summary>
    /// Move to next coordinate
    /// </summary>
    public static void MoveTo(Point coordxy, string cmt)
    {
        if (gcodeMath.isEqual(lastGC, coordxy))        // nothing to do
            return;
        bool rejectPoint = false;

        if (gcodeReduce && IsPathReduceOk)
        {
            double distance = gcodeMath.distancePointToPoint(coordxy, lastSetGC);
            if (distance < gcodeReduceVal)      // discard actual G1 movement
            { rejectPoint = true; }
        }
        if (!gcodeReduce || !rejectPoint)       // write GCode
        {
            if (loggerTrace) cncLogger.RealTimeLog(" MoveTo get angle p1 "+ lastSetGC.X + ";"+ lastSetGC.Y+ "  p2 "+ coordxy.X+ ";"+ coordxy.Y+ "");
            gcodeMath.cutAngle = getAngle(lastSetGC, coordxy, 0, 0);
            posStartAngle = gcodeMath.cutAngle;

            PenDown(cmt + " moveto");                           // also process tangetial axis
            if (loggerTrace) cncLogger.RealTimeLog(" MoveTo X:"+coordxy.X+" Y:"+coordxy.Y+"");
            gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
            gcodeMath.cutAngleLast = gcodeMath.cutAngle;
            MoveToDashed(coordxy, cmt);
            lastSetGC = coordxy;
        }
        lastGC = coordxy;
    }

    public static void MoveToSimple(xyPoint coordxy, string cmt, bool rapid = false)
    { MoveToSimple(new Point(coordxy.X, coordxy.Y), cmt, rapid); }
    public static void MoveToSimple(Point coordxy, string cmt, bool rapid = false)
    {
        if (loggerTrace) cncLogger.RealTimeLog(" MoveToSimple X"+coordxy.X+ " Y" + coordxy.Y + " rapid "+ rapid);

        gcodeMath.cutAngle = gcodeMath.getAngle(lastGC, coordxy, 0, 0); // get and store position
        if (rapid)
        {
            isStartPathIsPending = true;                  // and angle of desired
            posStartPath = coordxy;                     // start-point
            posStartAngle = gcodeMath.cutAngle;                   // Apply G0 on Pen-down, when needed (in Arc or MoveTo 
        }
        else
        {
            posStartAngle = gcodeMath.cutAngle;
            PenDown(cmt + " movetosimple");                           // also process tangetial axis
            gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
            gcodeMath.cutAngleLast = gcodeMath.cutAngle;
            MoveTo(coordxy, cmt);  //gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt);
        }

        lastSetGC = coordxy;
        lastGC = coordxy;
    }

    private static void MoveToDashed(Point coordxy, string cmt)
    {
        if (loggerTrace) cncLogger.RealTimeLog(" MoveToDashed X" + coordxy.X + " Y" + coordxy.Y);

        bool showDashInfo = false;
        string dashInfo = "";

        gcodeDimension[gcodeStringIndex].setDimensionXY(coordxy.X, coordxy.Y);

        if (!CNC_Settings.importLineDashPattern || (PathDashArray.Length <= 1))
        { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); }
        else
        {
            bool penUpG1 = !CNC_Settings.importLineDashPatternG0;
            double dX = coordxy.X - lastGC.X;
            double dY = coordxy.Y - lastGC.Y;
            double xx = lastGC.X, yy = lastGC.Y, dd;
            int i = 0;
            int save = 1000;
            if (dX == 0)
            {
                if (dY > 0)
                {
                    while (yy < coordxy.Y)
                    {
                        if (i >= PathDashArray.Length)
                            i = 0;
                        PenDown("MoveToDashed");
                        dd = PathDashArray[i++];
                        if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                        yy += dd;
                        if (yy < coordxy.Y)
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), dashInfo); }
                        else
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                        if (i >= PathDashArray.Length)
                            i = 0;
                        PenUp("MoveToDashed", false);
                        dd = PathDashArray[i++];
                        yy += dd;
                        if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                        if (yy < coordxy.Y)
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), dashInfo, true);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), dashInfo);
                        }
                        else
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt, true);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                            break;
                        }
                        if (save-- < 0) { Comment("break up dash 3"); break; }
                    }
                }
                else
                {
                    while (yy > coordxy.Y)
                    {
                        if (i >= PathDashArray.Length)
                            i = 0;
                        yy -= PathDashArray[i++];
                        PenDown("MoveToDashed");
                        if (yy > coordxy.Y)
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), cmt); }
                        else
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                        if (i >= PathDashArray.Length)
                            i = 0;
                        PenUp("MoveToDashed", false);
                        yy -= PathDashArray[i++];
                        if (yy > coordxy.Y)
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), cmt, true);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), cmt);
                        }
                        else
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt, true);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                            break;
                        }
                        if (save-- < 0) { Comment("break up dash 4"); break; }
                    }
                }
            }
            else
            {
                double dC = Math.Sqrt(dX * dX + dY * dY);
                double fX = dX / dC;        // factor X
                double fY = dY / dC;
                if (dX > 0)
                {
                    while (xx < coordxy.X)
                    {
                        if (i >= PathDashArray.Length)
                            i = 0;
                        dd = PathDashArray[i++];
                        xx += fX * dd;
                        yy += fY * dd;
                        PenDown("");
                        if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                        if (xx < coordxy.X)
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo); }
                        else
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                        if (i >= PathDashArray.Length)
                            i = 0;
                        dd = PathDashArray[i++];
                        xx += fX * dd;
                        yy += fY * dd;
                        PenUp("", false);
                        if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                        if (xx < coordxy.X)
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo, true);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo);
                        }
                        else
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt, true);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                            break;
                        }
                        if (save-- < 0) { Comment("break up dash 1"); break; }
                    }
                }
                else
                {
                    while (xx > coordxy.X)
                    {
                        if (i >= PathDashArray.Length)
                            i = 0;
                        dd = PathDashArray[i++];
                        xx += fX * dd;
                        yy += fY * dd;
                        PenDown("");
                        if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                        if (xx > coordxy.X)
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo); }
                        else
                        { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                        if (i >= PathDashArray.Length)
                            i = 0;
                        PenUp("", false);
                        dd = PathDashArray[i++];
                        xx += fX * dd;
                        yy += fY * dd;
                        if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                        if (xx > coordxy.X)
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo);
                        }
                        else
                        {
                            if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt);
                            else gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                            break;
                        }
                        if (save-- < 0) { Comment("break up dash 2"); break; }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Draw arc
    /// </summary>
    public static void ArcToCCW(Point coordxy, Point coordij, string cmt)
    {
        Point center = new Point(lastGC.X + coordij.X, lastGC.Y + coordij.Y);
        double offset = +Math.PI / 2;
        if (loggerTrace) cncLogger.RealTimeLog("  Start ArcToCCW G2 X" + coordxy.X + " Y" + coordxy.Y+ " cX X" + center.X + " cY" + center.Y + " ");

        if (gcodeReduce && IsPathReduceOk)                  // restore last skipped point for accurat G2/G3 use
        {
            if (!gcodeMath.isEqual(lastSetGC, lastGC))
            {
                if (loggerTrace) cncLogger.RealTimeLog(" gcodeReduce MoveTo X" + coordxy.X + " Y" + coordxy.Y );
                gcodeMath.cutAngle = getAngle(lastSetGC, lastGC, 0, 0);
                posStartAngle = gcodeMath.cutAngle;
                gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
                gcodeMath.cutAngleLast = gcodeMath.cutAngle;
                MoveToDashed(lastGC, cmt);
            }
        }

        gcodeMath.cutAngle = getAngle(lastGC, center, offset, 0);     // start angle
        posStartAngle = gcodeMath.cutAngle;

        PenDown(cmt + " from ArcToCCW");

        gcodeMath.cutAngle = getAngle(coordxy, center, offset, 2);    // end angle
        if (gcodeMath.isEqual(coordxy, lastGC))             // end = start position? Full circle!
        {
            gcodeMath.cutAngle -= 2 * Math.PI;                      // CCW 360°
        }
        setG2Dimension(3, coordxy, coordij);
        gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
        gcode.Arc(gcodeString[gcodeStringIndex], 3, coordxy, coordij, cmt, IsPathAvoidG23);
        gcodeMath.cutAngleLast = gcodeMath.cutAngle;

        lastSetGC = coordxy;
        lastGC = coordxy;
    }
    public static void Arc(int gnr, float x, float y, float i, float j, string cmt = "", bool avoidG23 = false)
    {
        Point coordxy = new Point(x, y);
        Point center = new Point(lastGC.X + i, lastGC.Y + j);
        double offset = +Math.PI / 2;
        if (loggerTrace) cncLogger.RealTimeLog("  Start Arc G"+gnr+ " X" + coordxy.X + " Y" + coordxy.Y + " cX X" + center.X + " cY" + center.Y + " ");
        if (gnr > 2) { offset = -offset; }

        if (gcodeReduce && IsPathReduceOk)                  // restore last skipped point for accurat G2/G3 use
        {
            if (!gcodeMath.isEqual(lastSetGC, lastGC))
            {
                if (loggerTrace) cncLogger.RealTimeLog("   gcodeReduce MoveTo X"+ lastGC.X+" Y"+ lastGC.Y);
                gcodeMath.cutAngle = getAngle(lastSetGC, lastGC, 0, 0);
                posStartAngle = gcodeMath.cutAngle;
                gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
                gcodeMath.cutAngleLast = gcodeMath.cutAngle;
                MoveToDashed(lastGC, cmt);
            }
        }

        gcodeMath.cutAngle = getAngle(lastGC, center, offset, 0);       // start angle
        posStartAngle = gcodeMath.cutAngle;
        if (loggerTrace) cncLogger.RealTimeLog("   Start Arc alpha"+ (180 * gcodeMath.cutAngle / Math.PI).ToString() +" offset "+( 180 * offset / Math.PI).ToString());

        PenDown(cmt + " from Arc");

        gcodeMath.cutAngle = getAngle(coordxy, center, offset, gnr);  // end angle
        if (gcodeMath.isEqual(coordxy, lastGC))             // end = start position? Full circle!
        {
            if (gnr > 2)
                gcodeMath.cutAngle += 2 * Math.PI;                  // CW 360°
            else
                gcodeMath.cutAngle -= 2 * Math.PI;                  // CCW 360°
        }

        setG2Dimension(gnr, x, y, i, j);
        gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
        gcode.Arc(gcodeString[gcodeStringIndex], gnr, x, y, i, j, cmt, avoidG23);
        gcodeMath.cutAngleLast = gcodeMath.cutAngle;

        lastSetGC = coordxy;
        lastGC = coordxy;
    }

    private static void setG2Dimension(int gnr, double x, double y, double i, double j)
    { setG2Dimension(gnr, new Point(x, y), new Point(i, j)); }
    private static void setG2Dimension(int gnr, Point xy, Point ij)
    {
        ArcProperties arcMove;
        arcMove = gcodeMath.getArcMoveProperties(new xyPoint(lastSetGC), new xyPoint(xy), ij.X, ij.Y, (gnr == 2));

        float x1 = (float)(arcMove.center.X - arcMove.radius);
        float x2 = (float)(arcMove.center.X + arcMove.radius);
        float y1 = (float)(arcMove.center.Y - arcMove.radius);
        float y2 = (float)(arcMove.center.Y + arcMove.radius);
        float r2 = 2 * (float)arcMove.radius;
        float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
        float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);
        gcodeDimension[gcodeStringIndex].setDimensionCircle(arcMove.center.X, arcMove.center.Y, arcMove.radius, aStart, aDiff);        // calculate new dimensions
    }
    private static void processTangentialAxis(double angleOld, double angleNew)
    {
        if (gcodeTangEnable)
        {
            double angleDiff = 180 * Math.Abs(angleNew - angleOld) / Math.PI;
            double swivelAngle = (double)CNC_Settings.importGCTangentialAngle;
            if (angleDiff > swivelAngle)
            {   // do pen up, turn, pen down
                if (penIsDown)
                {
                    string cmt = "";
                    if (comments) { cmt = "Tangential axis PenUp"; }
                    bool tmp = gcode.RepeatZ;
                    gcode.RepeatZ = false;              // doesn't solve the problem with repeatZ
                    if (loggerTrace) cncLogger.RealTimeLog("processTangentialAxis PenUp");
                    gcode.PenUp(gcodeString[gcodeStringIndex], cmt);
                    gcodeString[gcodeStringIndex].AppendFormat("G00 {0}{1:0.000} (a > {2:0.0})\r\n", gcodeTangName, (gcodeTangTurn / 2) * angleNew / Math.PI, swivelAngle);
                    if (comments) { cmt = "Tangential axis PenDown"; }
                    if (loggerTrace) cncLogger.RealTimeLog("processTangentialAxis PenDown");
                    gcode.PenDown(gcodeString[gcodeStringIndex], cmt);
                    gcodeMath.cutAngleLast = angleNew;
                    gcode.RepeatZ = tmp;
                }
            }
            else
            {   // just turn
                if (angleDiff != 0)
                {
                    string tmp = "";
                    if (comments)
                        tmp = " (Tangential axis)";
                    gcodeString[gcodeStringIndex].AppendFormat("G01 {0}{1:0.000}{2}\r\n", gcodeTangName, (gcodeTangTurn / 2) * angleNew / Math.PI, tmp);
                    gcodeMath.cutAngleLast = angleNew;
                }
            }
        }
    }

    private static double getAngle(Point a, Point b, double offset, int dir)
    {
        if (!gcodeTangEnable)
            return 0;
        double w = gcodeMath.getAngle(a, b, offset, dir);           //monitorAngle(gcodeMath.getAlpha(a, b) + offset, dir);
        if (loggerTrace) cncLogger.RealTimeLog("   getAngle p1 " + a.X +":"+ a.Y +"  p2 " + b.X + ":" + b.Y + " a "+(180 * w / Math.PI).ToString());
        return w;
    }


    /// <summary>
    /// set new index for code
    /// </summary>
    public static void SetGroup(int grp)
    {
        if (lastSetGroup == grp)    // nothing to do
            return;
        lastSetGroup = grp;
        PenUp("SetGroup");
        if (!IsPathFigureEnd)
        { SetFigureEndTag(PathCount); } //Comment(xmlMarker.figureEnd + " " + PathCount + ">");  }
        IsPathFigureEnd = true;

        if (groupObjects)
        {
            if ((grp >= 0) && (grp < gcodeStringMax))
                gcodeStringIndex = grp;
            else
            {
                gcode.Comment(gcodeString[gcodeStringIndex], "[plotter - setGroup] new index out of range");
                cncLogger.Warn(string.Format("setGroup - new gcodeStringIndex out of range: "+ grp));
            }
        }
    }

    /// <summary>
    /// add header and footer, return string of gcode
    /// </summary>
    public static string FinalGCode(string titel, string file)
    {
        cncLogger.RealTimeLog("FinalGCode() ");
        gcode.docTitle = DocTitle;
        gcode.docDescription = DocDescription;
        string header = string.Format("( Use case: {0} )\r\n", CNC_Settings.useCaseLastLoaded);
        header += gcode.GetHeader(titel, file);

        string footer = gcode.GetFooter();
        string output = "";
        if (gcodeTangEnable)
            footer = string.Format("G00 {0}{1:0.000} ({2})\r\n", gcodeTangName, 0, "Tangential axis move to zero") + footer;

        if (CNC_Settings.importRepeatEnable)      // repeat code x times
        {
            for (int i = 0; i < CNC_Settings.importRepeatCnt; i++)
                output += finalGcodeString.ToString().Replace(',', '.');

            return header + output + footer;
        }
        else
            return header + finalGcodeString.ToString().Replace(',', '.') + footer;
    }

    /// <summary>
    /// add additional header info
    /// </summary>
    public static void AddToHeader(string cmt)
    { gcode.AddToHeader(cmt); }

    /// <summary>
    /// return figure end tag string
    /// </summary>
    public static string SetFigureEnd(int nr)
    { return string.Format("{0} {1}>", xmlMarker.figureEnd, nr); }

    /// <summary>
    /// Insert Pen-up gcode command
    /// </summary>
    public static bool PenUp(string cmt = "", bool endFigure = true)
    {
        if (loggerTrace) cncLogger.RealTimeLog("  PenUp "+ cmt);

        if (!comments)
            cmt = "";
        bool penWasDown = penIsDown;
        if (penIsDown)
        { gcode.PenUp(gcodeString[gcodeStringIndex], cmt); }
        penIsDown = false;

        if (endFigure)
        {
            if ((Plotter.PathCount > 0) && !Plotter.IsPathFigureEnd)
                SetFigureEndTag(PathCount); //Plotter.Comment(xmlMarker.figureEnd + " " + Plotter.PathCount + ">");   // finish old index first
            Plotter.IsPathFigureEnd = true;
        }
        return penWasDown;
    }

    /// <summary>
    /// Insert Pen-down gcode command
    /// </summary>
    public static void PenDown(string cmt)
    {
        if (loggerTrace) cncLogger.RealTimeLog(" PenDown "+ cmt);
        if (!comments)
            cmt = "";

        if (!penIsDown)
        {
            if (isStartPathIsPending)
            {
                if (loggerTrace) cncLogger.RealTimeLog("  PenDown - MoveToRapid X"+posStartPath.X+" Y" + posStartPath.Y + " alpha"+ (180 * posStartAngle / Math.PI).ToString());
                gcode.setTangential(gcodeString[gcodeStringIndex], 180 * posStartAngle / Math.PI);
                gcode.MoveToRapid(gcodeString[gcodeStringIndex], posStartPath, cmt);
                gcodeMath.cutAngleLast = gcodeMath.cutAngle;
                isStartPathIsPending = false;
                gcodeMath.cutAngleLast = posStartAngle;
                lastGC = posStartPath;
                lastSetGC = posStartPath;
                gcodeStringIndexOld = gcodeStringIndex;
            }
            if (pauseBeforePenDown) { gcode.Pause(gcodeString[gcodeStringIndex], "Pause before pen down"); }
            gcode.PenDown(gcodeString[gcodeStringIndex], cmt);
        }
        else
            processTangentialAxis(gcodeMath.cutAngleLast, gcodeMath.cutAngle);

        penIsDown = true;
    }

    /// <summary>
    /// Insert tool change command
    /// </summary>
    public static void ToolChange(int toolnr, string cmt = "")
    { gcode.Tool(gcodeString[gcodeStringIndex], toolnr, cmt + " plotter toolchange"); }  // add tool change commands (if enabled) and set XYFeed etc.

    /// <summary>
    /// Insert text with settings via GCodeFromFont.xxx
    /// </summary>
    public static void InsertText(string tmp)
    {
        int oldPathCount = PathCount;
        PathCount = GCodeFromFont.getCode(PathCount, tmp);
        if (PathCount == oldPathCount) { AddToHeader(string.Format("Text insertion failed '{0}' with font '{1}'", GCodeFromFont.gcText, GCodeFromFont.gcFontName)); }
    }

    /// <summary>
    /// Insert M0 into gcode 
    /// </summary>
    public static void InsertPause(string cmt = "")
    { gcode.Pause(gcodeString[gcodeStringIndex], cmt); }

    /// <summary>
    /// set comment
    /// </summary>
    public static void Comment(string cmt)
    { gcode.Comment(gcodeString[gcodeStringIndex], cmt); }
}

public enum xmlMarkerType { none, Group, Figure, Pass, Contour, Fill };
public static class xmlMarker
{
    public const string groupStart = "<Group";
    public const string groupEnd = "</Group";
    public const string figureStart = "<Figure";
    public const string figureEnd = "</Figure";
    public const string passStart = "<Pass";
    public const string passEnd = "</Pass";
    public const string contourStart = "<Contour";
    public const string contourEnd = "</Contour";
    public const string fillStart = "<Fill";
    public const string fillEnd = "</Fill";
    public const string revolutionStart = "<Revolution";
    public const string revolutionEnd = "</Revolution";
    public const string clearanceStart = "<Clearance";
    public const string clearanceEnd = "</Clearance";

    public const string tangentialAxis = "<Tangential";
}