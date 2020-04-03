// These are the libraries used in this code
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ObjectTracker : MonoBehaviour
{// calling upon different classes, objects and variables
    public GameObject transformToTrack;
    Vector3 origin;
    Vector3 destination;
    int c;
    public int Speed;
    public gcParser gcParser;
    [SerializeField] private GameObject StartPoint;
    [SerializeField] private GameObject EndPoint;
    // Here the starting values will be determined
    void Start()
    {
        c = 0;
        Speed = 10;
        destination = transformToTrack.transform.position;
        EndPoint.transform.position = destination;
        StartPoint.transform.position = transformToTrack.transform.position;
    }
    // This is fixed update so it will look at the time between frames instead of the count of frames
    void FixedUpdate()
    {
        if (gcParser.FileLoaded == true)
        {
            nextline();
            if (transformToTrack.transform.localPosition == destination)
            {
                StartPoint.transform.position = transformToTrack.transform.position;
                c++;
                gcParser.c++;
                // this will tell gcParser it can continue to the next line of g-code
                //makes a new destination for the tracker based on the g-code
                //destination = new Vector3((float)transformToTrack.transform.localPosition.x + gcParser.lineList[c].X, (float)transformToTrack.transform.localPosition.z +gcParser.lineList[c].Z, (float)transformToTrack.transform.localPosition.y +gcParser.lineList[c].Y);
               
                //based on the type of g-code the speed will vary
               
            }
        }
    }
    void nextline()
    {
        if (gcParser.lineList[c].G == 0)
        {
            Speed = 20;
        }
        else
        {
            Speed = 10;
        }
        StartPoint.transform.localPosition = transformToTrack.transform.localPosition;
        destination = new Vector3(gcParser.lineList[c].X, gcParser.lineList[c].Z, gcParser.lineList[c].Y);
        EndPoint.transform.localPosition = destination;

        // this will move the tracker towards its endpoint with a constant speed
        transformToTrack.transform.position = Vector3.MoveTowards(transformToTrack.transform.position, destination, Time.deltaTime *(Speed/10));
    }
}