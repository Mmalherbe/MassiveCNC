// These are the libraries used in this code

using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using Assets.Scripts.classes;
using System.Collections.Generic;
using TMPro;
using System.Drawing.Drawing2D;
using System.Drawing;
using FontStyle = System.Drawing.FontStyle;
using Random = UnityEngine.Random;

public class gcParser : MonoBehaviour
{
    [SerializeField] private CNC_Settings Cnc_Settings;
    [SerializeField] private interactionController Interaction_Controller;
    [SerializeField] private int c = 1;
    [SerializeField] private gcLineBuilder Linebuilder;
    [SerializeField] private LineRenderer XAxis;
    [SerializeField] private LineRenderer YAxis;
    [SerializeField] private LineRenderer ZAxis;
    [SerializeField] internal bool FileLoaded = false;
    [SerializeField] internal bool StartFromHome = true;
    [SerializeField] internal bool ReturnToHome = true;
    [SerializeField] internal bool AUXOnInFigures = true;
    [SerializeField] internal bool AUXOnInTravels = true;
    [SerializeField] internal bool StretchLines = true;

    [SerializeField] internal GameObject HomePositionObj;
    [SerializeField] internal GameObject MiddlePointGcode;
    [SerializeField] internal GameObject StartPositionGcode;
    internal List<gcLine> lineList = new List<gcLine>();
    internal List<string> fileLinebyLine = new List<string>();
    [HideInInspector] public string GCode;
    internal float minScaleHorizontal = 0.01f;
    internal float maxScaleHorizontal = 1.001f;
    internal float minScaleVertical = 0.01f;
    internal float maxScaleVertical = 1.001f;
    internal float scaleToUseHorizontal = 1f;
    internal float scaleToUseVertical = 1f;

    internal List<Coords> OriginalCoords = new List<Coords>();
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;


    string getValue(string gCodeLine, string letter, string splitAt)
    {
        if (gCodeLine.IndexOf(letter) == -1) { return "-9999999"; }
        int index = gCodeLine.IndexOf(letter) + 1;
        int length = gCodeLine.IndexOf(splitAt, gCodeLine.IndexOf(letter));
        if (length == -1) // if length returns -1, get till the end of the line
            return gCodeLine.Substring(index);
        return gCodeLine.Substring(index, length - index);
    }
    internal void ParseFromGcodeFile()
    {//Initializing arrays to fill
        int c = -1; // counter for line number
        foreach (string line in fileLinebyLine)
        {
            gcLine gcl = new gcLine();
            gcl.linenr = c++;
            gcl.G = int.Parse(getValue(line, "G", " "));
            gcl.X = float.Parse(getValue(line, "X", " "));
            gcl.Y = float.Parse(getValue(line, "Y", " "));
            gcl.Z = float.Parse(getValue(line, "Z", " "));
            gcl.F = float.Parse(getValue(line, "F", " "));
            gcl.I = float.Parse(getValue(line, "I", " "));
            gcl.J = float.Parse(getValue(line, "J", " "));
            gcl.K = float.Parse(getValue(line, "K", " "));
            gcl.L = float.Parse(getValue(line, "L", " "));
            gcl.N = float.Parse(getValue(line, "B", " "));
            gcl.P = float.Parse(getValue(line, "P", " "));
            gcl.R = float.Parse(getValue(line, "R", " "));
            gcl.S = float.Parse(getValue(line, "S", " "));
            gcl.T = float.Parse(getValue(line, "T", " "));
            lineList.Add(gcl);
        }
        lineList = fill(lineList);
        FileLoaded = true;
        Linebuilder.buildlinesFromGcode();
    }
    internal void RedrawWithUpdatedScale()
    {
        if (OriginalCoords.Count != 0)
            GenerateGcodeFromPath();
    }
    internal float[] getMinMaxValues()
    {
        if (OriginalCoords.Count == 0) return new float[] { 0, 1, 0, 1 };
        return new float[] { OriginalCoords.Min(x => x.X) * scaleToUseHorizontal, OriginalCoords.Max(x => x.X) * scaleToUseHorizontal, OriginalCoords.Min(x => x.Y) * scaleToUseVertical, OriginalCoords.Max(x => x.Y) * scaleToUseVertical };
    }

    internal void SetCoordsAndMultiLine(List<Coords> coords, bool multiline = false)
    {
        ResetScales();
        Linebuilder.ClearLines();
        OriginalCoords = coords;

    }

    private void ResetScales()
    {
        scaleToUseVertical = 1f;
        scaleToUseHorizontal = 1f;
        maxScaleHorizontal = 1f;
        maxScaleVertical = 1f;
        Interaction_Controller.scaleSet = false;
        Interaction_Controller.updateScaleSliders(maxScaleHorizontal, maxScaleVertical, scaleToUseHorizontal, scaleToUseVertical);

    }
    internal void GenerateGcodeFromPath()
    {
        List<Coords> coords = OriginalCoords;
        if (coords.Count == 0) return;
        bool notsafe = false;
        List<gcLine> gcodeFromPath = new List<gcLine>();
        minX = coords.Min(i => i.X);
        maxX = coords.Max(i => i.X);
        minY = coords.Min(i => i.Y);
        maxY = coords.Max(i => i.Y);
        float minZ = coords.Min(i => i.Z);
        float maxZ = coords.Max(i => i.Z);
        float midX = maxX - minX;
        float midY = maxY - minY;
        float midZ = maxZ - minZ;

        minScaleHorizontal = Mathf.Floor((Cnc_Settings.WidthInMM / 2 - Cnc_Settings.HorizontalPaddingInMM) / (minX));
        maxScaleHorizontal = Mathf.Floor((Cnc_Settings.WidthInMM / 2 - Cnc_Settings.HorizontalPaddingInMM) / (maxX));
        minScaleVertical = Mathf.Floor((Cnc_Settings.HeightInMM / 2 - Cnc_Settings.VerticalPaddingInMM) / minY);
        maxScaleVertical = Mathf.Floor((Cnc_Settings.HeightInMM / 2 - Cnc_Settings.VerticalPaddingInMM) / maxY);
        float[] allscales = { minScaleHorizontal, maxScaleHorizontal, minScaleVertical, maxScaleVertical };
        float safeToScale = Mathf.Floor(allscales.Min(x => Mathf.Abs(x)));
        Cnc_Settings.ScaleFactorForMax = safeToScale;
        if (!Interaction_Controller.scaleSet)
        {
            Interaction_Controller.updateScaleSliders(maxScaleHorizontal, maxScaleVertical, scaleToUseHorizontal, scaleToUseVertical);
            scaleToUseHorizontal = safeToScale * (Cnc_Settings.defaultScalePercentage/100);
            scaleToUseVertical = safeToScale * (Cnc_Settings.defaultScalePercentage/100);
        }
        if (StartFromHome)
        {
            gcLine gcl = new gcLine();
            gcl.X = HomePositionObj.transform.position.x;
            gcl.Y = HomePositionObj.transform.position.y;
            gcl.Z = HomePositionObj.transform.position.z;
            gcl.AUX1 = AUXOnInTravels;
            gcodeFromPath.Add(gcl);
        }
        for(int i = 0; i < coords.Count;i++)
        {
          midX = 0;
            midY = 0;
            midZ = 0;
            gcLine gcl = new gcLine();
            gcl.X = float.Parse((coords[i].X - midX).ToString("F4"));
            gcl.Y = float.Parse((coords[i].Y - midY).ToString("F4"));
            gcl.Z = float.Parse((coords[i].Z - midZ).ToString("F4"));
            gcl.F = 20000f;
            gcl.G = 1;
            gcl.AUX1 = AUXOnInFigures;
            if (coords.IndexOf(coords.First(x => x.X == coords[i].X && x.Y == coords[i].Y && x.Z == coords[i].Z)) != i)
            {
                gcl.AUX1 = AUXOnInTravels;
            }
            gcodeFromPath.Add(gcl);

        }
        if (ReturnToHome)
        {
            gcLine gcl = new gcLine();
            gcl.X = HomePositionObj.transform.position.x;
            gcl.Y = HomePositionObj.transform.position.y;
            gcl.Z = HomePositionObj.transform.position.z;
            gcl.AUX1 = AUXOnInTravels;
            gcodeFromPath.Add(gcl);
        }

        gcodeFromPath = fill(gcodeFromPath);

        if (Cnc_Settings.ScaleToMax)
        {
            scaleToUseHorizontal = Cnc_Settings.ScaleFactorForMax;
            scaleToUseVertical = Cnc_Settings.ScaleFactorForMax;
        }
        float midPointX = MiddlePointGcode.transform.position.x;
        float midPointY = MiddlePointGcode.transform.position.y;
        float midPointZ = MiddlePointGcode.transform.position.z;

        if (StretchLines)
        {
            Dictionary<int, gcLine> StretchLineToAdd = new Dictionary<int, gcLine>();
            for(int i =1; i < gcodeFromPath.Count; i++)
            {
                float xStart = (float)gcodeFromPath[i - 1].X;
                float xEnd = (float)gcodeFromPath[i].X;
                float yStart = (float)gcodeFromPath[i - 1].Y;
                float yEnd = (float)gcodeFromPath[i].Y;
                // y = a*x+b
                // a = deltax/deltay
                // b = (insert 1 coordinate)..

                float deltaX = xEnd - xStart;
                float deltaY = yEnd - yStart;
                float a = deltaX / deltaY;
                float b = (yStart) / (a * xStart);

                float newStartX = Random.Range((Cnc_Settings.WidthInMM / 2) * (xStart < xEnd ? -1 : 1), xStart);
                float newStartY = a * newStartX + b;
                StretchLineToAdd.Add(i - 1, new gcLine
                {
                    G = 1,
                    X = newStartX,
                    Y = newStartY,
                    Z = gcodeFromPath[i - 1].Z,
                    AUX1 = AUXOnInTravels
                });

            }
            foreach(KeyValuePair<int,gcLine> kvp in StretchLineToAdd)
            {
                gcodeFromPath.Insert(kvp.Key, kvp.Value);
            }
        }

        foreach (gcLine gcl in gcodeFromPath)
        {

            gcl.X *= scaleToUseHorizontal;
            gcl.Y *= scaleToUseVertical;
            gcl.X += midPointX;
            gcl.Y += midPointY;


            if (Mathf.Abs((float)gcl.X) > Mathf.Abs(((Cnc_Settings.WidthInMM - (Cnc_Settings.HorizontalPaddingInMM * 2)) / 2)) || Mathf.Abs((float)gcl.Y) > Mathf.Abs(((Cnc_Settings.HeightInMM - (Cnc_Settings.VerticalPaddingInMM * 2)) / 2)))
            {
                notsafe = true;
            }
        }


        if (notsafe)
        {
            Debug.LogError("Code was not safe. Either reached the X or Y limit");

        }
        else
        {

            Interaction_Controller.UpdateMinMaxValues();
            Linebuilder.showOutLinesFromPoints(gcodeFromPath);
            gameObject.GetComponent<FileController>().writeFile(gcodeFromPath, "examp");
        }
    }

    List<gcLine> fill(List<gcLine> lines)
    {
        // Make sure every line has coordinates, if they don't give them the coordinates from the previous line. Where -999999 is a value given to a missing value.
        for (int i = 0; i < lines.Count - 1; i++)
        {
            if (i == 0)
            {
                if (lines[i].X == -9999999) lines[i].X = HomePositionObj.transform.position.x;
                if (lines[i].Y == -9999999) lines[i].Y = HomePositionObj.transform.position.z;
                if (lines[i].Z == -9999999) lines[i].Z = HomePositionObj.transform.position.y;
            }
            if (lines[i].X == -9999999) lines[i].X = lines[i - 1].X;
            if (lines[i].Y == -9999999) lines[i].Y = lines[i - 1].Y == -9999999 ? 0 : lines[i - 1].Y;
            if (lines[i].Z == -9999999) lines[i].Z = lines[i - 1].Z;
        }
        foreach (gcLine gcl in lines)
        {
            if (gcl.Y == -9999999) gcl.Y = 0;
            if (gcl.Z == -9999999) gcl.Z = 0;
            if (gcl.X == -9999999) gcl.X = 0;
        }
        return lines;
    }


}