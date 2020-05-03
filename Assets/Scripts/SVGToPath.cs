using Assets.Scripts;
using Assets.Scripts.classes;
using Assets.Scripts.classes.Font;
using SVGMeshUnity;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SVGToPath : MonoBehaviour
{
    [SerializeField] private SVGMesh Mesh;
    [SerializeField] private gcParser gcParser;

    private SVGData SVG;
    [SerializeField] private List<string> svgClassesToShow = new List<string>();
    [SerializeField] internal Dictionary<string, List<Coords>> svgPaths = new Dictionary<string, List<Coords>>();
    [SerializeField] private GameObject SVGClassHolder;
    [SerializeField] private GameObject SVGClassPrefab;
    private List<Coords> coordsToParse = new List<Coords>();
    [SerializeField] internal Dictionary<string, Dictionary<string, List<Coords>>> svgFont = new Dictionary<string, Dictionary<string, List<Coords>>>();
    internal void ParseSVGLinesToPath(string urlToFile)
	{
        svgParser parser = new svgParser();
        Assets.Scripts.classes.SvgLineFile.Svg svg = parser.ParseSVGLine(urlToFile);
        List<Coords> coordsForId = new List<Coords>();
        Coords coord = new Coords();
        foreach (Assets.Scripts.classes.SvgLineFile.G letter in svg.G)
        {


            foreach (Assets.Scripts.classes.SvgLineFile.Line line in letter.Line)
            {
                coord = new Coords();
                coord.X = float.Parse(line.X1 == null ? "0" : line.X1);
                coord.Y = float.Parse(line.Y1 == null ? "0" : line.Y1);
                coordsForId.Add(coord);
                coord = new Coords();
                coord.X = float.Parse(line.X2 == null ? "0" : line.X2);
                coord.Y = float.Parse(line.Y2 == null ? "0" : line.Y2);
                coordsForId.Add(coord);
            }
            CreateSVGClassObject(letter.Id);
            svgPaths.Add(letter.Id, coordsForId);
            svgClassesToShow.Add(letter.Id);
        }
    }
        internal void ParseSVGToPath(string urlToFile)
        {
        svgParser parser = new svgParser();
        SvgClass svg = parser.Parse(urlToFile);
        //string SVG_PATH = doc.
        



        foreach (SvgPath svgPath in svg.SvgPath.Where(x=>x.D.Length> 10))
        {

            SVG = new SVGData();
            SVG.Path(svgPath.D);

            Debug.Log(SVG.Dump());
            Mesh.Fill(SVG);
            List<Coords> coordsForId = new List<Coords>();
            float minX = Mesh.MeshData.Vertices.Min(x => x.x);
            float minY = Mesh.MeshData.Vertices.Min(y => y.y);
            float maxX = Mesh.MeshData.Vertices.Max(x => x.x);
            float maxY = Mesh.MeshData.Vertices.Max(y => y.y);
            float midX = minX + ((maxX - minX) / 2);
            float midY = minY + ((maxY - minY) / 2);
            midX = 0;
            midY = 0;
            for(int i =0; i < Mesh.MeshData.Vertices.Count;i++)
            {
                Coords coord = new Coords { X = Mesh.MeshData.Vertices[i].x - midX, Y = Mesh.MeshData.Vertices[i].y - midY, Z = Mesh.MeshData.Vertices[i].z };
                coordsForId.Add(coord);
            }


            if(svgPath.Class.Length == 0)
            {
                svgPath.Class = svgPath.Id;
            }
            CreateSVGClassObject(svgPath.Class);
            svgPaths.Add(svgPath.Class, coordsForId);
            svgClassesToShow.Add(svgPath.Class);
        }
        


    }


    private void ShowPathsFromSVG()
    {
        coordsToParse.Clear();
        List<Coords> toShow = new List<Coords>();

        


        foreach(KeyValuePair<string,List<Coords>> kvp in svgPaths)
        {
            if (svgClassesToShow.Contains(kvp.Key))
            {// class name is found in the list of classes to show
                
                foreach(Coords coord in kvp.Value)
                {
                    toShow.Add(coord);
                }
            }
        }
        coordsToParse = toShow;
        gcParser.SetCoordsAndMultiLine(coordsToParse);
    }

    internal void PathToGCode()
    {
        ShowPathsFromSVG();
        gcParser.GenerateGcodeFromPath();

    }

    private void CreateSVGClassObject(string className)
    {
        GameObject svgClassObject = (GameObject)Instantiate(SVGClassPrefab, SVGClassHolder.transform);
        svgClassObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
            ToggleToggled(svgClassObject.GetComponent<Toggle>());
        });
        for (int i =0; i < svgClassObject.transform.childCount; i++)
        {
            if(svgClassObject.transform.GetChild(i).name == "Label_ClassName")
            {
                svgClassObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = className;
            }
        }

        svgClassObject.name = className;

    }
    internal void ToggleToggled(Toggle _toggle)
    {
        if (svgClassesToShow.Contains(_toggle.name))
        {
            svgClassesToShow.Remove(_toggle.name);
        }
        else
        {
            svgClassesToShow.Add(_toggle.name);
        }

        ShowPathsFromSVG();
    }
}
