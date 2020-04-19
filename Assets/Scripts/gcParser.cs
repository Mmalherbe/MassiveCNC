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

public class gcParser : MonoBehaviour
{
    [SerializeField] private CNC_Settings Cnc_Settings;
    [SerializeField] private int c = 1;
    [SerializeField] private gcLineBuilder Linebuilder;
    [SerializeField] private LineRenderer XAxis;
    [SerializeField] private LineRenderer YAxis;
    [SerializeField] private LineRenderer ZAxis;
    [SerializeField] internal bool FileLoaded = false;
    [SerializeField] internal bool StartFromHome = true;
    [SerializeField] private GameObject HomePositionObj;
    internal List<gcLine> lineList = new List<gcLine>();
    internal List<string> fileLinebyLine = new List<string>();
    [HideInInspector] public string GCode;



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


    internal void GenerateGcodeFromPath(List<Coords> coords,bool multilineText = false)
    {
        bool notsafe = false;
        List<gcLine> gcodeFromPath = new List<gcLine>();
        float minX = coords.Min(i => i.X);
        float maxX = coords.Max(i => i.X);
        float minY = coords.Min(i => i.Y);
        float maxY = coords.Max(i => i.Y);
        float minZ = coords.Min(i => i.Z);
        float maxZ = coords.Max(i => i.Z);
        float midX = maxX - minX;
        float midY = maxY - minY;
        float midZ = maxZ - minZ;
        if (StartFromHome)
        {
            gcLine gcl = new gcLine();
            gcl.X = HomePositionObj.transform.position.x;
            gcl.Y = HomePositionObj.transform.position.y;
            gcl.Z = HomePositionObj.transform.position.z;
            gcodeFromPath.Add(gcl);
        }
        foreach (Coords coord in coords)
        {
            midX = 0;
            midY = 0;
            midZ = 0;

            gcLine gcl = new gcLine();
            gcl.X = (coord.X - midX);
            gcl.Y = coord.Y - midY;
            gcl.Z = coord.Z - midZ;
            gcl.G = 1;
            gcodeFromPath.Add(gcl);
        }
        gcodeFromPath = fill(gcodeFromPath);

        if (Cnc_Settings.ScaleToMax)
        {
            float scaleMinX = (Cnc_Settings.WidthInMM/2 - Cnc_Settings.HorizontalPaddingInMM) / (minX);
            float scaleMaxX = (Cnc_Settings.WidthInMM/2 - Cnc_Settings.HorizontalPaddingInMM) / (maxX);
            float scaleMinY = (Cnc_Settings.HeightInMM/2 - Cnc_Settings.VerticalPaddingInMM) / minY;
            float scaleMaxY = (Cnc_Settings.HeightInMM/2 - Cnc_Settings.VerticalPaddingInMM) / maxY;

            float[] allscales = { scaleMinX, scaleMaxX, scaleMinY, scaleMaxY };
            float safeToScale = Mathf.Floor(allscales.Min(x => Mathf.Abs(x)));
            Cnc_Settings.ScaleFactorForMax = safeToScale;

            foreach (gcLine gcl in gcodeFromPath)
            {

                gcl.X *= safeToScale;
                gcl.Y *= safeToScale;

                if (Mathf.Abs((float)gcl.X) > Mathf.Abs(((Cnc_Settings.WidthInMM - (Cnc_Settings.HorizontalPaddingInMM * 2)) / 2)) || Mathf.Abs((float)gcl.Y) > Mathf.Abs(((Cnc_Settings.HeightInMM - (Cnc_Settings.VerticalPaddingInMM * 2)) / 2)))
                {
                    notsafe = true;
                }
            }
        }


        if (notsafe)
        {
            Debug.LogError("Code was not safe. Either reached the X or Y limit");
        }
        else
        {
            Debug.Log("show");
            Linebuilder.showOutLinesFromPoints(gcodeFromPath, multilineText);
            gameObject.GetComponent<FileController>().writeFile(gcodeFromPath, "examp");
        }
    }

    List<gcLine> fill(List<gcLine> lines)
    {
        // Make sure every line has coordinates, if they don't give them the coordinates from the previous line. Where -999999 is a value given to a missing value.
        for (int i = 0; i < lines.Count -1; i++)
        {
            if(i == 0)
            {
                if (lines[i].X == -9999999) lines[i].X = HomePositionObj.transform.position.x;
                if (lines[i].Y == -9999999) lines[i].Y = HomePositionObj.transform.position.z;
                if (lines[i].Z == -9999999) lines[i].Z = HomePositionObj.transform.position.y;
            }
            if (lines[i].X == -9999999) lines[i].X = lines[i - 1].X;
            if (lines[i].Y == -9999999) lines[i].Y = lines[i - 1].Y == -9999999? 0 : lines[i - 1].Y;
            if (lines[i].Z == -9999999) lines[i].Z = lines[i - 1].Z;
        }
        foreach(gcLine gcl in lines)
        {
            if (gcl.Y == -9999999) gcl.Y = 0;
            if (gcl.Z == -9999999) gcl.Z = 0;
            if (gcl.X == -9999999) gcl.X = 0;
        }
        return lines;
    }


}