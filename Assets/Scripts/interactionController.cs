using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class interactionController : MonoBehaviour
{


    [SerializeField] gcParser gcParser;
    [SerializeField] FileController fileController;
    [SerializeField] TextToPath TextToPath;

    public void ParseTextToGcode_Click()
    {
        TextToPath.ClickParseTextToGcode();
    }
    public void ShowPathFromGcodeFile_Click()
    {
        fileController.openfile();
    }


}
