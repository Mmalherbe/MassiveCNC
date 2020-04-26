using System;
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
