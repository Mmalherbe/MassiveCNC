// These are the libraries used in this code
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ObjectTracker : MonoBehaviour
{// calling upon different classes, objects and variables
    public GameObject transformToTrack;
    public ObjToTrack codeToTrack;
    Vector3 origin;
    Vector3 destination;
    int c;
    public int Speed;
    public gcParser gcParser;
    [SerializeField] private bool RelativeMovement = false;
    [SerializeField] private GameObject HomePositionObj;
    [SerializeField] private Transform HomePositionTransform;
    [Header("Do not change below")]
    [SerializeField] private GameObject AxisParent;
    // Here the starting values will be determined
    bool runOnce = true;
    void Start()
    {
        HomePositionTransform = HomePositionObj.transform;
        c = 0;
        Speed = 10;
        if (!RelativeMovement)
        {
            AxisParent.transform.SetParent(HomePositionTransform);
        }

    }
    // This is fixed update so it will look at the time between frames instead of the count of frames
    
        public void StartMoveMent()
    {
        StartCoroutine(MakeObjectFollowPath());
    }

    IEnumerator MakeObjectFollowPath()
    {
        if (gcParser.FileLoaded == true)
        {
            while (c < gcParser.lineList.Count)
            {
                if (codeToTrack.DoneWithLine)
                {
                    if(gcParser.lineList[c].G == 1)
                    if (RelativeMovement)
                    {
                        destination = new Vector3(transformToTrack.transform.localPosition.x + gcParser.lineList[c].X, transformToTrack.transform.localPosition.z + gcParser.lineList[c].Z, transformToTrack.transform.localPosition.y + gcParser.lineList[c].Y);
                    }
                    else
                    {
                        transformToTrack.transform.SetParent(HomePositionTransform.transform); 
                        destination = new Vector3( gcParser.lineList[c].X,  gcParser.lineList[c].Z, gcParser.lineList[c].Y);
                    }
                    else if(gcParser.lineList[c].G == 2 || gcParser.lineList[c].G == 3)
                    {
                        bool CW = gcParser.lineList[c].G == 2;
                        Vector3 rotatePoint = new Vector3(transformToTrack.transform.localPosition.x + gcParser.lineList[c].I, transformToTrack.transform.localPosition.z + gcParser.lineList[c].J, transformToTrack.transform.localPosition.y);
                        destination = new Vector3(gcParser.lineList[c].X, gcParser.lineList[c].Z, gcParser.lineList[c].Y);
                        codeToTrack.MoveArc(destination,rotatePoint,CW);


                    }


                    codeToTrack.MoveStraight(destination);
                    yield return null;
                }
                yield return new WaitUntil(() => codeToTrack.DoneWithLine == true);
                c++;
            }
        }
        yield return null;

    }

  
}