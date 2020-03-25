using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace svg_parser
{
    class Program
    {
        static string svgpath;
        static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("No file path given.");
                
            }
            string path = @"C:\Users\Gebruiker\Desktop\massivecnc\Assets\svg-example.svg";

                svgparser.parseSVG(path);
                svgparser.mergeConnectedLines();


                svgparser.optimizePolys();
                foreach (svgparser.typLine pD in svgparser.pData)
                {
                    Console.Write(pD.PathCode);
                }
                Console.ReadKey();
           
        }
       
    }
}
