using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

public class svgToPath : MonoBehaviour
{

    public void Parse(string urlToPath)
    {
        if(!File.Exists(urlToPath) || Path.GetExtension(urlToPath).ToUpper() != "SVG")
        {
            return;

        }



    }

}
