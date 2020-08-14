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

public class FileController : MonoBehaviour
{
    // calling upon different classes, objects and variables
    [SerializeField] private gcParser gcParser;
    [SerializeField] private TextMeshProUGUI CNCbtn_text;
    [SerializeField] private TextMeshProUGUI SVGbtn_text;
    [SerializeField] private SVGToPath svgToPath;
    public void Start()
    {
        /*  string fileName = "spindleTest.cnc";
          string path = Path.Combine(Application.dataPath, "examples", fileName);

          using (System.IO.StreamWriter file =
                 new System.IO.StreamWriter(path))
          {
              file.WriteLine(";" + DateTime.Now);
              for (int i = 0; i < 1023; i++)
              {
                  file.WriteLine("M3 S"+i);

              }
          }*/

    }
    public void openCNCfile()
    {


        CNCbtn_text.text = "opening File";
        string path = EditorUtility.OpenFilePanel("Open GCode", "", "cnc,nc");
        //string path = Application.dataPath + @"/examples/example.nc";
        // opens a filebrowser. The chosen files path will be stored as String
        using (StreamReader sr = new StreamReader(path))
        {
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                if (!line.StartsWith(";") && !string.IsNullOrEmpty(line))
                {
                    gcParser.fileLinebyLine.Add(line.Trim());
                }


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
        if (path.ToUpper().Contains("LINE") || path.ToLower().Contains("line")  )
        {
            svgToPath.ParseSVGLinesToPath(path);
        }
        else
        {


            svgToPath.ParseSVGToPath(path);
        }
        SVGbtn_text.text = "File Opened"; // Changes the buttontext again                      
        return;
    }
    public string writeFile(List<gcLine> toWrite, string fileName)
    {
        string path = "";
        if (string.IsNullOrEmpty(fileName))
        {
            path = EditorUtility.SaveFilePanel(
           "Save Paths as Gcode",
           "",
           "exportedPaths" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".cnc",
           "cnc");

        }
        else
        {
            if (Path.GetExtension(fileName) == "")
            {
                fileName += ".cnc";
            }
            path = Path.Combine(Application.dataPath, "examples", fileName);
        }
        if (string.IsNullOrEmpty(path)) return "";
        using (System.IO.StreamWriter file =
        new System.IO.StreamWriter(path))
        {
            file.WriteLine(";" + DateTime.Now);
            float lastknownFeed = 0;
            bool lastknownAux1 = false;
            for (int i = 0; i < toWrite.Count; i++)
            {
                if (i == 0 || i == toWrite.Count - 1)
                {
                    file.WriteLine(toWrite[i].ToEdingString(false, 0f));
                }
                else
                {
                    float? feed = null;
                    bool? aux1 = null;
                    float? volt = null;
                    if (toWrite[i].F != null)
                    {
                        lastknownFeed = (float)toWrite[i].F;
                    }
                    if (toWrite[i].AUX1 != null)
                    {
                        lastknownAux1 = (bool)toWrite[i].AUX1;
                    }
                    if (toWrite[i].F != toWrite[i - 1].F)
                    {
                        if (toWrite[i].F == null)
                        {
                            feed = lastknownFeed;
                        }
                        else
                        {
                            feed = (float)toWrite[i].F;
                        }
                    }
                    if (toWrite[i].AUX1 != toWrite[i - 1].AUX1)
                    {
                        if (toWrite[i].AUX1 == null)
                        {
                            aux1 = lastknownAux1;
                        }
                        else
                        {
                            aux1 = (bool)toWrite[i].AUX1;
                        }
                    }
                    if (toWrite[i].volt != toWrite[i - 1].volt)
                    {
                        volt = (float)toWrite[i].volt;
                    }
                    file.WriteLine(toWrite[i].ToEdingString(_AUX1: aux1, _Volt: volt, _Feed: feed));
                }
            }
            /*foreach (gcLine line in toWrite)
            {
                file.WriteLine(line.ToEdingString());
            }*/
        }


        return path;
    }
}
