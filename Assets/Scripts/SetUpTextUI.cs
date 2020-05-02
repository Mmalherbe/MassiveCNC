using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Assets.Scripts.classes;
using TMPro;
using UnityEngine;

public class SetUpTextUI : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TMP_Dropdown FontDropDown;
    [SerializeField] private TMP_Dropdown FontStyleDropDown;
    [SerializeField] private TextToPath textToPath;
    [SerializeField] private bool UseSVGFonts = true;

    void Start()
    {
        if (UseSVGFonts)
        {
            var poep = Directory.GetFiles(Path.Combine(Application.dataPath, "FontSVGs"));
            foreach(string kak in poep)
            { 
                Debug.Log(kak);
            }
            foreach (string svgFile in Directory.GetFiles(Path.Combine(Application.dataPath, "FontSVGs")))
            {
                if (svgFile.ToUpper().EndsWith("SVG"))
                {
                    Assets.Scripts.classes.Font.Svg font = XmlOperation.Deserialize<Assets.Scripts.classes.Font.Svg>(svgFile);

                }
            }
        }
        else
        {
            foreach (FontFamily fontname in FontFamily.Families)
            {
                FontDropDown.options.Add(new TMP_Dropdown.OptionData() { text = fontname.Name });
            }
        }
    }
    public void OnFontStyleChange()
    {
        textToPath.fontstyleString = FontStyleDropDown.captionText.text;
    }
    public void OnFontChange()
    {
        textToPath.fontFamilyString = FontDropDown.captionText.text;
    }
}
