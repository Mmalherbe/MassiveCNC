using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GCodeNet;
using System.IO;

public class CNCSimulator : MonoBehaviour
{
    
    [SerializeField] private RectTransform CNCHead = null;
    [Header("Max size. Do not change unless you are 100% sure. Values in Meters.")]
    [SerializeField]private Vector2 CNCSize = new Vector2(10, 5);
    [SerializeField] RectTransform SizeForSimOnly = null;
    [Header("Speed used for movement.")]
    [SerializeField] private float speed = 1f;
    float timeOfTravel = 5; //time after object reach a target place 
    float currentTime = 0; // actual floting time 
    float normalizedValue;
    [SerializeField] private List<Vector3> parabolaPoints = new List<Vector3>();
    [SerializeField] private bool _Moving;
    [Header("!Do not adjust these positions!")]
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private Vector2 startPositionGcodeLine;
    [SerializeField] private Vector2 currentPositionOfHead;
    
    [SerializeField] private List<string> gcodeLines;
    [SerializeField] private int lineCounter;
    public bool DoneWithLine = false;
    // Start is called before the first frame update
    void Start()
    {

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
                    startPositionGcodeLine = CNCHead.localPosition;
                    string x = cmd.GetParameterValue(ParameterType.X).ToString();
                    string y = cmd.GetParameterValue(ParameterType.Y).ToString();
                    if (string.IsNullOrEmpty(x)) x = "0";
                    if (string.IsNullOrEmpty(y)) y = "0";
                    float smoothFactorX = SizeForSimOnly.sizeDelta.x / CNCSize.x; // Make the size of the window for sim unimportant
                    float smoothFactorY = SizeForSimOnly.sizeDelta.y / CNCSize.y; // Make the size of the window for sim unimportant
                    float endX = startPositionGcodeLine.x + float.Parse(x);
                    float endY = startPositionGcodeLine.y + float.Parse(y);


                    endPosition = new Vector2(endX,endY); 
                    StartCoroutine(StraightMove(currentPositionOfHead, endPosition,0.5f));
                }
                else if (cmd.CommandSubType == 2) // example line G03 X0.9926 Y1.2734 I-0.1125 J0.0316 
                {
                    ///<summary>
                    /// G03 / G02 X.. Y.. I and J are the difference from the start point to the center of the arc.
                    /// I is on X axis, J is on Y axis.
                    /// Great tutorial and example on how to write G02 / G03 https://www.youtube.com/watch?v=gXSlyxMiZ6s 
                    /// </summary>


                }

            
        }
        IEnumerator StraightMove(Vector2 start, Vector2 end, float speed = 1f)
        {
            float distanceThreshold = 0.1f;
            
            while (Vector2.Distance(CNCHead.localPosition, end) > distanceThreshold)
            {
                CNCHead.localPosition = Vector3.Lerp(CNCHead.localPosition, end, (Time.deltaTime*(speed)));
                yield return null;
            }
            DoneWithLine = true;
            yield return null; 
        }
    }

}
