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
    private char[] trims = { char.Parse("\n"),char.Parse("\r") };
    public void ParseTextToGcode(string _text)
    {

        List<string> TextLines = _text.Split(char.Parse("\n")).ToList();
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
                PointF[] pt = path.PathPoints;
                float minX = pt.Min(x => x.X);
                float minY = pt.Min(y => y.Y);
                float maxX = pt.Max(x => x.X);
                float maxY = pt.Max(y => y.Y);
                float midX = minX + ((maxX - minX) / 2);
                float midY = minY + ((maxY - minY) / 2);

                Debug.Log("Min X : " + minX + " ,Max X : " + maxX + " , Mid X : " + midX);
                Debug.Log("Min Y : " + minY + " ,Max Y : " + maxY + " , Mid Y : " + midY);
                foreach (PointF p in pt)
                {
                    coords.Add(new Coords() { X = p.X - (midX), Y = p.Y - (midY), Z = 0 });
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

         for(int i =1; i< ListPaths.Count; i++)
        {
            foreach(Coords coord in ListPaths[i].coordList)
            {
                coord.Y = coord.Y + ListPaths[i - 1].minY + Yaap;
            }
            ListPaths[i].minY = ListPaths[i].coordList.Min(x => x.Y);
            ListPaths[i-1].minY = ListPaths[i-1].coordList.Min(x => x.Y);
        }


        for (int i = 0; i < ListPaths.Count; i++)
        {
            Linebuilder.showOutLinesFromPoints(ListPaths[i].coordList);
            Gcparser.GenerateGcodeFromPath(ListPaths[i].coordList);

        }
    }

}
