using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
public class LineSwapper : MonoBehaviour
{
    public LineRenderer Line;
    public Transform origin;
    public Transform destination;
    // Use this for initialization
    void Start () {
        //initialisez the position of this specific line
        Line.SetPosition (0, origin.position);
        Line.SetPosition(1, destination.position);
           
    }}