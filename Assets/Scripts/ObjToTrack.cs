using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ObjToTrack : MonoBehaviour
{
    public gcParser gcParser;
    public ObjectTracker Tracker;
    public GameObject objToTrack;
    public Text current;
    // Update is called once per frame
    void Update()
    {
        //this looks every frame where the tracker is and sends it to a text component
        if (gcParser.FileLoaded == true)
        {
            current.text =
                "X:" + objToTrack.transform.position.x + "\n" +
                "Y:" + objToTrack.transform.position.z + "\n" +
                "Z:" + objToTrack.transform.position.y + "\n" +
                "Speed: " + Tracker.Speed + " mm/s ";
        }
    }
}
