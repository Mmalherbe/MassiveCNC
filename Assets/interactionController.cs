using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class interactionController : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI inputText;
    [SerializeField] gcParser gcParser;
    [SerializeField] FileController fileController;

    public void ParseTextToGcode_Click()
    {
        gcParser.ParseTextToGcode(inputText.text);
    }
    public void ShowPathFromGcodeFile_Click()
    {
        fileController.openfile();
    }


}
