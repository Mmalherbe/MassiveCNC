using Assets.Scripts.classes;
using Assets.Scripts.classes.Font;
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
            SvgClass svg = new SvgClass();

            svg = XmlOperation.Deserialize<SvgClass>(urlToFile);
            return svg;
        }

        public Assets.Scripts.classes.SvgLineFile.Svg ParseSVGLine(string urlToFile)
        {
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
            Assets.Scripts.classes.SvgLineFile.Svg svg = new Assets.Scripts.classes.SvgLineFile.Svg();

            svg = XmlOperation.Deserialize<Assets.Scripts.classes.SvgLineFile.Svg>(urlToFile);
            return svg;


        }



    }
}
