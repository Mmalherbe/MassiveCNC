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
    // calling upon different classes, objects and variables
    public TextMeshProUGUI GCodetext;
    internal List<string> fileLinebyLine = new List<string>();
    string[,] GCodeTabel;
    int b;
    int count;
    public int c;
    public gcLineBuilder Linebuilder;
    public LineRenderer XAxis;
    public LineRenderer YAxis;
    public LineRenderer ZAxis;
    [HideInInspector] public string GCode;
    public bool FileLoaded;
    public List<gcLine> lineList = new List<gcLine>();

    // Initializing start values
    void Start()
    {
        FileLoaded = false;
        c = 1;
        b = 0;
        count = 0;
    }
    void Update()
    {
        //This will show the current g-code statement the X-Carve or tracker is working with
        if (FileLoaded == true && 1==2)
        {
            GCodetext.text =
            "G:" + lineList[c].G + "\n" +
            "X:" + lineList[c].X + "\n" +
            "Y:" + lineList[c].Y + "\n" +
            "Z:" + lineList[c].Z + "\n" +
            "F:" + lineList[c].F + "\n" +
            "I:" + lineList[c].I + "\n" +
            "J:" + lineList[c].J + "\n" +
            "K:" + lineList[c].K + "\n" +
            "L:" + lineList[c].L + "\n" +
            "N:" + lineList[c].N + "\n" +
            "P:" + lineList[c].P + "\n" +
            "R:" + lineList[c].R + "\n" +
            "S:" + lineList[c].S + "\n" +
            "T:" + lineList[c].T + "\n" + count;

        }
    }
    public void Parse()
    {//This will split the string into seperate parts into an array

        Organize();
    }


    string getValue(string gCodeLine, string letter, string splitAt)
    {
        if (gCodeLine.IndexOf(letter) == -1) { return "-9999999"; }
        int index = gCodeLine.IndexOf(letter) + 1;
        int length = gCodeLine.IndexOf(splitAt, gCodeLine.IndexOf(letter));
        if (length == -1) // if length returns -1, get till the end of the line
            return gCodeLine.Substring(index);
        return gCodeLine.Substring(index, length - index);
    }
    void Organize()
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
        poep();
     //   Linebuilder.buildlinesFromGcode();
    }
    void poep()
    {
        GraphicsPath path = new GraphicsPath();

        path.StartFigure();

        path.AddString("Games-XL", new FontFamily("arial"),
          1, 50, new Point(0, 0),
          StringFormat.GenericTypographic);

        path.CloseFigure();
        PointF[] pt = path.PathPoints;
        List<Coords> coords = new List<Coords>();
        foreach (PointF p in pt)
        {
            coords.Add(new Coords() { X = p.X, Y = p.Y, Z = 0 });
        }
        Linebuilder.showOutLinesFromPoints(coords);
        GenerateGcodeFromPath(coords);
    }

    void GenerateGcodeFromPath(List<Coords> coords)
    {
        List<gcLine> gcodeFromPath = new List<gcLine>();
        foreach(Coords coord in coords)
        {
            gcLine gcl = new gcLine();
            gcl.G = 1;
            gcl.X = coord.X;
            gcl.Z = coord.Y;
            gcodeFromPath.Add(gcl);
        }
        gcodeFromPath = fill(gcodeFromPath);
        gameObject.GetComponent<FileController>().writeFile(gcodeFromPath, "examp");
    }

    List<gcLine> fill(List<gcLine> lines)
    {
        // Make sure every line has coordinates, if they don't give them the coordinates from the previous line. Where -999999 is a value given to a missing value.
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].X == -9999999) lines[i].X = lines[i - 1].X;
            if (lines[i].Y == -9999999) lines[i].Y = lines[i - 1].Y;
            if (lines[i].Z == -9999999) lines[i].Z = lines[i - 1].Z;
        }
        return lines;
    }


}