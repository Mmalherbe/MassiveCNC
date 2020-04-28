using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SetUpTextUI : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TMP_Dropdown FontDropDown;
    [SerializeField] private TMP_Dropdown FontStyleDropDown;
    [SerializeField] private TextToPath textToPath;
    
    void Start()
    {
        foreach(FontFamily fontname in FontFamily.Families)
        {
            FontDropDown.options.Add(new TMP_Dropdown.OptionData() { text = fontname.Name });
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
