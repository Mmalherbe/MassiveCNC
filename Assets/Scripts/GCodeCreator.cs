using System.Collections;
using System.Collections.Generic;
using GCodeNet;
using UnityEngine;

public class GCodeCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static void CreateGCodeCommand()
    {
        //Create a G1 command (Rapid Linear Movement)
        var cmd = new Command(CommandType.G, 1);
        cmd.SetParameterValue(ParameterType.X, 10);
        cmd.SetParameterValue(ParameterType.Y, 20);

        //Convert to GCode
        Debug.Log(cmd.ToGCode()); //Output: "G1 X10 Y20"

        //Convert to GCode with the CRC
        Debug.Log(cmd.ToGCode(addCrc: true)); //Output: "G1 X10 Y20*116"

        //Convert to GCode with the CRC and a line number
        Debug.Log(cmd.ToGCode(addCrc: true, lineNumber: 4)); //Output: "N4 G1 X10 Y20*46"
    }


}
