using Assets.Scripts.classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using UnityEngine;

public class TextToPath : MonoBehaviour
{
    [SerializeField] private gcLineBuilder Linebuilder;
    [SerializeField] private gcParser Gcparser;
    [SerializeField] private int fontSize = 50;
    [SerializeField] private int fontStyle = 1;
    [SerializeField] private CNC_Settings Cnc_Settings;
    private char[] trims = { char.Parse("\n"), char.Parse("\r") };
    public void ParseTextToGcode(string _text)
    {

        List<string> _TextLines = _text.Split(char.Parse("\n")).ToList();
        List<string> TextLines = new List<string>();
        foreach (string txt in _TextLines)
        {
            if (txt.Length > 0)
            {
                TextLines.Add(txt);
            }
        }

        List<TextLinePath> ListPaths = new List<TextLinePath>();

        for (int i = 0; i < TextLines.Count; i++)
        {
            bool empty = TextLines[i] == string.Empty;
            if (empty) { return; }
            List<Coords> coords = new List<Coords>();
            TextLinePath txtLinPath = new TextLinePath();
            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddString(TextLines[i].Trim(char.Parse("\n")), new FontFamily("arial"),
                  fontStyle, fontSize, new Point(Mathf.RoundToInt(0), Mathf.RoundToInt(0)),
                  StringFormat.GenericTypographic);
                path.CloseFigure();
                PointF[] pt = path.PathPoints;
                float minX = pt.Min(x => x.X);
                float minY = pt.Min(y => y.Y);
                float maxX = pt.Max(x => x.X);
                float maxY = pt.Max(y => y.Y);
                float midX = minX + ((maxX - minX) / 2);
                float midY = minY + ((maxY - minY) / 2);


                foreach (PointF p in pt)
                {
                    coords.Add(new Coords() { X = p.X - (midX), Y = p.Y - (midY), Z = 0.00001f });
                }
                txtLinPath.id = i;
                txtLinPath.coordList = coords;
                txtLinPath.minX = minX;
                txtLinPath.maxX = maxX;
                txtLinPath.minY = minY;
                txtLinPath.maxY = maxY;


                ListPaths.Add(txtLinPath);
            }
        }

        float allPathsMinX = ListPaths.Min(x => x.minX);
        float allPathsMaxX = ListPaths.Max(x => x.maxX);
        float allPathsMinY = ListPaths.Min(x => x.minY);
        float allPathsMaxY = ListPaths.Max(x => x.maxY);
        float Yaap = 10f;


        float allSize = 0f;
        for (int i = 1; i < ListPaths.Count; i++)
        {
            allSize += (ListPaths[i - 1].maxY - ListPaths[i - 1].minY);
            for (int j = 0; j < ListPaths[i].coordList.Count; j++)
            {
                ListPaths[i].coordList[j].Y = ListPaths[i].coordList[j].Y + allSize + Yaap;

            }
            ListPaths[i].maxY = ListPaths[i].coordList.Max(x => x.Y);
            ListPaths[i].minY = ListPaths[i].coordList.Min(x => x.Y);
            //ListPaths[i-1].minY = ListPaths[i-1].coordList.Min(x => x.Y);
        }


        for (int i = 0; i < ListPaths.Count; i++)
        {
           
            Gcparser.GenerateGcodeFromPath(ListPaths[i].coordList,ListPaths.Count>1);

        }
    }


}
