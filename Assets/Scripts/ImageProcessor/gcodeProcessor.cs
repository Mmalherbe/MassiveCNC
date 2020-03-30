using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Dimensions;

namespace Assets.Scripts.ImageProcessor
{
    class gcodeProcessor
    {
        private static modalGroup modal;
        private static List<gcodeByLine> gcodeList;
        private static List<gcodeByLine> simuList;
        private static List<coordByLine> coordList;
        private static List<coordByLine> centerList;
        public static Dimensions G0Size = new Dimensions();
        private static int lastFigureNumber;
        private static int figureMarkerCount;
        private static int onlyZ = 0;
        private static int figureCount = 0;
        private static int gcodeMinutes;
        private static int gcodeDistance;
        private static int feedXmax;
        private static int feedYmax;
        private static int feedZmax;
        private static int feedAmax;
        private static int feedBmax;
        private static int feedCmax;
        private static int[] lastSubroutine = new int[] { 0, 0, 0 };
        private static bool tangentialAxisEnable = false;
        private static string tangentialAxisName = "C";
        private static gcodeByLine oldLine = new gcodeByLine();    // actual parsed line
        private static gcodeByLine newLine = new gcodeByLine();    // last parsed line
        public static Dimensions xyzSize = new Dimensions();
        public static GraphicsPath pathPenUp = new GraphicsPath();
        public static GraphicsPath pathPenDown = new GraphicsPath();
        public static GraphicsPath pathRuler = new GraphicsPath();
        public static GraphicsPath pathTool = new GraphicsPath();
        public static GraphicsPath pathMarker = new GraphicsPath();
        public static GraphicsPath pathHeightMap = new GraphicsPath();
        public static GraphicsPath pathMachineLimit = new GraphicsPath();
        public static GraphicsPath pathToolTable = new GraphicsPath();
        public static GraphicsPath pathBackground = new GraphicsPath();
        public static GraphicsPath pathMarkSelection = new GraphicsPath();
        public static GraphicsPath pathRotaryInfo = new GraphicsPath();
        public static GraphicsPath pathDimension = new GraphicsPath();
        public static GraphicsPath path = pathPenUp;
        public static void getGCodeLines(IList<string> oldCode, bool processSubs = false)
        {
#if (debuginfo)
            log.Add("   GCodeVisu getGCodeLines");
            File.WriteAllText("logfile.txt", "");
#endif
            string[] GCode = oldCode.ToArray<string>();
            string singleLine;
            modal = new modalGroup();               // clear

            gcodeList = new List<gcodeByLine>();    //.Clear();
            simuList = new List<gcodeByLine>();    //.Clear();
            coordList = new List<coordByLine>();    //.Clear();
            centerList = new List<coordByLine>();    //.Clear();

            figureMarkerCount = 0;
            lastFigureNumber = -1;
            //            int figureMarkerLine = 0;
            bool figureActive = false;
            gcodeMinutes = 0;
            gcodeDistance = 0;
            feedXmax = CNC_Settings.feedXmax;
            feedYmax = CNC_Settings.feedYmax;
            feedZmax = CNC_Settings.feedZmax;
            feedAmax = CNC_Settings.feedAmax;
            feedBmax = CNC_Settings.feedBmax;
            feedCmax = CNC_Settings.feedCmax;

            oldLine.resetAll(CNC_Settings.homePos);         // reset coordinates and parser modes, set initial pos
            newLine.resetAll();                     // reset coordinates and parser modes
            bool programEnd = false;
            figureCount = 1;                        // will be inc. in createDrawingPathFromGCode
            bool isArc = false;
            bool upDateFigure = false;
            tangentialAxisEnable = false;
            //            figureCountNr.Clear();

            for (int lineNr = 0; lineNr < GCode.Length; lineNr++)   // go through all gcode lines
            {
                modal.resetSubroutine();                            // reset m, p, o, l Word
                singleLine = GCode[lineNr].ToUpper().Trim();        // get line, remove unneeded chars
                if (singleLine == "")
                    continue;

                if (GCode[lineNr].Contains(xmlMarker.figureStart))                    // check if marker available
                {
                    figureMarkerCount++;
                    figureActive = true;
                }
                if (GCode[lineNr].Contains(xmlMarker.tangentialAxis))
                {
                    tangentialAxisEnable = true;
                    tangentialAxisName = GCode[lineNr].Substring(xmlMarker.tangentialAxis.Length + 2, 1);
                    cncLogger.RealTimeLog("Show tangetial axis '"+tangentialAxisName+"'");
                }



                if (processSubs && programEnd)
                { singleLine = "( " + singleLine + " )"; }          // don't process subroutine itself when processed

                newLine.parseLine(lineNr, singleLine, ref modal);
                calcAbsPosition(newLine, oldLine);                  // Calc. absolute positions and set object dimension: xyzSize.setDimension

                if (figureMarkerCount > 0)                          // preset figure nr
                    newLine.figureNumber = figureMarkerCount;

                if ((modal.mWord == 98) && processSubs)
                    newLine.codeLine = "(" + GCode[lineNr] + ")";
                else
                {
                    if (processSubs && programEnd)
                        newLine.codeLine = "( " + GCode[lineNr] + " )";   // don't process subroutine itself when processed
                    else
                        newLine.codeLine = GCode[lineNr];                 // store original line
                }

                if (!programEnd)
                {
                    upDateFigure = createDrawingPathFromGCode(newLine, oldLine);        // add data to drawing path
                    calculateProcessTime(newLine, oldLine);
                    //                Logger.Info("g {0} x {1} y {2}",newLine.motionMode,newLine.actualPos.X,newLine.actualPos.Y);
                }
                if (figureMarkerCount > 0)
                {
                    if (figureActive)
                        newLine.figureNumber = figureMarkerCount;
                    else
                        newLine.figureNumber = -1;
                }
                isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                if (tangentialAxisEnable)
                {
                    if (tangentialAxisName == "C") { newLine.alpha = Math.PI * newLine.actualPos.C / 180; }
                    else if (tangentialAxisName == "B") { newLine.alpha = Math.PI * newLine.actualPos.B / 180; }
                    else if (tangentialAxisName == "A") { newLine.alpha = Math.PI * newLine.actualPos.A / 180; }
                    else if (tangentialAxisName == "Z") { newLine.alpha = Math.PI * newLine.actualPos.Z / 180; }
                    //                    else if (tangentialAxisName == "U") { newLine.alpha = Math.PI * newLine.actualPos.U / 180; }
                    //                    else if (tangentialAxisName == "V") { newLine.alpha = Math.PI * newLine.actualPos.V / 180; }
                    //                    else if (tangentialAxisName == "W") { newLine.alpha = Math.PI * newLine.actualPos.W / 180; }
                }

                oldLine = new gcodeByLine(newLine);                     // get copy of newLine      
                gcodeList.Add(new gcodeByLine(newLine));                // add parsed line to list
                simuList.Add(new gcodeByLine(newLine));                // add parsed line to list
                coordList.Add(new coordByLine(lineNr, newLine.figureNumber, (xyPoint)newLine.actualPos, newLine.alpha, isArc));
#if (debuginfo)
                File.AppendAllText("logfile.txt",lineNr+"  "+ newLine.figureNumber+"  "+ newLine.actualPos.X+"  "+ newLine.actualPos.Y + "\r");
#endif
                if ((modal.mWord == 30) || (modal.mWord == 2)) { programEnd = true; }
                if (modal.mWord == 98)
                {
                    if (lastSubroutine[0] == modal.pWord)
                        addSubroutine(GCode, lastSubroutine[1], lastSubroutine[2], modal.lWord, processSubs);
                    else
                        findAddSubroutine(modal.pWord, GCode, modal.lWord, processSubs);      // scan complete GCode for matching O-word
                }

                if (GCode[lineNr].Contains(xmlMarker.figureEnd))                    // check if marker available
                { figureActive = false; }
            }
        }
        /// <summary>
        /// Calc. absolute positions and set object dimension: xyzSize.setDimension
        /// </summary>
        private static void calcAbsPosition(gcodeByLine newLine, gcodeByLine oldLine)
        {
            if (!newLine.ismachineCoordG53)         // only use world coordinates
            {
                if ((newLine.motionMode >= 1) && (oldLine.motionMode == 0))     // take account of last G0 move
                {
                    xyzSize.setDimensionX(oldLine.actualPos.X);
                    xyzSize.setDimensionY(oldLine.actualPos.Y);
                }
                else
                {
                    G0Size.setDimensionX(newLine.actualPos.X);
                    G0Size.setDimensionY(newLine.actualPos.Y);
                }
                if (newLine.x != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                    {
                        newLine.actualPos.X = (double)newLine.x;
                        if (newLine.motionMode >= 1)//if (newLine.actualPos.X != toolPos.X)            // don't add actual tool pos
                        {
                            xyzSize.setDimensionX(newLine.actualPos.X);
                        }
                    }
                    else
                    {
                        newLine.actualPos.X = oldLine.actualPos.X + (double)newLine.x;
                        if (newLine.motionMode >= 1)//if (newLine.actualPos.X != toolPos.X)            // don't add actual tool pos
                        {
                            xyzSize.setDimensionX(newLine.actualPos.X);// - toolPosX);
                        }
                    }
                }
                else
                    newLine.actualPos.X = oldLine.actualPos.X;

                if (newLine.y != null)
                {
                    if (newLine.isdistanceModeG90)
                    {
                        newLine.actualPos.Y = (double)newLine.y;
                        if (newLine.motionMode >= 1)//if (newLine.actualPos.Y != toolPos.Y)            // don't add actual tool pos
                        {
                            xyzSize.setDimensionY(newLine.actualPos.Y);
                        }
                    }
                    else
                    {
                        newLine.actualPos.Y = oldLine.actualPos.Y + (double)newLine.y;
                        if (newLine.motionMode >= 1)//if (newLine.actualPos.Y != toolPos.Y)            // don't add actual tool pos
                        {
                            xyzSize.setDimensionY(newLine.actualPos.Y);// - toolPosY);
                        }
                    }
                }
                else
                    newLine.actualPos.Y = oldLine.actualPos.Y;

                if (newLine.z != null)
                {
                    if (newLine.isdistanceModeG90)
                    {
                        newLine.actualPos.Z = (double)newLine.z;
                       
                    }
                    else
                    {
                        newLine.actualPos.Z = oldLine.actualPos.Z + (double)newLine.z;
                    }
                }
                else
                    newLine.actualPos.Z = oldLine.actualPos.Z;

                if (newLine.a != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.A = (double)newLine.a;
                    else
                        newLine.actualPos.A = oldLine.actualPos.A + (double)newLine.a;
                }
                else
                    newLine.actualPos.A = oldLine.actualPos.A;

                if (newLine.b != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.B = (double)newLine.b;
                    else
                        newLine.actualPos.B = oldLine.actualPos.B + (double)newLine.b;
                }
                else
                    newLine.actualPos.B = oldLine.actualPos.B;

                if (newLine.c != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.C = (double)newLine.c;
                    else
                        newLine.actualPos.C = oldLine.actualPos.C + (double)newLine.c;
                }
                else
                    newLine.actualPos.C = oldLine.actualPos.C;

                if (newLine.u != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.U = (double)newLine.u;
                    else
                        newLine.actualPos.U = oldLine.actualPos.U + (double)newLine.u;
                }
                else
                    newLine.actualPos.U = oldLine.actualPos.U;

                if (newLine.v != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.V = (double)newLine.v;
                    else
                        newLine.actualPos.V = oldLine.actualPos.V + (double)newLine.v;
                }
                else
                    newLine.actualPos.V = oldLine.actualPos.V;

                if (newLine.w != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.W = (double)newLine.w;
                    else
                        newLine.actualPos.W = oldLine.actualPos.W + (double)newLine.w;
                }
                else
                    newLine.actualPos.W = oldLine.actualPos.W;
            }
            newLine.alpha = oldLine.alpha;
            if (((xyPoint)oldLine.actualPos).DistanceTo((xyPoint)newLine.actualPos) != 0)
                newLine.alpha = gcodeMath.getAlpha((xyPoint)oldLine.actualPos, (xyPoint)newLine.actualPos);
        }
        private static void markPath(GraphicsPath path, float x, float y, int type)
        {
            float markerSize = 1;
            if (!CNC_Settings.importUnitmm)
            { markerSize /= 25.4F; }
            createMarker(path, x, y, markerSize, type, false);    // draw circle
        }

        private static string findAddSubroutine(int foundP, string[] GCode, int repeat, bool processSubs)
        {
            modalGroup tmp = new modalGroup();                      // just temporary use
            gcodeByLine tmpLine = new gcodeByLine();                // just temporary use
            int subStart = 0, subEnd = 0;
            bool foundO = false;
            for (int lineNr = 0; lineNr < GCode.Length; lineNr++)   // go through GCode lines
            {
                tmpLine.parseLine(lineNr, GCode[lineNr], ref tmp);       // parse line
                if (tmp.oWord == foundP)                            // subroutine ID found?
                {
                    if (!foundO)
                    {
                        subStart = lineNr;
                        foundO = true;
                    }
                    else
                    {
                        if (tmp.mWord == 99)                        // subroutine end found?
                        {
                            subEnd = lineNr;
                            break;
                        }
                    }
                }
            }
            if ((subStart > 0) && (subEnd > subStart))
            {
                addSubroutine(GCode, subStart, subEnd, repeat, processSubs);    // process subroutine
                lastSubroutine[0] = foundP;
                lastSubroutine[1] = subStart;
                lastSubroutine[2] = subEnd;
            }
            return String.Format("Start:{0} EndX:{1} ", subStart, subEnd);
        }

        private static bool createDrawingPathFromGCode(gcodeByLine newL, gcodeByLine oldL)
        {
            bool passLimit = false;
            bool figureStart = false;
            var pathOld = path;
            bool xyMove = ((newL.x != null) || (newL.y != 0));

            if (newL.isSubroutine && (!oldL.isSubroutine))
                markPath(pathPenUp, (float)newL.actualPos.X, (float)newL.actualPos.Y, 2); // 2=rectangle

            if (!newL.ismachineCoordG53)
            {
                /*      if (newL.codeLine.Contains(xmlMarker.penUp))
                      { path = pathPenUp; path.StartFigure(); }
                      if (newL.codeLine.Contains(xmlMarker.penDown))
                      { path = pathPenDown; path.StartFigure(); } */

                if ((newL.motionMode > 0) && (oldL.motionMode == 0))
                { path = pathPenDown; path.StartFigure(); }
                if ((newL.motionMode == 0) && (oldL.motionMode > 0))
                { path = pathPenUp; path.StartFigure(); }

                if ((path != pathOld))
                {
                    passLimit = true;
                    if (figureMarkerCount <= 0)
                        path.SetMarkers(); //path.StartFigure();
                    else
                    {
                        if (figureMarkerCount != figureCount)
                        { path.SetMarkers(); }// path.StartFigure(); }
                    }
                    if (path == pathPenDown)
                    {
                        figureStart = true;
                        if (figureMarkerCount <= 0)
                        {
                            if (pathPenDown.PointCount > 0)
                            {
                                figureCount++;                  // only inc. if old figure was filled
                                oldL.figureNumber = figureCount;
                            }
                        }
                        else
                        {
                            figureCount = figureMarkerCount;
                            oldL.figureNumber = figureCount;
                        }
                        /*#if (debuginfo)
                                                File.AppendAllText("logfile.txt", ">>>>"+newL.codeLine + "  " + figureCount +"\r" );
                        #endif*/
                    }
                }

                if (newL.motionMode == 0 || newL.motionMode == 1)
                {
                    bool otherAxis = (newL.actualPos.A != oldL.actualPos.A) || (newL.actualPos.B != oldL.actualPos.B) || (newL.actualPos.C != oldL.actualPos.C);
                    otherAxis = otherAxis || (newL.actualPos.U != oldL.actualPos.U) || (newL.actualPos.V != oldL.actualPos.V) || (newL.actualPos.W != oldL.actualPos.W);
                    if ((newL.actualPos.X != oldL.actualPos.X) || (newL.actualPos.Y != oldL.actualPos.Y) || otherAxis || (oldL.motionMode == 2 || oldL.motionMode == 3))
                    {
                        if ((CNC_Settings.ctrl4thUse) && (path == pathPenDown))
                        {
                            if (passLimit)
                                pathRotaryInfo.StartFigure();
                            float scale = (float)CNC_Settings.rotarySubstitutionDiameter * (float)Math.PI / 360;
                            if (CNC_Settings.ctrl4thInvert)
                                scale = scale * -1;

                            float newR = 0, oldR = 0;
                            if (CNC_Settings.ctrl4thName == "A") { oldR = (float)oldL.actualPos.A * scale; newR = (float)newL.actualPos.A * scale; }
                            else if (CNC_Settings.ctrl4thName == "B") { oldR = (float)oldL.actualPos.B * scale; newR = (float)newL.actualPos.B * scale; }
                            else if (CNC_Settings.ctrl4thName == "C") { oldR = (float)oldL.actualPos.C * scale; newR = (float)newL.actualPos.C * scale; }
                            else if (CNC_Settings.ctrl4thName == "U") { oldR = (float)oldL.actualPos.U * scale; newR = (float)newL.actualPos.U * scale; }
                            else if (CNC_Settings.ctrl4thName == "V") { oldR = (float)oldL.actualPos.V * scale; newR = (float)newL.actualPos.V * scale; }
                            else if (CNC_Settings.ctrl4thName == "W") { oldR = (float)oldL.actualPos.W * scale; newR = (float)newL.actualPos.W * scale; }

                            if (CNC_Settings.ctrl4thOverX)
                            {
                                pathRotaryInfo.AddLine((float)oldL.actualPos.X, oldR, (float)newL.actualPos.X, newR); // rotary over X
                                xyzSize.setDimensionY(newR);
                            }
                            else
                            {
                                pathRotaryInfo.AddLine(oldR, (float)oldL.actualPos.Y, newR, (float)newL.actualPos.Y); // rotary over Y
                                xyzSize.setDimensionX(newR);
                            }
                        }
                        path.AddLine((float)oldL.actualPos.X, (float)oldL.actualPos.Y, (float)newL.actualPos.X, (float)newL.actualPos.Y);
                        onlyZ = 0;  // x or y has changed
                    }
                    if (newL.actualPos.Z != oldL.actualPos.Z)  //else
                    { onlyZ++; }

                    // mark Z-only movements - could be drills
                    if ((onlyZ > 1) && (passLimit) && (path == pathPenUp))  // pen moved from -z to +z
                    {
                        float markerSize = 1;
                        if (!CNC_Settings.importUnitmm)
                        { markerSize /= 25.4F; }
                        createMarker(pathPenDown, (xyPoint)newL.actualPos, markerSize, 1, false);       // draw cross
                        createMarker(pathPenUp, (xyPoint)newL.actualPos, markerSize, 4, false);       // draw circle
                        path = pathPenUp;
                        onlyZ = 0;
                        passLimit = false;
                    }
                }
            }
            if ((newL.motionMode == 2 || newL.motionMode == 3) && (newL.i != null || newL.j != null))
            {
                if (newL.i == null) { newL.i = 0; }
                if (newL.j == null) { newL.j = 0; }

                ArcProperties arcMove;
                arcMove = gcodeMath.getArcMoveProperties((xyPoint)oldL.actualPos, (xyPoint)newL.actualPos, newL.i, newL.j, (newL.motionMode == 2));
                centerList.Add(new coordByLine(newL.lineNumber, figureCount, arcMove.center, 0, true));

                newL.distance = Math.Abs(arcMove.radius * arcMove.angleDiff);
                float x1 = (float)(arcMove.center.X - arcMove.radius);
                float x2 = (float)(arcMove.center.X + arcMove.radius);
                float y1 = (float)(arcMove.center.Y - arcMove.radius);
                float y2 = (float)(arcMove.center.Y + arcMove.radius);
                float r2 = 2 * (float)arcMove.radius;
                float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
                float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);
                path.AddArc(x1, y1, r2, r2, aStart, aDiff);
                if (!newL.ismachineCoordG53)
                    xyzSize.setDimensionCircle(arcMove.center.X, arcMove.center.Y, arcMove.radius, aStart, aDiff);        // calculate new dimensions
            }
            if (path == pathPenDown)
                newL.figureNumber = figureCount;
            else
                newL.figureNumber = -1;

            return figureStart;
        }
        private static void calculateProcessTime(gcodeByLine newL, gcodeByLine oldL)
        {
            int feed = Math.Min(feedXmax, feedYmax);         // feed in mm/min
            if (newL.z != null)
                feed = Math.Min(feed, feedZmax);                // max feed defines final speed
            if (newL.a != null)
                feed = Math.Min(feed, feedAmax);                // max feed defines final speed
            if (newL.b != null)
                feed = Math.Min(feed, feedBmax);                // max feed defines final speed
            if (newL.c != null)
                feed = Math.Min(feed, feedCmax);                // max feed defines final speed

            int distanceX = Math.Abs(newL.actualPos.X - oldL.actualPos.X);
            int distanceY = Math.Abs(newL.actualPos.Y - oldL.actualPos.Y);
            int distanceXY = Math.Max(distanceX, distanceY);
            int distanceZ = Math.Abs(newL.actualPos.Z - oldL.actualPos.Z);

            if (newL.motionMode > 1)
                distanceXY = int.Parse(newL.distance.ToString());     // Arc is calc in createDrawingPathFromGCode

            int distanceAll = Math.Max(distanceXY, distanceZ);

            if (newL.motionMode > 0)
                feed = Math.Min(feed, newL.feedRate);           // if G1,2,3 use set feed

            gcodeDistance += distanceAll;
            gcodeMinutes += distanceAll / feed;
        }

        private static void createMarker(GraphicsPath path, float centerX, float centerY, float dimension, int style, bool rst = true)
        {
            if (dimension == 0) { return; }
            if (rst)
                path.Reset();
            if (style == 0)   // horizontal cross
            {
                path.StartFigure(); path.AddLine(centerX, centerY + dimension, centerX, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY, centerX - dimension, centerY);
            }
            else if (style == 1)   // diagonal cross
            {
                path.StartFigure(); path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX - dimension, centerY - dimension, centerX + dimension, centerY + dimension);
            }
            else if (style == 2)            // box
            {
                path.StartFigure();
                path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY + dimension);
                path.AddLine(centerX + dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY - dimension);
                path.AddLine(centerX - dimension, centerY - dimension, centerX - dimension, centerY + dimension);
                path.CloseFigure();
            }
            else if (style == 3)            // marker
            {
                path.StartFigure();
                path.AddLine(centerX, centerY, centerX, centerY - dimension);
                path.AddLine(centerX, centerY - dimension, centerX + dimension, centerY);
                path.AddLine(centerX + dimension, centerY, centerX, centerY + dimension);
                path.AddLine(centerX, centerY + dimension, centerX - dimension, centerY);
                path.AddLine(centerX - dimension, centerY, centerX, centerY - dimension);
                path.CloseFigure();
                //                path.StartFigure(); path.AddLine(centerX, centerY - dimension, centerX, centerY);
            }
            else
            {
                path.StartFigure(); path.AddArc(centerX - dimension, centerY - dimension, 2 * dimension, 2 * dimension, 0, 360);
            }
        }

        /// <summary>
        /// process subroutines
        /// </summary>
        private static void addSubroutine(string[] GCode, int start, int stop, int repeat, bool processSubs)
        {
            bool showPath = true;
            bool isArc = false;
            for (int loop = 0; loop < repeat; loop++)
            {
                for (int subLineNr = start + 1; subLineNr < stop; subLineNr++)      // go through real line numbers and parse sub-code
                {
                    if (GCode[subLineNr].IndexOf("%START_HIDECODE") >= 0) { showPath = false; }
                    if (GCode[subLineNr].IndexOf("%STOP_HIDECODE") >= 0) { showPath = true; }

                    newLine.parseLine(subLineNr, GCode[subLineNr], ref modal);      // reset coordinates, set lineNumber, parse GCode
                    newLine.isSubroutine = !processSubs;
                    calcAbsPosition(newLine, oldLine);                              // calc abs position

                    if (!showPath) newLine.ismachineCoordG53 = true;

                    if (processSubs)
                        gcodeList.Add(new gcodeByLine(newLine));      // add parsed line to list
                    simuList.Add(new gcodeByLine(newLine));      // add parsed line to list
                    if (!newLine.ismachineCoordG53)
                    {
                        isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                        coordList.Add(new coordByLine(subLineNr, newLine.figureNumber, (xyPoint)newLine.actualPos, newLine.alpha, isArc));
                        if (((newLine.motionMode > 0) || (newLine.z != null)) && !((newLine.x == CNC_Settings.homePos.X) && (newLine.y == CNC_Settings.homePos.Y)))
                            xyzSize.setDimensionXYZ(newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);             // calculate max dimensions
                    }                                                                                                       // add data to drawing path
                    if (showPath)
                        createDrawingPathFromGCode(newLine, oldLine);
                    oldLine = new gcodeByLine(newLine);   // get copy of newLine                         
                }
            }
        }

    }
}
