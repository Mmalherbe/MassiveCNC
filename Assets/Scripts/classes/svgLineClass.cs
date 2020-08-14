using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Assets.Scripts.classes.SvgLineFile
{
   
	[XmlRoot(ElementName = "defs", Namespace = "http://www.w3.org/2000/svg")]
	public class Defs
	{
		[XmlElement(ElementName = "style", Namespace = "http://www.w3.org/2000/svg")]
		public string Style { get; set; }
	}

	[XmlRoot(ElementName = "line", Namespace = "http://www.w3.org/2000/svg")]
	public class Line
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
		[XmlAttribute(AttributeName = "y2")]
		public string Y2 { get; set; }
		[XmlAttribute(AttributeName = "x1")]
		public string X1 { get; set; }
		[XmlAttribute(AttributeName = "y1")]
		public string Y1 { get; set; }
		[XmlAttribute(AttributeName = "x2")]
		public string X2 { get; set; }
	}

	[XmlRoot(ElementName = "g", Namespace = "http://www.w3.org/2000/svg")]
	public class G
	{
		[XmlElement(ElementName = "line", Namespace = "http://www.w3.org/2000/svg")]
		public List<Line> Line { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "data-name")]
		public string Dataname { get; set; }
	}

	[XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
	public class Svg
	{
		[XmlElement(ElementName = "defs", Namespace = "http://www.w3.org/2000/svg")]
		public Defs Defs { get; set; }
		[XmlElement(ElementName = "g", Namespace = "http://www.w3.org/2000/svg")]
		public List<G> G { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		[XmlAttribute(AttributeName = "viewBox")]
		public string ViewBox { get; set; }
	}


}
