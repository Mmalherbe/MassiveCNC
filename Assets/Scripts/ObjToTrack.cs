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
    [SerializeField] internal bool DoneWithLine = true;
    [SerializeField] private float speed = 1f;
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
    public void MoveStraight(Vector3 endPos)
    {
        if (DoneWithLine)
        {
            DoneWithLine = false;
            StartCoroutine(StraightMove( endPos));
        }
        
    }
    public void MoveArc(Vector3 endPoint,Vector3 rotatePoint,bool cww)
    {
        if (DoneWithLine)
        {
            DoneWithLine = false;
            StartCoroutine(ArcMove(endPoint,rotatePoint,cww));
        }
    }
    IEnumerator ArcMove(Vector3 end,Vector3 rotatePoint,bool cww)
    {

        float distanceThreshold = 0.1f;
        while (Vector3.Distance(transform.localPosition, end) > distanceThreshold)
        {
            transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * speed);
            yield return null;
        }
        DoneWithLine = true;
        yield return null;
    }
    IEnumerator StraightMove(Vector3 end)
    {
        float distanceThreshold = 0.1f;
        while (Vector3.Distance(transform.localPosition,end) > distanceThreshold)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, end, (Time.deltaTime * speed));
            yield return null;
        }
        DoneWithLine = true;
        yield return null;




    }

}
