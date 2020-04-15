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

    public void ParseTextToGcode(string _text)
    {

        List<string> TextLines = _text.Split(char.Parse("\n")).ToList();
        Dictionary<int, List<Coords>> DictLineCoords = new Dictionary<int, List<Coords>>();

        for (int i = 0; i < TextLines.Count; i++)
        {
            if(TextLines[i].Trim().Length < 1) { return; }
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


            DictLineCoords.Add(i, coords);
            }
        }



        

        Debug.Log("displaying path for : " + DictLineCoords[0]);
        Linebuilder.showOutLinesFromPoints(DictLineCoords[0]);
        Gcparser.GenerateGcodeFromPath(DictLineCoords[0]);
    }

}
