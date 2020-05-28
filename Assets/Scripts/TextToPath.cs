using Assets.Scripts.classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextToPath : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputTextForTextToGCode;
    [SerializeField] private gcLineBuilder Linebuilder;
    [SerializeField] private gcParser Gcparser;
    [SerializeField] private SVGToPath svgParser;
    [SerializeField] internal int fontSize = 10;
    [SerializeField] internal float XoffSetCharacters = 0;
    [SerializeField] internal float YoffSetLines = 10;
    [SerializeField] internal bool useMidPointOfEachCharacter;
    private int fontStyle = 0;
    [SerializeField] private CNC_Settings Cnc_Settings;
    internal string fontstyleString = "Regular";
    internal string fontFamilyString = "Arial";
    private char[] trims = { char.Parse("\n"), char.Parse("\r") };
    internal bool svgFont;

    public void ClickParseTextToGcode()
    {
        ParseTextToGcode(inputTextForTextToGCode.text);
    }
    public void ParseTextToGcode(string _text)
    {
        List<string> _TextLines = _text.Split(char.Parse("\n")).ToList();
        List<string> TextLines = _TextLines.ToList<string>();
        List<TextLinePath> ListPaths = new List<TextLinePath>();
        List<TextLinePath> ListPathsOneLine = new List<TextLinePath>();
        List<Coords> svgLines = new List<Coords>();
        List<List<Coords>> LineOfCharacterCoords = new List<List<Coords>>();
        List<Coords> CoordsOneCharacter = new List<Coords>();
        float allSizeX = 0f;
        if (svgFont)
        {
            LineOfCharacterCoords.Clear();
            Dictionary<string, List<Coords>> fontDict = new Dictionary<string, List<Coords>>(svgParser.svgFont.First(x => x.Key == fontFamilyString).Value);
            for (int i = 0; i < TextLines.Count; i++)// lines of characters
            {
                TextLinePath txtLinPath = new TextLinePath();
                for (int j = 0; j < TextLines[i].Length; j++) // character
                {
                    List<Coords> coords = new List<Coords>();
                    CoordsOneCharacter.Clear();
                    CoordsOneCharacter = new List<Coords>(fontDict.First(x => x.Key == TextLines[i][j].ToString()).Value); // list of coords for character

                    float minX = CoordsOneCharacter.Min(x => x.X);
                    float minY = CoordsOneCharacter.Min(y => y.Y);
                    float maxX = CoordsOneCharacter.Max(x => x.X);
                    float maxY = CoordsOneCharacter.Max(y => y.Y);
                    float midX = ((maxX - minX) / 2);
                    float midY = ((maxY - minY) / 2);

                    if (!useMidPointOfEachCharacter)
                    {
                        foreach (Coords p in CoordsOneCharacter)
                        {
                            coords.Add(new Coords() { X = p.X, Y = p.Y, Z = 0.00001f });
                        }

                    }
                    else
                    {
                        foreach (Coords p in CoordsOneCharacter)
                        {
                            coords.Add(new Coords() { X = p.X - (midX), Y = p.Y - (midY), Z = 0.00001f });
                        }
                    }
                    LineOfCharacterCoords.Add(coords);
                    TextLinePath PathOneCharacter = new TextLinePath
                    {
                        id = j,
                        coordList = coords,
                        minX = minX,
                        maxX = maxX,
                        minY = minY,
                        maxY = maxY
                    };

                    ListPathsOneLine.Add(PathOneCharacter);
                }
                allSizeX = 0f;
                for (int l = 1; l < ListPathsOneLine.Count; l++)
                {
                    ListPathsOneLine[l - 1].maxX = ListPathsOneLine[l - 1].coordList.Max(x => x.X);
                    ListPathsOneLine[l - 1].minX = ListPathsOneLine[l - 1].coordList.Min(x => x.X);
                    allSizeX += (ListPathsOneLine[l - 1].maxY - ListPathsOneLine[l - 1].minY);

                    for (int k = 0; k < ListPathsOneLine[l].coordList.Count; k++)
                    {
                        ListPathsOneLine[l].coordList[k].X = ListPathsOneLine[l].coordList[k].X + allSizeX + XoffSetCharacters;
                    }
                    ListPathsOneLine[l].maxX = ListPathsOneLine[l].coordList.Max(x => x.X);
                    ListPathsOneLine[l].minX = ListPathsOneLine[l].coordList.Min(x => x.X);
                    //ListPaths[i-1].minY = ListPaths[i-1].coordList.Min(x => x.Y);
                }
                foreach (TextLinePath tlp in ListPathsOneLine)
                {
                    tlp.minX = tlp.coordList.Min(x => x.X);
                    tlp.maxX = tlp.coordList.Max(x => x.X);

                    tlp.minY = tlp.coordList.Min(x => x.Y);
                    tlp.maxY = tlp.coordList.Max(x => x.Y);

                    ListPaths.Add(tlp);
                }
            }
            float allPathsSVGMinX = ListPaths.Min(x => x.minX);
            float allPathsSVGMaxX = ListPaths.Max(x => x.maxX);
            float allPathsSVGMinY = ListPaths.Min(x => x.minY);
            float allPathsSVGMaxY = ListPaths.Max(x => x.maxY);

            float allPathsSVGMidX = allPathsSVGMinX + ((allPathsSVGMaxX - allPathsSVGMinX) / 2);
            float allPathsSVGMidY = allPathsSVGMinY + ((allPathsSVGMaxY - allPathsSVGMinY) / 2);

            foreach (TextLinePath tlp in ListPaths)
            {
                foreach (Coords coord in tlp.coordList)
                {
                    coord.X -= useMidPointOfEachCharacter?allPathsSVGMidX:0;
                    coord.Y -= useMidPointOfEachCharacter?allPathsSVGMidY:0;
                }
            }
        }
        else
        {
            switch (fontstyleString)
            {
                default: fontStyle = 0; break;
                case "Bold": fontStyle = 1; break;
                case "Italic": fontStyle = 2; break;
                case "Regular": fontStyle = 0; break;
                case "Strikeout": fontStyle = 8; break;
                case "Underline": fontStyle = 4; break;


            }
            for (int i = 0; i < TextLines.Count; i++)
            {
                bool empty = TextLines[i] == string.Empty;
                if (!empty) empty = TextLines[i].Trim().Length == 0;
                if (!empty)
                {
                    List<Coords> coords = new List<Coords>();
                    TextLinePath txtLinPath = new TextLinePath();
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.StartFigure();
                        path.AddString(TextLines[i].Trim(char.Parse("\n")), new FontFamily(fontFamilyString),
                          fontStyle, fontSize, new Point(Mathf.RoundToInt(0), Mathf.RoundToInt(0)),
                          StringFormat.GenericDefault);
                        path.CloseAllFigures();
                        PointF[] pt = path.PathPoints;
                        float minX = pt.Min(x => x.X);
                        float minY = pt.Min(y => y.Y);
                        float maxX = pt.Max(x => x.X);
                        float maxY = pt.Max(y => y.Y);
                        float midX = minX + ((maxX - minX) / 2);
                        float midY = minY + ((maxY - minY) / 2);
                        foreach (PointF p in pt)
                        {
                            coords.Add(new Coords() { X = p.X - (useMidPointOfEachCharacter ? (midX):0), Y = p.Y - (useMidPointOfEachCharacter ? (midY) : 0), Z = 0.00001f });
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
            }
        }
        float allPathsMinX = ListPaths.Min(x => x.minX);
        float allPathsMaxX = ListPaths.Max(x => x.maxX);
        float allPathsMinY = ListPaths.Min(x => x.minY);
        float allPathsMaxY = ListPaths.Max(x => x.maxY);
        if (TextLines.Count > 1)
        {
            /// Setting the multiline text 
            float allSize = 0f;
            for (int i = 1; i < ListPaths.Count; i++)
            {
                allSize += (ListPaths[i - 1].maxY - ListPaths[i - 1].minY);
                for (int j = 0; j < ListPaths[i].coordList.Count; j++)
                {
                    ListPaths[i].coordList[j].Y = ListPaths[i].coordList[j].Y + allSize + YoffSetLines;

                }
                ListPaths[i].maxY = ListPaths[i].coordList.Max(x => x.Y);
                ListPaths[i].minY = ListPaths[i].coordList.Min(x => x.Y);
                //ListPaths[i-1].minY = ListPaths[i-1].coordList.Min(x => x.Y);
            }
        }
        if (ListPaths.Count > 1)
        {
            List<Coords> allCoords = new List<Coords>();
            for (int j = 0; j < ListPaths.Count; j++)
            {

                for (int k = 0; k < ListPaths[j].coordList.Count; k++)
                {
                    if (k == 0)
                    {
                        ListPaths[j].coordList[k].Travel = true;
                    }
                    allCoords.Add(ListPaths[j].coordList[k]);

                }

            }
            Gcparser.SetCoordsAndMultiLine(allCoords, true);
        }
        else
        {
            Gcparser.SetCoordsAndMultiLine(ListPaths[0].coordList);
        }
        Gcparser.GenerateGcodeFromPath();
    }


}
