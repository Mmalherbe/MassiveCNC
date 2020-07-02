using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OosterhofDesign;
using System.Linq;
using TMPro;
using UnityEngine.UIElements;
using Assets.Scripts.classes;

public class EdingCncApiController : MonoBehaviour
{
    [SerializeField] string EdingINIpath;
    [SerializeField] bool movee = false;
    [SerializeField] Vector3 MovePos;
    [SerializeField] internal static G_GetServer G_GetServer = null;
    [SerializeField] internal static G_StatusItemsposition G_StatusItemsposition = null;
    [SerializeField] internal static G_JoggingFunctions g_JoggingFunctions = null;
    private TextMeshPro TextMeshPos = null;
    [SerializeField] internal Vector3 HeadPosition;

    // Start is called before the first frame update
    void Start()
    {

        if (G_GetServer == null)
        {
            G_GetServer = new G_GetServer(EdingINIpath);
            G_GetServer.ConnectServer();//do not use any static methods from the cncwrapper
            G_StatusItemsposition = new G_StatusItemsposition(G_GetServer);
            g_JoggingFunctions = new G_JoggingFunctions(G_GetServer);
        }

        TextMeshPos= gameObject.GetComponent<TextMeshPro>() ;
    }

    // Update is called once per frame
    void Update()
    {    
            CncCartDouble machinePos = G_StatusItemsposition.GetMachinePosition();//do not use the static CncGetMachinePosition() !!!
            HeadPosition = new Vector3((float)machinePos.x, (float)machinePos.y, (float)machinePos.z);
       
    }

    

   internal void Move(Vector3 point)
    {
        CncCartDouble position = new CncCartDouble();
        position.x = point.x;
        position.y = point.y;
        position.z = point.z;
        CncCartBool Cncbool = new CncCartBool();
        Cncbool.x = 1;
        Cncbool.y = 1;
        Cncbool.z = 1;
        Cncbool.a = 1;
        Cncbool.b = 1;
        Cncbool.c = 1;
        g_JoggingFunctions.MoveTo(position, Cncbool, 10000);
        movee = false;
    }
}
