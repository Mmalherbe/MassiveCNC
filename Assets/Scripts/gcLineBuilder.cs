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


    internal void ClearLines()
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
                    linerenderer.startWidth = linerenderer.endWidth = Cnc_Settings.ScaleFactorInUnity / 10;
                }
            }
        }
    }

    public void GetLinesFromInDrag(Rect drawnRect)
    {
        Debug.Log(drawnRect);
        var x = 0;
        foreach (GameObject line in lines)
        {
            if (drawnRect.Contains(line.transform.position, true))
            {
                Debug.Log(x++);
            }
        }
    }

    public List<gcLine> ExportLinesToGcode(List<gcLine> gCodeLines)
    {
        List<gcLine> toExport = new List<gcLine>();

        for (int i = 0; i < gCodeLines.Count; i++)
        {
            gcLine line = new gcLine();
            line = gCodeLines[i];
            line.AUX1 = LinePlaceHolder.transform.GetChild(i).GetComponent<LineRenderer>().enabled;
            toExport.Add(line);

        }
        Dictionary<int, gcLine> delayLines = new Dictionary<int, gcLine>();

        for (int i = 0; i < gCodeLines.Count; i++)
        {
            if (i > 0 && i < gCodeLines.Count - 1)
            {
                if (gCodeLines[i].G != 4 && gCodeLines[i + 1].G != 4)
                {
                    if (gCodeLines[i].AUX1 != gCodeLines[i + 1].AUX1)
                    { // indrukken van spuit +- 0.5f ,
                        // loslaten -0.2f

                        gcLine delayline = new gcLine();
                        delayline.G = 4;
                        delayline.P = 0.5f;
                        delayLines.Add(i+1, delayline);
                    }
                }

            }
        }

        foreach (KeyValuePair<int, gcLine> delayline in delayLines)
        {

            toExport.Insert(delayline.Key, new gcLine { });
            toExport.Insert(delayline.Key+1, delayline.Value);
            
        }
    



            return toExport;
    }


public void showOutLinesFromPoints(List<gcLine> gCodeLines, bool multiple = false)
{
    if (!multiple) ClearLines();

    for (int i = 0; i < gCodeLines.Count; i++)
    {
        if (i == 0)
        {
            GameObject line = (GameObject)Instantiate(LinePrefab, LinePlaceHolder.transform);
            //line.transform.position = LinePlaceHolder.transform.position;
            line.transform.GetChild(0).position = new Vector3(gCodeLines[i].X != null ? (float)gCodeLines[i].X : 0f, gCodeLines[i].Z != null ? (float)gCodeLines[i].Z : 0f, gCodeLines[i].Y != null ? -(float)gCodeLines[i].Y : 0f);
            line.transform.GetChild(1).position = new Vector3(gCodeLines[i + 1].X != null ? (float)gCodeLines[i + 1].X : 0f, gCodeLines[i + 1].Z != null ? (float)gCodeLines[i + 1].Z : 0f, gCodeLines[i + 1].Y != null ? -(float)gCodeLines[i + 1].Y : 0f);
            LineRenderer linerenderer = line.gameObject.GetComponent<LineRenderer>();
            linerenderer.startColor = (Color.green);
            linerenderer.endColor = (Color.green);
            linerenderer.startWidth = linerenderer.endWidth = Cnc_Settings.ScaleFactorInUnity / 10;
            linerenderer.enabled = gCodeLines[i].AUX1 != null ? (bool)gCodeLines[i].AUX1 : false;
        }
        else
        {
            GameObject line = (GameObject)Instantiate(LinePrefab, LinePlaceHolder.transform);
            line.transform.position = LinePlaceHolder.transform.position;
            line.transform.GetChild(0).position = new Vector3(gCodeLines[i - 1].X != null ? (float)gCodeLines[i - 1].X : 0f, gCodeLines[i - 1].Z != null ? (float)gCodeLines[i - 1].Z : 0f, gCodeLines[i - 1].Y != null ? -(float)gCodeLines[i - 1].Y : 0f);
            line.transform.GetChild(1).position = new Vector3(gCodeLines[i].X != null ? (float)gCodeLines[i].X : 0f, gCodeLines[i].Z != null ? (float)gCodeLines[i].Z : 0f, gCodeLines[i].Y != null ? -(float)gCodeLines[i].Y : 0f);
            // if it is a rapidpositioning statement it will be made green

            LineRenderer linerenderer = line.gameObject.GetComponent<LineRenderer>();
            linerenderer.startColor = (Color.green);
            linerenderer.endColor = (Color.green);
            linerenderer.startWidth = linerenderer.endWidth = Cnc_Settings.ScaleFactorInUnity / 10;
            linerenderer.enabled = gCodeLines[i].AUX1 != null ? (bool)gCodeLines[i].AUX1 : false;
        }

    }

}

}