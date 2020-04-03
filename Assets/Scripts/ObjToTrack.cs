using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ObjToTrack : MonoBehaviour
{
    public gcParser gcParser;
    public ObjectTracker Tracker;
    public TextMeshProUGUI current;
    [SerializeField] private GameObject Parent;
    // Update is called once per frame
    void Update()
    {
        //this looks every frame where the tracker is and sends it to a text component
        if (gcParser.FileLoaded == true)
        {
            current.text =
                "X:" + (transform.localPosition.x )+ "\n" +
                "Y:" + ( transform.localPosition.z )+ "\n" +
                "Z:" + ( transform.localPosition.y )+ "\n" +
                "Speed: " + Tracker.Speed + " mm/s ";
        }
    }
}
