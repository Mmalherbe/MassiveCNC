using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Dimensions;

public static class CNC_Settings 
{
    public static int importBezierLineSegmentsCnt; // Amount of linesegments per Bezier
    public static bool importResizeSVG; // Resize the SVG ?
    public static float importSVGMaxSize; // Max size SVG
    public static bool importSVGPathClose; // Close the SVG path?
    public static bool importSVGToMM;  // Import SVG To MM
    public static bool importSVGNodesOnly; // Only import SVG nodes
    public static bool importSVGGroups; // Import SVG groups
    internal static double importGCTangentialTurn; // Amount of turns per tangetial in GC
    internal static double machineLimitsHomeX; // Home pos X
    internal static double machineLimitsRangeX; // Max pos X
    internal static double machineLimitsHomeY; // Home Pos Y
    internal static double machineLimitsRangeY; // Max pos Y
    internal static float importGCLineSegmentLength;
    internal static bool importGCLineSegmentEquidistant;
    internal static string importGCHeader;
    internal static bool importUnitGCode;
    internal static bool importUnitmm;
    internal static string importGCFooter;
    internal static int importRepeatCnt;
    internal static bool grblTranslateMessage;
    internal static bool ctrl4thUse;
    internal static string ctrl4thName;
    internal static bool importLineDashPattern;
    internal static bool importLineDashPatternG0;
    internal static double importGCTangentialAngle;
    internal static bool importRepeatEnable;
    internal static string useCaseLastLoaded;
    internal static string importGCTangentialAxis;
    internal static bool importGCTangentialEnable;
    internal static float importRemoveShortMovesLimit;
    internal static bool importRemoveShortMovesEnable;
    internal static bool importGroupSortInvert;
    internal static bool importPauseBeforePenDown;
    internal static bool importPauseBeforePath;
    internal static int importGCDecPlaces;
    internal static float importGCXYFeed;
    internal static bool importGCZEnable; // IS Z-AXIS ENABLED ( UP DOWN)
    internal static float importGCZUp;
    internal static float importGCZDown;
    internal static float importGCZFeed;
    internal static bool importGCTTZAxis;
    internal static float importGCZIncrement;
    internal static bool importGCZIncStartZero;
    internal static bool importGCSpindleToggle;
    internal static bool importGCPWMEnable;
    internal static float importGCPWMUp;
    internal static float importGCPWMDlyUp;
    internal static float importGCPWMDown;
    internal static float importGCPWMDlyDown;
    internal static bool importGCIndEnable;
    internal static string importGCIndPenUp;
    internal static string importGCIndPenDown;
    internal static float importGCSegment;
    internal static bool importGCSubEnable; // enable sub routines in g-code?
    internal static bool importGCLineSegmentation;
    internal static bool importGCSubFirst; // subroutines in g-code first before the rest?
    internal static string importGCSubroutine;
    internal static int feedYmax;
    internal static int feedXmax;
    internal static int feedZmax;
    internal static int feedAmax;
    internal static int feedBmax;
    internal static int feedCmax;
    internal static xyzPoint homePos;
    internal static float rotarySubstitutionDiameter;
    internal static bool ctrl4thInvert;
    internal static bool ctrl4thOverX;

    public static class datapath
    {
        public const string fonts = "data\\fonts";
    }

    }

