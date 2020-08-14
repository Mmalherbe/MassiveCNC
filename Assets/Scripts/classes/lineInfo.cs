using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.classes
{
  public class lineInfo
    {

        public float? a;
        public float?b;
        public float? x;
        public float? y { 
            get {
                if (a == null || b == null || x == null) return null;
                return (a>0?a:1) * x + (b>0?b:0);
                    } }
    }
}
