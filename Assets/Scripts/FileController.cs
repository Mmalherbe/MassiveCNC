// These are the libraries used in this code
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using Assets.Scripts.classes;
using System;
using Path = System.IO.Path;
using TMPro;

public class FileController : MonoBehaviour {
    // calling upon different classes, objects and variables
    [SerializeField] private  gcParser gcParser;
    [SerializeField] private TextMeshProUGUI CNCbtn_text;
    [SerializeField] private TextMeshProUGUI SVGbtn_text;
    [SerializeField] private SVGToPath svgToPath;

    public void openCNCfile(){


        CNCbtn_text.text = "opening File";
        string path = EditorUtility.OpenFilePanel("Open GCode", "", "cnc,nc");
        //string path = Application.dataPath + @"/examples/example.nc";
        // opens a filebrowser. The chosen files path will be stored as String
        using (StreamReader sr = new StreamReader(path))
        {
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                if (line.StartsWith(";") || string.IsNullOrEmpty(line))
                {
                    break;
                }
                    gcParser.fileLinebyLine.Add(line.Trim());

            }
        }
       
        gcParser.ParseFromGcodeFile();
        CNCbtn_text.text = "File Opened"; // Changes the buttontext again                      
return;           
}
    public void openSVGfile()
    {
        SVGbtn_text.text = "opening File";
        string path = EditorUtility.OpenFilePanel("Open SVG", "", "svg");
        svgToPath.ParseSVGToPath(path);
        SVGbtn_text.text = "File Opened"; // Changes the buttontext again                      
        return;
    }
    public  void writeFile(List<gcLine> toWrite,string fileName)
    {
        if(Path.GetExtension(fileName) == "")
        {
            fileName += ".cnc";
        } 
        string path =  Path.Combine(Application.dataPath,"examples", fileName);
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(path))
        {
            file.WriteLine(";"+DateTime.Now);
            foreach (gcLine line in toWrite)
            {
                file.WriteLine(line.ToEdingString());
            }
        }



    }
}