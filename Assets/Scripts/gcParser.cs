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
    string[] SplittedGCode;
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
        SplittedGCode = GCode.Replace("\r","").Split(new string[] { " ", "\n", "" }, StringSplitOptions.RemoveEmptyEntries);
        Organize();
    }
    void Organize()
    {//Initializing arrays to fill
        GCodeTabel = new string[SplittedGCode.Length, 14];
        for (int i = 0;i < SplittedGCode.Length;i++)
        {//This checks what kind of g-statement it is 
            
            if (SplittedGCode[i].StartsWith("G"))
            {
                b++;
                lineList.Add(new gcLine());
                //every g is placed in a new column of the array
                GCodeTabel[b - 1, 0] = SplittedGCode[i];
                //places the code in the riht position of the array
                GCodeTabel[b - 1, 0] = GCodeTabel[b - 1, 0].Remove(0, 1);
                //removes the G in front of it
                lineList[b - 1].G= int.Parse(GCodeTabel[b - 1, 0], System.Globalization.CultureInfo.InvariantCulture);
                // changes it into an double
            }
            else if (SplittedGCode[i].StartsWith("X"))
            {
                GCodeTabel[b - 1, 1] = SplittedGCode[i];
                GCodeTabel[b - 1, 1] = GCodeTabel[b - 1, 1].Remove(0, 1);
                lineList[b - 1].X = float.Parse(GCodeTabel[b - 1, 1], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("Y"))
            {
                GCodeTabel[b - 1, 2] = SplittedGCode[i];
                GCodeTabel[b - 1, 2] = GCodeTabel[b - 1, 2].Remove(0, 1);
                lineList[b - 1].Y = float.Parse(GCodeTabel[b - 1, 2], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("Z"))
            {
                GCodeTabel[b - 1, 3] = SplittedGCode[i];
                GCodeTabel[b - 1, 3] = GCodeTabel[b - 1, 3].Remove(0, 1);
                lineList[b - 1].Z = float.Parse(GCodeTabel[b - 1, 3], System.Globalization.CultureInfo.InvariantCulture);

            }
            else if (SplittedGCode[i].StartsWith("F"))
            {
                GCodeTabel[b - 1, 4] = SplittedGCode[i];
                GCodeTabel[b - 1, 4] = GCodeTabel[b - 1, 4].Remove(0, 1);
                lineList[b - 1].F = float.Parse(GCodeTabel[b - 1, 4], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("I"))
            {
                GCodeTabel[b - 1, 5] = SplittedGCode[i];
                GCodeTabel[b - 1, 5] = GCodeTabel[b - 1, 5].Remove(0, 1);
                lineList[b - 1].I = float.Parse(GCodeTabel[b - 1, 5], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("J"))
            {
                GCodeTabel[b - 1, 6] = SplittedGCode[i];
                GCodeTabel[b - 1, 6] = GCodeTabel[b - 1, 6].Remove(0, 1);
                lineList[b - 1].J = float.Parse(GCodeTabel[b - 1, 6], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("K"))
            {
                GCodeTabel[b - 1, 7] = SplittedGCode[i];
                GCodeTabel[b - 1, 7] = GCodeTabel[b - 1, 7].Remove(0, 1);
                lineList[b - 1].K = float.Parse(GCodeTabel[b - 1, 7], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("L"))
            {
                GCodeTabel[b - 1, 8] = SplittedGCode[i];
                GCodeTabel[b - 1, 8] = GCodeTabel[b - 1, 8].Remove(0, 1);
                lineList[b - 1].L = float.Parse(GCodeTabel[b - 1, 8], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("N"))
            {
                GCodeTabel[b - 1, 9] = SplittedGCode[i];
                GCodeTabel[b - 1, 9] = GCodeTabel[b - 1, 9].Remove(0, 1);
                lineList[b - 1].N = float.Parse(GCodeTabel[b - 1, 9], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("P"))
            {
                GCodeTabel[b - 1, 10] = SplittedGCode[i];
                GCodeTabel[b - 1, 10] = GCodeTabel[b - 1, 10].Remove(0, 1);
                lineList[b - 1].P = float.Parse(GCodeTabel[b - 1, 10], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("R"))
            {
                GCodeTabel[b - 1, 11] = SplittedGCode[i];
                GCodeTabel[b - 1, 11] = GCodeTabel[b - 1, 11].Remove(0, 1);
                lineList[b - 1].R = float.Parse(GCodeTabel[b - 1, 11], System.Globalization.CultureInfo.InvariantCulture);

            }
            else if (SplittedGCode[i].StartsWith("S"))
            {
                GCodeTabel[b - 1, 12] = SplittedGCode[i];
                GCodeTabel[b - 1, 12] = GCodeTabel[b - 1, 12].Remove(0, 1);
                lineList[b - 1].S = float.Parse(GCodeTabel[b - 1, 12], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (SplittedGCode[i].StartsWith("T"))
            {
                GCodeTabel[b - 1, 13] = SplittedGCode[i];
                GCodeTabel[b - 1, 13] = GCodeTabel[b - 1, 13].Remove(0, 1);
                lineList[b - 1].T = float.Parse(GCodeTabel[b - 1, 13], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        fill();
        setAxis();
        FileLoaded = true;
        Linebuilder.buildlines();
    }
    void fill()
    {// every g-statement without coordinates will be given the coordinates of the last statement to ensure the tracker can go through coordinates
        for (int i = 1; i < lineList.Count; i++)
        {
            if (lineList[i].X == 0d)
            {
                lineList[i].X = lineList[i - 1].X;
            }
            if (lineList[i].Y == 0d)
            {
                lineList[i].Y = lineList[i - 1].Y;
            }
            if (lineList[i].Z == 0d)
            {
                lineList[i].Z = lineList[i - 1].Z;
            }
        }
    }
    void setAxis()
    {//The minimal and maximal values of the Axis
        double Xmin = 0.0;
        double Xmax = 100.0;
        double Ymin = 0.0;
        double Ymax = 100.0;
        double Zmin = 0.0;
        double Zmax = 100.0;
        //If a value of a coordinate is below the min or max value it will ebco-me the new value
        for (int i = 0; i < lineList.Count; i++)
        {
            if (lineList[i].X < Xmin)
            {
                Xmin = lineList[i].X;
            }
            else if (lineList[i].X > Xmax)
            {
                Xmax = lineList[i].X;
            }
            if (lineList[i].Z < Zmin)
            {
                Zmin = lineList[i].Z;
            }
            else if (lineList[i].Z > Zmax)
            {
                Zmax = lineList[i].Z;
            }
            if (lineList[i].Y < Ymin)
            {
                Ymin = lineList[i].Y;
            }
            else if (lineList[i].Y > Ymax)
            {
                Ymax = lineList[i].Y;
            }
            //setting the axis
            XAxis.SetPosition(0, new Vector3((float)Xmin, 0, 0));
            XAxis.SetPosition(1, new Vector3((float)Xmax, 0, 0));
            YAxis.SetPosition(0, new Vector3(0, (float)Ymin, 0));
            YAxis.SetPosition(1, new Vector3(0, (float)Ymax, 0));
            ZAxis.SetPosition(0, new Vector3(0, 0, (float)Zmin));
            ZAxis.SetPosition(1, new Vector3(0, 0, (float)Zmax));
        }
    }
}