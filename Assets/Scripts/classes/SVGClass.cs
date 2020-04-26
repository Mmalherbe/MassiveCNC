/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Assets.Scripts.classes
{
	[XmlRoot(ElementName = "defs", Namespace = "http://www.w3.org/2000/svg")]
	public class Defs
	{
		[XmlElement(ElementName = "style", Namespace = "http://www.w3.org/2000/svg")]
		public string Style { get; set; }
	}

	[XmlRoot(ElementName = "path", Namespace = "http://www.w3.org/2000/svg")]
	public class SvgPath
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
		[XmlAttribute(AttributeName = "d")]
		public string D { get; set; }
		[XmlAttribute(AttributeName = "transform")]
		public string Transform { get; set; }
	}

	[XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
	public class SvgClass
	{
		[XmlElement(ElementName = "defs", Namespace = "http://www.w3.org/2000/svg")]
		public Defs Defs { get; set; }
		[XmlElement(ElementName = "path", Namespace = "http://www.w3.org/2000/svg")]
		public List<SvgPath> SvgPath { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "data-name")]
		public string Dataname { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		[XmlAttribute(AttributeName = "viewBox")]
		public string ViewBox { get; set; }
	}

}




/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.classes
{
  public class SVGClass
    {
        public string id;
        public string viewBox;
        public List<SVGPath> paths;
        public Dictionary<string, SVGStyle> style;
    }

    public class SVGPath
    {
        public string pathClass;
        public string d;
        public string transform;
    }

    public class SVGStyle
    {
        public string fill;
    }
}
*/
