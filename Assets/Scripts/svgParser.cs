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
        public void Parse(string urlToFile)
        {

            Debug.Log(urlToFile);
            if (Path.GetExtension(urlToFile).ToUpper() != ".SVG")
            {
                return;
            }
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.None;
            settings.XmlResolver = null;
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader reader = XmlReader.Create(urlToFile, settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            Debug.Log(doc);
            SVGClass svg = new SVGClass();
            foreach (XmlNode ele in doc)
            {
                if (ele.Name.ToUpper() == "SVG")
                {
                    foreach (XmlNode node in ele)
                    {
                        if(node.Name.ToUpper() == "DEFS")
                        {
                            foreach (XmlNode nodeinnode in node)
                            {
                                Debug.Log(nodeinnode);
                                if (svg.paths.Where(x => x.pathClass == nodeinnode.Name) == null)
                                {
                                    Debug.Log("poep");
                                }
                            }
                        }
                        Debug.Log(node);
                    }

                }
            }



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
