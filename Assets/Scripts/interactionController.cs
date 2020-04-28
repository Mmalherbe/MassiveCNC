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
    internal bool scaleSet = false;


    private void Start()
    {
        if (string.IsNullOrEmpty(WidthInputField.text)) WidthInputField.text = Cnc_Settings.WidthInMM.ToString();
        if (string.IsNullOrEmpty(HeightInputField.text)) HeightInputField.text = Cnc_Settings.HeightInMM.ToString();

        if (string.IsNullOrEmpty(WidthMarginInputField.text)) WidthMarginInputField.text = Cnc_Settings.HorizontalPaddingInMM.ToString();
        if (string.IsNullOrEmpty(HeightMarginInputField.text)) HeightMarginInputField.text = Cnc_Settings.VerticalPaddingInMM.ToString();

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

}
