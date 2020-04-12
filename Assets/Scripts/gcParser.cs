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
        if (FileLoaded == true)
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


    string getValue(string gCodeLine,string letter, string splitAt)
    {
        if (gCodeLine.IndexOf(letter) == -1) { return ""; }
            return  gCodeLine.Substring(gCodeLine.IndexOf(letter) +1, gCodeLine.IndexOf(splitAt, gCodeLine.IndexOf(letter)));
    }
    void Organize()
    {//Initializing arrays to fill
        int c = -1; // counter for line number
        foreach(string line in fileLinebyLine)
        {
            gcLine gcl = new gcLine();
            gcl.linenr = c++;
            gcl.G =  int.Parse(getValue(line, "G", " "));
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
        fill();
        FileLoaded = true;
        Linebuilder.buildlines();
    }
    
    void fill()
    {
        // Make sure every line has coordinates, if they don't give them the coordinates from the previous line. Where -999999 is a value given to a missing value.
        for (int i =0; i< lineList.Count; i++)
        {
            if (lineList[i].X == -999999) lineList[i].X = lineList[i - 1].X;
            if (lineList[i].Y == -999999) lineList[i].Y = lineList[i - 1].Y;
            if (lineList[i].Z == -999999) lineList[i].Z = lineList[i - 1].Z;

        }

    }

    
}