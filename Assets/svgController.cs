using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class svgController : MonoBehaviour
{
    [SerializeField] private bool fileLoaded;

    public bool loadFile(string fileName)
    {
        string ext = Path.GetExtension(fileName).ToLower();
        if (ext == ".svg")
        { startConvertSVG(fileName); fileLoaded = true; }

    }
    private void startConvertSVG(string source)
    {
        cncLogger.RealTimeLog("startConvertSVG");
        string gcode = GCodeFromSVG.convertFromFile(source);        // get code
        SaveRecentFile(source);
        foldCode();
    }
}
