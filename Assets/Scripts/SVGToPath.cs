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

    public void Start()
    {
        ParseSVGToPath(Application.dataPath + "/SVGTJE.svg");
    }

    internal void ParseSVGToPath(string urlToFile)
    {
        svgParser parser = new svgParser();
        parser.Parse(urlToFile);

    /* string SVG_PATH = doc.
        SVG = new SVGData();
        SVG.Path(SVG_PATH);
        Debug.Log(SVG.Dump());
   */
    }
    

}
