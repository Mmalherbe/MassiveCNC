using Assets.Scripts;
using Assets.Scripts.classes;
using SVGMeshUnity;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class SVGToPath : MonoBehaviour
{
    [SerializeField] private SVGMesh Mesh;

    private SVGData SVG;
    [SerializeField] private List<SvgClass> svgClasses = new List<SvgClass>();
    [SerializeField] private Dictionary<string, List<Coords>> svgPaths = new Dictionary<string, List<Coords>>();
    public void Start()
    {
        ParseSVGToPath(Application.dataPath + "/SVGTJE.svg");
    }

    internal void ParseSVGToPath(string urlToFile)
    {
        svgParser parser = new svgParser();
        SvgClass svg = parser.Parse(urlToFile);
        //string SVG_PATH = doc.
        
        foreach (SvgPath svgPath in svg.SvgPath)
        {

            SVG = new SVGData();
            SVG.Path(svgPath.D);

            Debug.Log(SVG.Dump());
            Mesh.Fill(SVG);
            List<Coords> coordsForId = new List<Coords>();
           foreach(Vector3 coords in Mesh.MeshData.Vertices)
            {
                Coords coord = new Coords { X = coords.x, Y = coords.y, Z = coords.z };
                coordsForId.Add(coord);
            
            }
            svgPaths.Add(svgPath.Class, coordsForId);
            Debug.Log(Mesh.MeshData.Vertices);
        }

        Debug.Log(svgPaths);

    }
    

}
