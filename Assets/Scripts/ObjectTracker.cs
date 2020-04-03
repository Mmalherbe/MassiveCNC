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
    // Here the starting values will be determined
    void Start()
    {
        c = 1;
        Speed = 10;
        destination = transformToTrack.transform.position;
    }
    // This is fixed update so it will look at the time between frames instead of the count of frames
    void FixedUpdate()
    {
        if (gcParser.FileLoaded == true)
        {
            nextline();
            if (transformToTrack.transform.position == destination)
            {
                c++;
                gcParser.c++;
                // this will tell gcParser it can continue to the next line of g-code
                //makes a new destination for the tracker based on the g-code
                destination = new Vector3((float)gcParser.lineList[c].X, (float)gcParser.lineList[c].Z, (float)gcParser.lineList[c].Y);
                //based on the type of g-code the speed will vary
                if (gcParser.lineList[c].G == 0)
                {
                    Speed=20;
                }
                else
                {
                    Speed=10;
                }
            }
        }
    }
    void nextline()
    {
        // this will move the tracker towards its endpoint with a constant speed
        transformToTrack.transform.position = Vector3.MoveTowards(transformToTrack.transform.position, destination, Time.deltaTime * Speed);
    }
}