// These are the libraries used in this code
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
public class BrowseNCFile : MonoBehaviour {
    // calling upon different classes, objects and variables
    public gcParser gcParser;
    public Text btn_text;
    string ReadText ;
    public void Clicked () {
        //This code will change the text of the button and call for the openfile function
        btn_text.text = "opening File";
        openfile ();
        return;
    }
    void openfile(){
        string path = EditorUtility.OpenFilePanel("Open GCode", "", "nc");
        // opens a filebrowser. The chosen files path will be stored as String
        ReadText = File.ReadAllText (path); // Reads out the file on the specific path
        gcParser.GCode = ReadText; // Calls for two function of the gcParser Class
        gcParser.Parse();
btn_text.text = "File Opened"; // Changes the buttontext again                      
return;           
}
}