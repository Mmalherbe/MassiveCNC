// These are the libraries used in this code
using System.Collections;
using UnityEngine;
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
                    float destx = gcParser.lineList[c].X.HasValue ? gcParser.lineList[c].X.Value : 0f;
                    float desty = gcParser.lineList[c].Y.HasValue ? gcParser.lineList[c].Y.Value : 0f;
                    float destz = gcParser.lineList[c].Z.HasValue ? gcParser.lineList[c].Z.Value : 0f;
                    if (gcParser.lineList[c].G == 1)
                    {
                        if (RelativeMovement)
                        {
                            destination = new Vector3(transformToTrack.transform.localPosition.x + destx, transformToTrack.transform.localPosition.z + destz, transformToTrack.transform.localPosition.y + desty);
                        }
                        else
                        {

                            transformToTrack.transform.SetParent(HomePositionTransform.transform);
                            destination = new Vector3(destx, destz, desty);
                        }
                        codeToTrack.MoveStraight(destination);
                    }
                    else if (gcParser.lineList[c].G == 2 || gcParser.lineList[c].G == 3)
                    {
                        float desti = gcParser.lineList[c].I.HasValue ? gcParser.lineList[c].I.Value : 0f;
                        float destj = gcParser.lineList[c].J.HasValue ? gcParser.lineList[c].J.Value : 0f;
                        if (RelativeMovement)
                        {
                            bool CW = gcParser.lineList[c].G == 2;
                            Vector3 rotatePoint = new Vector3(transformToTrack.transform.localPosition.x +desti, transformToTrack.transform.localPosition.z + destj, transformToTrack.transform.localPosition.y);
                            destination = new Vector3(transformToTrack.transform.localPosition.x + destx, transformToTrack.transform.localPosition.z + destz, transformToTrack.transform.localPosition.y + desty);
                            codeToTrack.MoveArc(destination, rotatePoint, CW);
                        }
                        else
                        {
                            bool CW = gcParser.lineList[c].G == 2;
                            Vector3 rotatePoint = new Vector3(transformToTrack.transform.localPosition.x + desti, transformToTrack.transform.localPosition.z + destj, transformToTrack.transform.localPosition.y);
                            destination = new Vector3( destx,  destz,  desty);
                            codeToTrack.MoveArc(destination, rotatePoint, CW);
                        }

                    }



                    yield return null;
                }
                yield return new WaitUntil(() => codeToTrack.DoneWithLine == true);
                c++;
            }
        }
        yield return null;

    }

  
}