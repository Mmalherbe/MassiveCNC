using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GCodeNet;
using System.IO;

public class CNCSimulator : MonoBehaviour
{
    
    [SerializeField] private GameObject CNCHead = null;
    [Header("Max size. Do not change unless you are 100% sure. Values in Meters.")]
    [SerializeField] private GameObject CNCSizeObject = null;
    [SerializeField]private Vector2 CNCSize;
    [Header("Speed used for movement.")]
    [SerializeField] private float speed = 1f;
    float timeOfTravel = 5; //time after object reach a target place 
    float currentTime = 0; // actual floting time 
    float normalizedValue;
    [SerializeField] private List<Vector3> parabolaPoints = new List<Vector3>();
    [SerializeField] private bool _Moving;
    [Header("!Do not adjust these positions!")]
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private Vector3 startPositionGcodeLine;
    [SerializeField] private Vector3 currentPositionOfHead;
    [SerializeField] private GameObject HomePosObj;
    private Vector3 HomePos;
    [SerializeField] private GameObject StartPosObj;
    [SerializeField] private GameObject EndPosObj;
    [SerializeField] private GameObject CentPointObj;
    [SerializeField] private List<string> gcodeLines;
    [SerializeField] private int lineCounter;
    public bool DoneWithLine = false;

    #region Testshit only



    #endregion





    // Start is called before the first frame update
    void Start()
    {
        HomePos = new Vector3(HomePosObj.transform.localPosition.x + CNCHead.transform.localScale.x/2,0, HomePosObj.transform.localPosition.z + CNCHead.transform.localScale.z / 2); 
        CNCSize = new Vector2(CNCSizeObject.transform.localScale.x, CNCSizeObject.transform.localScale.z);
        CNCHead.transform.localPosition = HomePos;
    }

    // Update is called once per frame
    void Update()
    {
        if (!DoneWithLine) return;
        if (DoneWithLine)
        {
            lineCounter++;
            CreateCommandFromGCodes(gcodeLines);
        }

    }
    public void ReadFileAndStartSimulating()
    {
        using (StreamReader sr = new StreamReader("Assets/example.cnc")) // read file , 
        {
            // Read the stream to a string, and write the string to the console.
             while(sr.Peek() >= 0)
            {
                gcodeLines.Add(sr.ReadLine());
            }
        }

        StartSimulation();

    }
    private void StartSimulation()
    {       
        lineCounter = 0;
        CreateCommandFromGCodes(gcodelines:gcodeLines);
    }


    public void CreateCommandFromGCodes(List<string> gcodelines)
    {
        DoneWithLine = false;
        var cmd = Command.Parse(gcodelines[lineCounter]);
        Debug.Log(cmd.CommandType); //Output: "G"
        Debug.Log(cmd.CommandSubType); //Output: "1"
        Debug.Log(cmd.GetParameterValue(ParameterType.X)); //Output: "10"
        Debug.Log(cmd.GetParameterValue(ParameterType.Y)); //Output: "20"
        /// 
        /// G code explained
        /// 
        /// G1 : Linear movement ( go to point x , y ) Relative!
        /// G2 : Arc movement clockwise 
        /// 


        if (cmd.CommandType == CommandType.G)  // cnc402 uses G1 G2 G3 
        {
            if (cmd.CommandSubType == 1) // Linear movement
            {
                startPositionGcodeLine = CNCHead.transform.localPosition;
                string x = cmd.GetParameterValue(ParameterType.X).ToString();
                string y = cmd.GetParameterValue(ParameterType.Y).ToString();
                if (string.IsNullOrEmpty(x)) x = "0";
                if (string.IsNullOrEmpty(y)) y = "0";
                float endX = startPositionGcodeLine.x + float.Parse(x);
                float endY = startPositionGcodeLine.y + float.Parse(y);


                endPosition = new Vector2(endX, endY);
                StartCoroutine(StraightMove(currentPositionOfHead, endPosition, 0.5f));
            }
            else if (cmd.CommandSubType == 2 || cmd.CommandSubType == 3) // example line G03 X0.9926 Y1.2734 I-0.1125 J0.0316 
            {
                string x = cmd.GetParameterValue(ParameterType.X).ToString();
                string y = cmd.GetParameterValue(ParameterType.Y).ToString();
                if (string.IsNullOrEmpty(x)) x = "0";
                if (string.IsNullOrEmpty(y)) y = "0";
                float X = float.Parse(x);
                float Y = float.Parse(y);
                string i = cmd.GetParameterValue(ParameterType.I).ToString();
                string j = cmd.GetParameterValue(ParameterType.J).ToString();
                if (string.IsNullOrEmpty(i)) i = "0";
                if (string.IsNullOrEmpty(j)) j = "0";
                float I = float.Parse(i);
                float J = float.Parse(j);
                   
                CNCHead.transform.position = transform.InverseTransformDirection(StartPosObj.transform.position); // Set the head to the position the arc should start in.
               StartCoroutine(ArcMove(I:I,J:J,X:X,Y:Y,clockWise:cmd.CommandSubType == 2));
                ///<summary>
                /// G03 / G02 X.. Y.. I and J are the difference from the start point to the center of the arc.
                /// I is on X axis, J is on Y axis.
                /// Great tutorial and example on how to write G02 / G03 https://www.youtube.com/watch?v=gXSlyxMiZ6s 
                /// </summary>


            }


        }
    }
        IEnumerator StraightMove(Vector3 start, Vector3 end, float speed = 1f)
        {
            float distanceThreshold = 0.1f;
            
            while (Vector2.Distance(CNCHead.transform.localPosition, end) > distanceThreshold)
            {
                CNCHead.transform.localPosition = Vector3.Lerp(CNCHead.transform.localPosition, end, (Time.deltaTime*(speed)));
                yield return null;
            }
            DoneWithLine = true;
            yield return null; 
        }

        IEnumerator ArcMove(float I, float J,float X, float Y ,bool clockWise,float speed =20f)
        {
            ///<summary>
            /// G03 / G02 X.. Y.. I and J are the difference from the start point to the center of the arc.
            /// I is on X axis, J is on Y axis.
            /// </summary>
           float distanceThreshold = 0.1f;
           CentPointObj.transform.localPosition = new Vector3(CNCHead.transform.localPosition.x +(I/2), CNCHead.transform.localPosition.y, CNCHead.transform.localPosition.z +(J/2));
           EndPosObj.transform.localPosition = new Vector3(CNCHead.transform.localPosition.x + (X/2), CNCHead.transform.localPosition.y, CNCHead.transform.localPosition.z + (Y/2));
        yield return null; // just to pop the cubes in their pos
            while (Vector3.Distance(CNCHead.transform.localPosition, EndPosObj.transform.localPosition) > distanceThreshold)
            {
         
              CNCHead.transform.RotateAround(transform.InverseTransformDirection(CentPointObj.transform.position), new Vector3(0,clockWise?1:-1,0), (Time.deltaTime * speed));

            yield return null;
            }
            DoneWithLine = true;
            yield return null;

        }

}
