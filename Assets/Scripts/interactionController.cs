using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class interactionController : MonoBehaviour
{

    [SerializeField] SetUpEnviroment setupEnviroment;
    [SerializeField] gcParser gcParser;
    [SerializeField] FileController fileController;
    [SerializeField] TextToPath TextToPath;
    [SerializeField] SVGToPath SvgToPath;
    [SerializeField] CNC_Settings Cnc_Settings;
    [SerializeField] SetUpEnviroment setUpEnviroment;

    [SerializeField] TMP_InputField HeightInputField;
    [SerializeField] TMP_InputField WidthInputField;
    [SerializeField] TMP_InputField HeightMarginInputField;
    [SerializeField] TMP_InputField WidthMarginInputField;
    [SerializeField] Toggle ScaleToMaxToggle;

    [SerializeField] Slider RatioScaleSlider;
    [SerializeField] Slider HorizontalScaleSlider;
    [SerializeField] Slider VerticalScaleSlider;

    [SerializeField] TextMeshProUGUI MinXValueHolder;
    [SerializeField] TextMeshProUGUI MaxXValueHolder;
    [SerializeField] TextMeshProUGUI MinYValueHolder;
    [SerializeField] TextMeshProUGUI MaxYValueHolder;

    [SerializeField] TMP_InputField StartLocX;
    [SerializeField] TMP_InputField StartLocY;
    [SerializeField] TMP_InputField StartLocZ;

    [SerializeField] TMP_InputField MidPointLocX;
    [SerializeField] TMP_InputField MidPointLocY;
    [SerializeField] TMP_InputField MidPointLocZ;



    internal bool scaleSet = false;


    private void Start()
    {
        if (string.IsNullOrEmpty(WidthInputField.text)) WidthInputField.text = Cnc_Settings.WidthInMM.ToString();
        if (string.IsNullOrEmpty(HeightInputField.text)) HeightInputField.text = Cnc_Settings.HeightInMM.ToString();

        if (string.IsNullOrEmpty(WidthMarginInputField.text)) WidthMarginInputField.text = Cnc_Settings.HorizontalPaddingInMM.ToString();
        if (string.IsNullOrEmpty(HeightMarginInputField.text)) HeightMarginInputField.text = Cnc_Settings.VerticalPaddingInMM.ToString();

        if (string.IsNullOrEmpty(StartLocX.text)) StartLocX.text = Cnc_Settings.DefaultHomeX.ToString();
        if (string.IsNullOrEmpty(StartLocY.text)) StartLocY.text = Cnc_Settings.DefaultHomeY.ToString();
        if (string.IsNullOrEmpty(StartLocZ.text)) StartLocZ.text = Cnc_Settings.DefaultHomeZ.ToString();

        if (string.IsNullOrEmpty(MidPointLocX.text)) MidPointLocX.text = Cnc_Settings.DefaultHomeX.ToString();
        if (string.IsNullOrEmpty(MidPointLocY.text)) MidPointLocY.text = Cnc_Settings.DefaultHomeY.ToString();
        if (string.IsNullOrEmpty(MidPointLocZ.text)) MidPointLocZ.text = Cnc_Settings.DefaultHomeZ.ToString();


        setupEnviroment.camSet = true;
        setUpEnviroment.ResetCamera();


    }
    public void updateScaleSliders( float maxHorizontal, float maxVertical, float currentScaleHorizontal, float currentScaleVertical)
    {
        HorizontalScaleSlider.minValue = 0.01f;
        HorizontalScaleSlider.maxValue = Mathf.Floor(maxHorizontal);
        VerticalScaleSlider.minValue = 0.01f;
        VerticalScaleSlider.maxValue = Mathf.Floor(maxVertical);
        RatioScaleSlider.minValue = 0.01f;
        if (Cnc_Settings.ScaleToMax)
        {
            RatioScaleSlider.value = Cnc_Settings.ScaleFactorForMax;
        }
        RatioScaleSlider.maxValue = Cnc_Settings.ScaleFactorForMax;
        HorizontalScaleSlider.value = currentScaleHorizontal;
        VerticalScaleSlider.value = currentScaleVertical;

    }
    public void ParseTextToGcode_Click()
    {
        TextToPath.ClickParseTextToGcode();
    }
    public void ShowPathFromGcodeFile_Click()
    {
        fileController.openCNCfile();
    }
    public void LoadSVGToPath_Click()
    {
        fileController.openSVGfile();
    }
    public void SVGClassToggle_Changed(Toggle _toggle)
    {
        Cnc_Settings.ScaleToMax = _toggle.isOn;
        SvgToPath.ToggleToggled(_toggle);
    }
    public void WidthCNCChanged()
    {
        if (WidthInputField.text.Length == 0) return;
        Cnc_Settings.WidthInMM = float.Parse(WidthInputField.text);
        setupEnviroment.ResetCamera();

    }
    public void WidthMarginCNCChanged()
    {
        if (WidthMarginInputField.text.Length == 0) return;
        Cnc_Settings.HorizontalPaddingInMM = float.Parse(WidthMarginInputField.text);
        setupEnviroment.ResetCamera();

    }
    public void HeightMarginCNCChanged()
    {
        if (HeightMarginInputField.text.Length == 0) return;

        Cnc_Settings.VerticalPaddingInMM = float.Parse(HeightMarginInputField.text);
        setupEnviroment.ResetCamera();

    }
    public void HeightCNCChanged()
    {
        if (HeightInputField.text.Length == 0) return;

        Cnc_Settings.HeightInMM = float.Parse(HeightInputField.text);
        setupEnviroment.ResetCamera();
    }

    public void RatioScaleChanged()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
        if (scaleSet)
        {
            HorizontalScaleSlider.value = VerticalScaleSlider.value = RatioScaleSlider.value;
            gcParser.scaleToUseHorizontal = gcParser.scaleToUseVertical = RatioScaleSlider.value;
            gcParser.RedrawWithUpdatedScale();
        }
    }
    public void HorizontalScaleChanged()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
        if (scaleSet)
        {
            gcParser.scaleToUseHorizontal = HorizontalScaleSlider.value;
            gcParser.RedrawWithUpdatedScale();
        }
    }
    public void VerticalScaleChanged()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
        if (scaleSet)
        {
            gcParser.scaleToUseVertical = VerticalScaleSlider.value;
            gcParser.RedrawWithUpdatedScale();
        }
    }
    public void ResetScale_Click()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
        gcParser.scaleToUseHorizontal = 1;
        gcParser.scaleToUseVertical = 1;
        gcParser.RedrawWithUpdatedScale();
    }
    public void ScaleToMax_Toggled()
    {
        Cnc_Settings.ScaleToMax = ScaleToMaxToggle.isOn;
    }

    public void UpdateMinMaxValues()
    {
       
        float[] MinMaxValues = gcParser.getMinMaxValues();
        MinXValueHolder.text = MinMaxValues[0].ToString();
        MaxXValueHolder.text = MinMaxValues[1].ToString();
        MinYValueHolder.text = MinMaxValues[2].ToString();
        MaxYValueHolder.text = MinMaxValues[3].ToString();
        scaleSet = true;

    }

    public void SVGToPathParse_Click()
    {
        SvgToPath.PathToGCode();

    }

    public void StartLocationInputChanged()
    {
        if (StartLocX.text.Length == 0) StartLocX.text = gcParser.HomePositionObj.transform.position.x.ToString();
        if (StartLocY.text.Length == 0) StartLocY.text = gcParser.HomePositionObj.transform.position.y.ToString();
        if (StartLocZ.text.Length == 0) StartLocZ.text = gcParser.HomePositionObj.transform.position.z.ToString();

        float startX = float.Parse(StartLocX.text);
        float startY = float.Parse(StartLocY.text);
        float startZ = float.Parse(StartLocZ.text);

        if (Mathf.Abs(startX) < Mathf.Abs(Cnc_Settings.WidthInMM / 2) &&
            Mathf.Abs(startY) < Mathf.Abs(Cnc_Settings.HeightInMM / 2) &&
            Mathf.Abs(startZ) < Mathf.Abs(Cnc_Settings.MinimumZinMM) &&
            Mathf.Abs(startZ) < Mathf.Abs(Cnc_Settings.MaximumZinMM)
            )
        {
            Vector3 newStartLoc = new Vector3(startX, startY, startZ);
            gcParser.StartPositionGcode.transform.position = newStartLoc;
        }

    }

    public void MidPointLocationInputChanged()
    {
        if (MidPointLocX.text.Length == 0) MidPointLocX.text = gcParser.HomePositionObj.transform.position.x.ToString();
        if (MidPointLocY.text.Length == 0) MidPointLocY.text = gcParser.HomePositionObj.transform.position.y.ToString();
        if (MidPointLocZ.text.Length == 0) MidPointLocZ.text = gcParser.HomePositionObj.transform.position.z.ToString();

        float MidPointX = float.Parse(MidPointLocX.text);
        float MidPointY = float.Parse(MidPointLocY.text);
        float MidPointZ = float.Parse(MidPointLocZ.text);

        if (Mathf.Abs(MidPointX) > Mathf.Abs(Cnc_Settings.WidthInMM / 2) ||
            Mathf.Abs(MidPointY) > Mathf.Abs(Cnc_Settings.HeightInMM / 2) ||
            Mathf.Abs(MidPointZ) > Mathf.Abs(Cnc_Settings.MinimumZinMM) ||
            Mathf.Abs(MidPointZ) > Mathf.Abs(Cnc_Settings.MaximumZinMM)
            )
        {
            MidPointX = 0f;
            MidPointY = 0f;
            MidPointZ = 0f;
        }
            Vector3 newMidPointLoc = new Vector3(MidPointX, MidPointY, MidPointZ);
            gcParser.MiddlePointGcode.transform.position = newMidPointLoc;
        

    }
}
