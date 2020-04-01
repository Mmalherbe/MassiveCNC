using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Dimensions;

public static class CNC_Settings 
{
    public static int importBezierLineSegmentsCnt = 12; // Amount of linesegments per Bezier
    public static bool importResizeSVG =false; // Resize the SVG ?
    public static float importSVGMaxSize = 100; // Max size SVG
    public static bool importSVGPathClose = false; // Close the SVG path?
    public static bool importSVGToMM = false;  // Import SVG To MM
    public static bool importSVGNodesOnly = false; // Only import SVG nodes
    public static bool importSVGGroups = false; // Import SVG groups
    internal static double importGCTangentialTurn = 360; // Amount of turns per tangetial in GC
    internal static double machineLimitsHomeX = 0; // Home pos X
    internal static double machineLimitsRangeX = 200; // Max pos X
    internal static double machineLimitsHomeY = 0; // Home Pos Y
    internal static double machineLimitsRangeY = 200; // Max pos Y
    internal static float importGCLineSegmentLength = 10;
    internal static bool importGCLineSegmentEquidistant = false;
    internal static string importGCHeader = "G54; ";
    internal static bool importUnitGCode = false;
    internal static bool importUnitmm = true;
    internal static string importGCFooter = "M30";
    internal static int importRepeatCnt =2;
    internal static bool grblTranslateMessage = false;
    internal static bool ctrl4thUse = false;
    internal static string ctrl4thName = "A";
    internal static bool importLineDashPattern = false;
    internal static bool importLineDashPatternG0 = false;
    internal static double importGCTangentialAngle = 30;
    internal static bool importRepeatEnable = false;
    internal static string useCaseLastLoaded = "?";
    internal static string importGCTangentialAxis = "A";
    internal static bool importGCTangentialEnable= false;
    internal static float importRemoveShortMovesLimit = 0.1f;
    internal static bool importRemoveShortMovesEnable = true;
    internal static bool importGroupSortInvert = false;
    internal static bool importPauseBeforePenDown = true;
    internal static bool importPauseBeforePath = true;
    internal static int importGCDecPlaces =2000;
    internal static bool importGCZEnable = true; // IS Z-AXIS ENABLED ( UP DOWN)
    internal static float importGCZUp = 2;
    internal static float importGCZDown = -2;
    internal static float importGCZFeed = 1000;
    internal static bool importGCTTZAxis = false;
    internal static float importGCZIncrement =1;
    internal static bool importGCZIncStartZero = false;
    internal static bool importGCSpindleToggle = false;
    internal static bool importGCPWMEnable = false;
    internal static float importGCPWMUp = 200;
    internal static float importGCPWMDlyUp = 0;
    internal static float importGCPWMDown =800;
    internal static float importGCPWMDlyDown = 0;
    internal static bool importGCIndEnable = false;
    internal static string importGCIndPenUp = "(Pen Up command);(Cmd2)";
    internal static string importGCIndPenDown = "(Pen Down command);(Cmd2)";
    internal static float importGCSegment = 1;
    internal static bool importGCSubEnable = false; // enable sub routines in g-code?
    internal static bool importGCLineSegmentation = false;
    internal static bool importGCSubFirst = false; // subroutines in g-code first before the rest?
    internal static string importGCSubroutine = "data\\scripts\\subroutine.nc";
    internal static int feedYmax = 100;
    internal static int feedXmax = 100;
    internal static int feedZmax = 100;
    internal static int feedAmax = 100;
    internal static int feedBmax = 100;
    internal static int feedCmax = 100;
    internal static float importGCXYFeed = 2000;
    internal static xyzPoint homePos;
    internal static float rotarySubstitutionDiameter = 20;
    internal static bool ctrl4thInvert = false;
    internal static bool ctrl4thOverX = false;
   

    public static class datapath
    {
        public const string fonts = "data\\fonts";
    }

    }

