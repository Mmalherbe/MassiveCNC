using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using Assets.Scripts.classes;
public class svgToPath : MonoBehaviour
{

    public void Parse(string urlToPath)
    {
        if(!File.Exists(urlToPath) || System.IO.Path.GetExtension(urlToPath).ToUpper() != "SVG")
        {
            return;

        }
        SvgClass svg = new SvgClass();

       svg = XmlOperation.Deserialize<SvgClass>(urlToPath);



    }

}
