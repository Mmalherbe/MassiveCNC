using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.classes
{
    public class AISvg
    {
        public string id;
        public string dataname;
        public string xmlns;
        public Dictionary<string, string> defs;
        public List<svgPath> paths;
    }
    public class svgPath
    {
        public string d;
        public string Class;
        public List<PathCommand> commands;
    }

    public class PathCommand
    {
        public string commandType;
        public float X;
        public float Y;

    }

}
