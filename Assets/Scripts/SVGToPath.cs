using Assets.Scripts.classes;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SVGToPath : MonoBehaviour
{

    internal void ParseSVGToPath(string urlToFile)
    {
      if(Path.GetExtension(urlToFile).ToUpper() != "SVG")
        {
            return;
        }
    
    }
    

}
