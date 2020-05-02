/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0

 Online created by  Xml2CSharp
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Assets.Scripts.classes
{
	public class SVGFontClass
	{
		[XmlRoot(ElementName = "defs", Namespace = "http://www.w3.org/2000/svg")]
		public class Defs
		{
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
		}

		[XmlRoot(ElementName = "namedview", Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd")]
		public class Namedview
		{
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "pagecolor")]
			public string Pagecolor { get; set; }
			[XmlAttribute(AttributeName = "bordercolor")]
			public string Bordercolor { get; set; }
			[XmlAttribute(AttributeName = "borderopacity")]
			public string Borderopacity { get; set; }
			[XmlAttribute(AttributeName = "pageopacity", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Pageopacity { get; set; }
			[XmlAttribute(AttributeName = "pageshadow", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Pageshadow { get; set; }
			[XmlAttribute(AttributeName = "zoom", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Zoom { get; set; }
			[XmlAttribute(AttributeName = "cx", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Cx { get; set; }
			[XmlAttribute(AttributeName = "cy", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Cy { get; set; }
			[XmlAttribute(AttributeName = "document-units", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Documentunits { get; set; }
			[XmlAttribute(AttributeName = "current-layer", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Currentlayer { get; set; }
			[XmlAttribute(AttributeName = "showgrid")]
			public string Showgrid { get; set; }
			[XmlAttribute(AttributeName = "window-width", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Windowwidth { get; set; }
			[XmlAttribute(AttributeName = "window-height", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Windowheight { get; set; }
			[XmlAttribute(AttributeName = "window-x", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Windowx { get; set; }
			[XmlAttribute(AttributeName = "window-y", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Windowy { get; set; }
			[XmlAttribute(AttributeName = "window-maximized", Namespace = "http://www.inkscape.org/namespaces/inkscape")]
			public string Windowmaximized { get; set; }
		}

		[XmlRoot(ElementName = "type", Namespace = "http://purl.org/dc/elements/1.1/")]
		public class Type
		{
			[XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
			public string Resource { get; set; }
		}

		[XmlRoot(ElementName = "Work", Namespace = "http://creativecommons.org/ns#")]
		public class Work
		{
			[XmlElement(ElementName = "format", Namespace = "http://purl.org/dc/elements/1.1/")]
			public string Format { get; set; }
			[XmlElement(ElementName = "type", Namespace = "http://purl.org/dc/elements/1.1/")]
			public Type Type { get; set; }
			[XmlElement(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
			public string Title { get; set; }
			[XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
			public string About { get; set; }
		}

		[XmlRoot(ElementName = "RDF", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
		public class RDF
		{
			[XmlElement(ElementName = "Work", Namespace = "http://creativecommons.org/ns#")]
			public Work Work { get; set; }
		}

		[XmlRoot(ElementName = "metadata", Namespace = "http://www.w3.org/2000/svg")]
		public class Metadata
		{
			[XmlElement(ElementName = "RDF", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
			public RDF RDF { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
		}

		[XmlRoot(ElementName = "path", Namespace = "http://www.w3.org/2000/svg")]
		public class Path
		{
			[XmlAttribute(AttributeName = "d")]
			public string D { get; set; }
			[XmlAttribute(AttributeName = "style")]
			public string Style { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
		}

		[XmlRoot(ElementName = "g", Namespace = "http://www.w3.org/2000/svg")]
		public class G
		{
			[XmlElement(ElementName = "path", Namespace = "http://www.w3.org/2000/svg")]
			public Path Path { get; set; }
			[XmlAttribute(AttributeName = "aria-label")]
			public string Arialabel { get; set; }
			[XmlAttribute(AttributeName = "transform")]
			public string Transform { get; set; }
			[XmlAttribute(AttributeName = "style")]
			public string Style { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
		}

		[XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
		public class Svg
		{
			[XmlElement(ElementName = "defs", Namespace = "http://www.w3.org/2000/svg")]
			public Defs Defs { get; set; }
			[XmlElement(ElementName = "namedview", Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd")]
			public Namedview Namedview { get; set; }
			[XmlElement(ElementName = "metadata", Namespace = "http://www.w3.org/2000/svg")]
			public Metadata Metadata { get; set; }
			[XmlElement(ElementName = "g", Namespace = "http://www.w3.org/2000/svg")]
			public G G { get; set; }
			[XmlAttribute(AttributeName = "dc", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Dc { get; set; }
			[XmlAttribute(AttributeName = "cc", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Cc { get; set; }
			[XmlAttribute(AttributeName = "rdf", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Rdf { get; set; }
			[XmlAttribute(AttributeName = "svg", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string _svg { get; set; }
			[XmlAttribute(AttributeName = "xmlns")]
			public string Xmlns { get; set; }
			[XmlAttribute(AttributeName = "sodipodi", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Sodipodi { get; set; }
			[XmlAttribute(AttributeName = "inkscape", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Inkscape { get; set; }
			[XmlAttribute(AttributeName = "width")]
			public string Width { get; set; }
			[XmlAttribute(AttributeName = "height")]
			public string Height { get; set; }
			[XmlAttribute(AttributeName = "viewBox")]
			public string ViewBox { get; set; }
			[XmlElement(ElementName = "version")]
			public List<string> Version { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "docname", Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd")]
			public string Docname { get; set; }
		}

	}
}