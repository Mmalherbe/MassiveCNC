// These are the libraries used in this code
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Assets.Scripts.classes;
using System.Linq;

public class gcLineBuilder : MonoBehaviour
{
    // calling upondifferent classes, objects and variables
    [SerializeField] private CNC_Settings Cnc_Settings;
    public gcParser gcParser;
    public GameObject LinePrefab;
    public GameObject LinePlaceHolder;
    [SerializeField] private GameObject HomePositionObj;
    [SerializeField] private GameObject FloorPlaneObj;
    Transform origin;
    Transform destination;
    int i;
    int segment;
    int StepSize;
    GameObject[] lines;
    float LowestYValue;
    // initialization of startvalues
    void Start()
    {
        segment = 1;
        StepSize = 4000;
    }// Update is called once per frame


    void ClearLines()
    {
        lines = GameObject.FindGameObjectsWithTag("gcLine");
        //this finds all the lines
        for (int i = 0; i < lines.Length; i++)
        {
            Destroy(lines[i]);
            // this destroys all existing lines                      
        }
    }
    public void buildlinesFromGcode()
    {
        ClearLines();
        //this code will initialise line-object as many as the stepsize
        for (int i = segment; i < segment + StepSize; i++)
        {
            if (i == gcParser.lineList.Count) { break; }
            if (gcParser.lineList[i].G == 0 || gcParser.lineList[i].G == 1)
            {
                GameObject line = (GameObject)Instantiate(LinePrefab, LinePlaceHolder.transform);
                //line.transform.localPosition = LinePlaceHolder.transform.localPosition;
                line.transform.GetChild(0).localPosition = new Vector3((float)gcParser.lineList[i - 1].X, (float)gcParser.lineList[i - 1].Z, (float)gcParser.lineList[i - 1].Y);
                line.transform.GetChild(1).localPosition = new Vector3((float)gcParser.lineList[i].X, (float)gcParser.lineList[i].Z, (float)gcParser.lineList[i].Y);
                if (gcParser.lineList[i].G <= 3)
                {
                    LineRenderer linerenderer = line.gameObject.GetComponent<LineRenderer>();
                    linerenderer.startColor = (Color.green);
                    linerenderer.endColor = (Color.green);
                    linerenderer.startWidth = linerenderer.endWidth = Cnc_Settings.ScaleFactorInUnity/10;
                }
            }
        }//this updates the countertext
        LinePlaceHolder.transform.localPosition = HomePositionObj.transform.localPosition;
    }
    public void showOutLinesFromPoints(List<gcLine> gCodeLines,bool multiple = false)
    {
        if (!multiple)ClearLines();
        
        for (int i = 0; i < gCodeLines.Count; i++)
        {
            if (i == 0)
            {
                GameObject line = (GameObject)Instantiate(LinePrefab, LinePlaceHolder.transform);
                //line.transform.position = LinePlaceHolder.transform.position;
                line.transform.GetChild(0).position = new Vector3( (float)gCodeLines[i].X, (float)gCodeLines[i].Z, -(float)gCodeLines[i].Y);
                line.transform.GetChild(1).position = new Vector3((float)gCodeLines[i+1].X, (float)gCodeLines[i+1].Z, -(float)gCodeLines[i+1].Y);
                LineRenderer linerenderer = line.gameObject.GetComponent<LineRenderer>();
                    linerenderer.startColor = (Color.green);
                    linerenderer.endColor = (Color.green);
                linerenderer.startWidth = linerenderer.endWidth = Cnc_Settings.ScaleFactorInUnity / 10;
                
            }
            else
            {
                GameObject line = (GameObject)Instantiate(LinePrefab, LinePlaceHolder.transform);
                line.transform.position = LinePlaceHolder.transform.position;
                  line.transform.GetChild(0).position = new Vector3( (float)gCodeLines[i - 1].X, (float)gCodeLines[i - 1].Z, -(float)gCodeLines[i - 1].Y);
                  line.transform.GetChild(1).position = new Vector3((float)gCodeLines[i].X, (float)gCodeLines[i].Z, -(float)gCodeLines[i].Y);
                // if it is a rapidpositioning statement it will be made green
                
                    LineRenderer linerenderer = line.gameObject.GetComponent<LineRenderer>();
                    linerenderer.startColor = (Color.green);
                    linerenderer.endColor = (Color.green);
                linerenderer.startWidth = linerenderer.endWidth = Cnc_Settings.ScaleFactorInUnity / 10;
            }

        }
      
    }

}