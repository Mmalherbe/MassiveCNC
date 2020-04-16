using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CNC_Settings
{
    [SerializeField] public static float WidthInMM { get; internal set; }
    [SerializeField] public static float HeightInMM { get; internal set; }
    [SerializeField] public static float HorizontalPadding { get; internal set; }
    [SerializeField] public static float VerticalPadding { get; internal set; }

    public static bool ScaleToMax { get; internal set; }
}
