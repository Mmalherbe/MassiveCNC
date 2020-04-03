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
    void Update () 
    {// this code will interpret the pressing of the up or down key to change between segments of lines
        if (Input.GetKeyDown (KeyCode.UpArrow) ) 
        {
            segment = segment + StepSize;
            buildlines();
        }
        if (Input.GetKeyDown (KeyCode.DownArrow) ) {
            segment = segment - StepSize;
            buildlines();
        }
    }public void buildlines()
    {
    lines = GameObject.FindGameObjectsWithTag("lines");
    //this finds all the lines
        for (int i = 0;i < lines.Length;i++) {
        Destroy(lines[i]);
        // this destroys all existing lines                      
        }
        //this code will initialise line-object as many as the stepsize
        for (int i = segment;i < segment + StepSize;i++) {
            if (gcParser.lineList[i].G == 0 || gcParser.lineList[i].G == 1)
            {
                GameObject line = (GameObject)Instantiate(LinePrefab);
                line.transform.GetChild(0).position = new Vector3((float)gcParser.lineList[i - 1].X, (float)gcParser.lineList[i - 1].Z, (float)gcParser.lineList[i - 1].Y);
                line.transform.GetChild(1).position = new Vector3((float)gcParser.lineList[i].X, (float)gcParser.lineList[i].Z, (float)gcParser.lineList[i].Y);
                // if it is a rapidpositioning statement it will be made green
                if (gcParser.lineList [i].G == 0) {
                    LineRenderer linerenderer = line.gameObject.GetComponent<LineRenderer> ();
                linerenderer.startColor=(Color.green);
                linerenderer.endColor=(Color.green);
            }//this will set Lineplaceholder as parent 
                line.transform.SetParent(LinePlaceHolder.transform);
        }
    }//this updates the countertext
        counter.text = "G-Code regel: " + segment + " - " + (segment + StepSize);
}}