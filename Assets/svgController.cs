using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class svgController : MonoBehaviour
{
    // Start is called before the first frame update

        [SerializeField] string svgPath;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ParseExample()
    {
        if (readExampleFile())
        {
            svgParser.parseSVG(svgPath);
            svgParser.mergeConnectedLines();


            svgParser.optimizePolys();
            Debug.Log(svgParser.pData);
        }

    }

    bool readExampleFile()
    {
        using (StreamReader sr = new StreamReader("Assets/svg-example.svg")) // read file , 
        {
            // Read the stream to a string, and write the string to the console.
            svgPath = sr.ReadToEnd();
        }
        return true;
    }
}
