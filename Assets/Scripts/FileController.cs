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
    public void Start()
    {
        string fileName = "spindleTest.cnc";
        string path = Path.Combine(Application.dataPath, "examples", fileName);

        using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(path))
        {
            file.WriteLine(";" + DateTime.Now);
            for (int i = 0; i < 1023; i++)
            {
                file.WriteLine("M3 S"+i);

            }
        }

    }
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
        if (path.ToUpper().Contains("LINE")) {
            svgToPath.ParseSVGLinesToPath(path);
        } else
        {

        
            svgToPath.ParseSVGToPath(path);
        }
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
            file.WriteLine(toWrite[0].ToEdingString(toWrite[0].AUX1,toWrite[0].volt));
            for(int i =1; i < toWrite.Count; i+=2)
            {
                if(toWrite[i].AUX1 != toWrite[i - 1].AUX1 && toWrite[i].volt != toWrite[i - 1].volt) // if the aux1 input changed compared to the line before
                {
                    file.WriteLine(toWrite[i].ToEdingString(_AUX1:toWrite[i].AUX1,_volt:toWrite[i].volt));
                }else if(toWrite[i].AUX1 != toWrite[i - 1].AUX1)
                {
                    file.WriteLine(toWrite[i].ToEdingString(_AUX1:toWrite[i].AUX1));
                }
                else if(toWrite[i].volt != toWrite[i - 1].volt){
                    file.WriteLine(toWrite[i].ToEdingString(_volt:toWrite[i].volt));
                }
                else
                {
                    file.WriteLine(toWrite[i].ToEdingString());
                }
            }

          /*  foreach (gcLine line in toWrite)
            {
                file.WriteLine(line.ToEdingString());
            }
            */
        }



    }
}