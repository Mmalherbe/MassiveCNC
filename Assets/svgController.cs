using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class svgController : MonoBehaviour
{
    [SerializeField] private bool fileLoaded;
    private void Start()
    {
        string exampleFilePath = Path.Combine(Application.dataPath, @"examples\graphic_bunny_string_art.svg");
        loadFile(exampleFilePath);
    }
    public bool loadFile(string fileName)
    {
        string ext = Path.GetExtension(fileName).ToLower();
        if (ext == ".svg")
        { startConvertSVG(fileName); fileLoaded = true; }
        return true;
    }
    private void startConvertSVG(string source)
    {
        cncLogger.RealTimeLog("startConvertSVG");
        string gcode = GCodeFromSVG.convertFromFile(source);      
        Debug.Log(gcode);
    }
}
