// These are the libraries used in this code
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class gcLineBuilder : MonoBehaviour
{
    // calling upondifferent classes, objects and variables
    public gcParser gcParser;
    public GameObject LinePrefab;
    public GameObject LinePlaceHolder;
    public TextMeshProUGUI counter;
    Transform origin;
    Transform destination;
    int i;
    int segment;
    int StepSize;
    GameObject[] lines;
    // initialization of startvalues
    void Start () {
    segment = 1;
    StepSize = 4000;
    }// Update is called once per frame
    public void buildlines()
    {
    lines = GameObject.FindGameObjectsWithTag("gcLine");
    //this finds all the lines
        for (int i = 0;i < lines.Length;i++) {
        Destroy(lines[i]);
        // this destroys all existing lines                      
        }
        //this code will initialise line-object as many as the stepsize
        for (int i = segment;i < segment + StepSize;i++) {
            if(i == gcParser.lineList.Count) { break; }
            if (gcParser.lineList[i].G == 0 || gcParser.lineList[i].G == 1)
            {
                GameObject line = (GameObject)Instantiate(LinePrefab,LinePlaceHolder.transform);
                line.transform.position = LinePlaceHolder.transform.position;
                line.transform.GetChild(0).localPosition = new Vector3((float)gcParser.lineList[i - 1].X, (float)gcParser.lineList[i - 1].Z, (float)gcParser.lineList[i - 1].Y);
                line.transform.GetChild(1).localPosition = new Vector3((float)gcParser.lineList[i].X, (float)gcParser.lineList[i].Z, (float)gcParser.lineList[i].Y);
                // if it is a rapidpositioning statement it will be made green
                if (gcParser.lineList [i].G == 0) {
                    LineRenderer linerenderer = line.gameObject.GetComponent<LineRenderer> ();
                linerenderer.startColor=(Color.green);
                linerenderer.endColor=(Color.green);
            }
        }
    }//this updates the countertext
        counter.text = "G-Code regel: " + segment + " - " + (segment + StepSize);
}}