using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OosterhofDesign;
using System.Linq;
using TMPro;
public class CncApiScript : MonoBehaviour
{
    internal static G_GetServer G_GetServer = null;
    internal static G_StatusItemsposition G_StatusItemsposition = null;
    private TextMeshPro TextMeshPos = null;


    // Start is called before the first frame update
    void Start()
    {
        if (G_GetServer == null)
        {
            G_GetServer = new G_GetServer("");
            G_GetServer.ConnectServer();//do not use any static methods from the cncwrapper
            G_StatusItemsposition = new G_StatusItemsposition(G_GetServer);
        }

        TextMeshPos= gameObject.GetComponent<TextMeshPro>() ;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TextMeshPos != null)
        {
            CncCartDouble machinePos = G_StatusItemsposition.GetMachinePosition();//do not use the static CncGetMachinePosition() !!!

            TextMeshPos.text = "X:" + machinePos.x + "\nY:" + machinePos.y + "\nZ:" + machinePos.z;
        }
        
    }

}
