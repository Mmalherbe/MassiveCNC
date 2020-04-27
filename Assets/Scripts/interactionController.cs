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


    [SerializeField] TMP_InputField HeightInputField;
    [SerializeField] TMP_InputField WidthInputField;
    [SerializeField] TMP_InputField HeightMarginInputField;
    [SerializeField] TMP_InputField WidthMarginInputField;
    [SerializeField] Toggle ScaleToMaxToggle;

    [SerializeField] Slider RatioScaleSlider;
    [SerializeField] Slider HorizontalScaleSlider;
    [SerializeField] Slider VerticalScaleSlider;

    public void updateScaleSliders(float minHorizontal,float maxHorizontal, float minVertical,float maxVertical,float currentScaleHorizontal,float currentScaleVertical)
    {
        HorizontalScaleSlider.minValue = minHorizontal;
        HorizontalScaleSlider.maxValue = maxHorizontal;
        VerticalScaleSlider.minValue = minVertical;
        VerticalScaleSlider.maxValue = maxVertical;
        RatioScaleSlider.minValue = 0f;
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
        SvgToPath.ToggleToggled(_toggle);
    }
    public void WidthCNCChanged()
    {
        Cnc_Settings.WidthInMM = float.Parse(WidthInputField.text);
        setupEnviroment.ResetCamera();

    }
    public void WidthMarginCNCChanged()
    {
        Cnc_Settings.HorizontalPaddingInMM = float.Parse(WidthMarginInputField.text);
        setupEnviroment.ResetCamera();

    }
    public void HeightMarginCNCChanged()
    {
        Cnc_Settings.VerticalPaddingInMM = float.Parse(HeightMarginInputField.text);
        setupEnviroment.ResetCamera();

    }
    public void HeightCNCChanged()
    {
        Cnc_Settings.HeightInMM = float.Parse(HeightInputField.text);
        setupEnviroment.ResetCamera();
    }

    public void RatioScaleChanged()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
    }
    public void HorizontalScaleChanged()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
        gcParser.scaleToUseHorizontal = HorizontalScaleSlider.value;
    }
    public void VerticalScaleChanged()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
        gcParser.scaleToUseVertical = VerticalScaleSlider.value;
    }
    public void ResetScale_Click()
    {
        Cnc_Settings.ScaleToMax = false;
        ScaleToMaxToggle.isOn = false;
        gcParser.scaleToUseHorizontal = 1;
        gcParser.scaleToUseVertical = 1;

    }
    public void ScaleToMax_Toggled()
    {
        Cnc_Settings.ScaleToMax = ScaleToMaxToggle.isOn;
    }
}
