// These are the libraries used in this code
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using Assets.Scripts.classes;
using System;
using Path = System.IO.Path;

public class FileController : MonoBehaviour {
    // calling upon different classes, objects and variables
    public gcParser gcParser;
    public Text btn_text;
    string ReadText ;

    public void openfile(){
        btn_text.text = "opening File";
        string path = EditorUtility.OpenFilePanel("Open GCode", "", "cnc");
        //string path = Application.dataPath + @"/examples/example.nc";
        // opens a filebrowser. The chosen files path will be stored as String
        using (StreamReader sr = new StreamReader(path))
        {
            while (sr.Peek() >= 0)
            {
                gcParser.fileLinebyLine.Add(sr.ReadLine());
            }
        }
       
        ReadText = File.ReadAllText (path); // Reads out the file on the specific path
        gcParser.GCode = ReadText; // Calls for two function of the gcParser Class
        gcParser.ParseFromGcodeFile();
        btn_text.text = "File Opened"; // Changes the buttontext again                      
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