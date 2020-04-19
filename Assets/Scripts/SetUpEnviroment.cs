using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpEnviroment : MonoBehaviour
{
    [SerializeField] GameObject MoveMentField;
    [SerializeField] CNC_Settings Cnc_Settings;
    [SerializeField] Camera MoveMentFieldCamera;
    private void Start()
    {
    
        
        Cnc_Settings.ScaleFactorInUnity = Cnc_Settings.WidthInMM / 8;
        MoveMentField.transform.localScale = new Vector3(Cnc_Settings.WidthInMM, 1, Cnc_Settings.HeightInMM);
        MoveMentFieldCamera.transform.position = new Vector3(MoveMentFieldCamera.transform.position.x, (MoveMentFieldCamera.transform.position.y * Cnc_Settings.ScaleFactorInUnity), MoveMentFieldCamera.transform.position.z);
        MoveMentFieldCamera.orthographicSize *= Cnc_Settings.ScaleFactorInUnity;

    }


}
