using Assets.Scripts.classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts
{
    public class svgParser
    {
        public SvgClass Parse(string urlToFile)
        {

            Debug.Log(urlToFile);
            if (System.IO.Path.GetExtension(urlToFile).ToUpper() != ".SVG")
            {
                return null;
            }
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.None;
            settings.XmlResolver = null;
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader reader = XmlReader.Create(urlToFile, settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            Debug.Log(doc);
            SvgClass svg = new SvgClass();

            svg = XmlOperation.Deserialize<SvgClass>(urlToFile);
            return svg;
        }


    }
}
/*
 * public class SVGClass
    {
        public string id;
        public string viewBox;
        public List<SVGPath> path;

    }

    public class SVGPath
    {
        public string pathClass;
        public string d;
        public string transform;
    }
 */
