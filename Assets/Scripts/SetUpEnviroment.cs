using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpEnviroment : MonoBehaviour
{
    [SerializeField] GameObject MoveMentField;
    [SerializeField] CNC_Settings Cnc_Settings;
    [SerializeField] Camera MoveMentFieldCamera;
    internal bool camSet = false;
    internal void ResetCamera()
    {
        if (camSet)
        {
            Cnc_Settings.ScaleFactorInUnity = (Cnc_Settings.WidthInMM / 8) / 10;
            MoveMentField.transform.localScale = new Vector3(Cnc_Settings.WidthInMM / 10, 1, Cnc_Settings.HeightInMM / 10);
            MoveMentFieldCamera.transform.position = new Vector3(MoveMentFieldCamera.transform.position.x, (MoveMentFieldCamera.transform.position.y * Cnc_Settings.ScaleFactorInUnity), MoveMentFieldCamera.transform.position.z);
            MoveMentFieldCamera.orthographicSize *= Cnc_Settings.ScaleFactorInUnity;
        }
    }

}
