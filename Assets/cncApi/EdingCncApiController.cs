using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OosterhofDesign;
using System.Linq;
using TMPro;
using UnityEngine.UIElements;
using Assets.Scripts.classes;
using Assets.Scripts.classes.Font;
using System.Diagnostics;
using System.IO;
using UnityEditor.PackageManager;

public class EdingCncApiController : MonoBehaviour
{
    [SerializeField] string EdingINIpath;
    [SerializeField] CNC_Settings Cnc_Settings;
    [SerializeField] bool movee = false;
    [SerializeField] Vector3 MovePos;
    [SerializeField] internal static G_GetServer G_GetServer = null;
    [SerializeField] internal static G_StatusItemsposition G_StatusItemsposition = null;
    [SerializeField] internal static G_JoggingFunctions g_JoggingFunctions = null;
    [SerializeField] internal static G_CommandsJobInterpreter g_CommandsJobInterpreter = null;
    private TextMeshPro TextMeshPos = null;
    [SerializeField] internal Vector3 HeadPosition;
    private bool _Connected = false;
    private bool ServerError = false;
    private bool Moving = false;
    private bool DoneWithLine = false;

    // Start is called before the first frame update
    void Start()
    {
        TextMeshPos = gameObject.GetComponent<TextMeshPro>();
        G_GetServer = new G_GetServer(EdingINIpath);
        G_GetServer.ConnectServer();
        G_StatusItemsposition = new G_StatusItemsposition(G_GetServer);
        g_JoggingFunctions = new G_JoggingFunctions(G_GetServer);
        g_CommandsJobInterpreter = new G_CommandsJobInterpreter(G_GetServer);
        _Connected = true;

    }
    public bool IsConnected()
    {
        return _Connected;
    }
    public void Connect(bool turnOn)
    {
        if (turnOn)
        {
            try
            {
                if (_Connected)
                {
                    G_GetServer.ConnectServer();
                    _Connected = true;
                }
            }
            catch { _Connected = true; }
        }


        else
        {
            try
            {
                G_GetServer.DisConnectServer();
                _Connected = false;
            }
            catch { _Connected = false; }
        }



    }

    public void OnDisable()
    {
        Connect(false);

    }
    // Update is called once per frame
    void Update()
    {
        if (_Connected)
        {
            CncCartDouble machinePos = G_StatusItemsposition.GetMachinePosition();//do not use the static CncGetMachinePosition() !!!
            if (machinePos != null)
            {
                HeadPosition = new Vector3((float)machinePos.x, (float)machinePos.y, (float)machinePos.z);
            }
            else
            {
                _Connected = false;
            }

        }
    }
    public void LoadJob(string urlToCodeToLoad)
    {
        var a = g_CommandsJobInterpreter.LoadJob(urlToCodeToLoad); // returns  CNC_RC_OK if loaded OK.

        if (a == CncRc.CNC_RC_OK)
        {
            g_CommandsJobInterpreter.RunOrResumeJob();
        }

    }



    IEnumerator Move(gcLine moveTo)
    {
        float distanceThreshold = 0.1f;
        CncCartDouble position = new CncCartDouble();
        position.x = (double)moveTo.X;
        position.y = (double)moveTo.Y;
        position.z = (double)moveTo.Z;
        CncCartBool Cncbool = new CncCartBool();
        Cncbool.x = 1;
        Cncbool.y = 1;
        Cncbool.z = 1;
        Cncbool.a = 1;
        Cncbool.b = 1;
        Cncbool.c = 1;
        while(Vector3.Distance(HeadPosition,new Vector3((float)moveTo.X, (float)moveTo.Y, (float)moveTo.Z)) < distanceThreshold)
        {
            g_JoggingFunctions.MoveTo(position, Cncbool, (double)moveTo.F);
            yield return null;
        }
        DoneWithLine = true;
        Moving = false;
        movee = false;
        yield return null;
    }
    internal void MoveAlongPath(List<gcLine> gcodePath)
    {
        StartCoroutine(StartMovementAroundPath(gcodePath));
    }
    internal void SendGCode()
    {
        


    }
    IEnumerator StartMovementAroundPath(List<gcLine> gcodePath)
    {
        int c = 0;
        while( c < gcodePath.Count )
        {
            if (DoneWithLine) {
                DoneWithLine = false;
                if (!Cnc_Settings.RelativeMovement || c == 0)
                {
                    StartCoroutine(Move(gcodePath[c]));
                }
                else
                {
                    gcLine relativeLine = new gcLine();
                    relativeLine = gcodePath[c];
                    relativeLine.X = gcodePath[c].X - gcodePath[c - 1].X;
                    relativeLine.Y = gcodePath[c].Y - gcodePath[c - 1].Y;
                    relativeLine.Z = gcodePath[c].Z - gcodePath[c - 1].Z;
                    StartCoroutine(Move(relativeLine));
                }
            }
            yield return new WaitUntil(() => DoneWithLine == true);
            c++;
        }
       yield return null;
    }
}
