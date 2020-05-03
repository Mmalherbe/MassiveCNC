using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.classes;
using TMPro;
using UnityEngine;

public class SetUpTextUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SVGToPath SvgToPath;
    [SerializeField] private TMP_Dropdown FontDropDown;
    [SerializeField] private TMP_Dropdown FontStyleDropDown;
    [SerializeField] private TextToPath textToPath;
    [SerializeField] private bool UseSVGFonts = true;

    void Start()
    {
        textToPath.svgFont = UseSVGFonts;
        if (UseSVGFonts)
        {
            var poep = Directory.GetFiles(Path.Combine(Application.dataPath, "FontSVGs"));
            foreach (string kak in poep)
            {
                Debug.Log(kak);
            }
            foreach (string svgFile in Directory.GetFiles(Path.Combine(Application.dataPath, "FontSVGs")))
            {
                if (svgFile.ToUpper().EndsWith("SVG"))
                {
                    Assets.Scripts.classes.SvgLineFile.Svg font = XmlOperation.Deserialize<Assets.Scripts.classes.SvgLineFile.Svg>(svgFile);
                    LoadInSVGFonts(font,Path.GetFileName(svgFile));
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

    private void LoadInSVGFonts(Assets.Scripts.classes.SvgLineFile.Svg font,string fontName)
    {
        List<Coords> coordsForChar = new List<Coords>();
        Coords coord = new Coords(); 
        Dictionary<string, List<Coords>> charCoords = new Dictionary<string, List<Coords>>();
        foreach (Assets.Scripts.classes.SvgLineFile.G letter in font.G)
        {
            coordsForChar = new List<Coords>();

            float minX = Mathf.Min(float.Parse(letter.Line.Min(x => x.X1)), float.Parse(letter.Line.Min(x => x.X2)));
            float maxX = Mathf.Max(float.Parse(letter.Line.Max(x => x.X1)), float.Parse(letter.Line.Max(x => x.X2)));

            float minY = Mathf.Min(float.Parse(letter.Line.Min(x => x.Y1)), float.Parse(letter.Line.Min(x => x.Y2)));
            float maxY = Mathf.Max(float.Parse(letter.Line.Max(x => x.Y1)), float.Parse(letter.Line.Max(x => x.Y2)));


            float midX = minX + ((maxX - minX) / 2);
            float midY = minY + ((maxY - minY) / 2);

            foreach (Assets.Scripts.classes.SvgLineFile.Line line in letter.Line)
            {
                coord = new Coords();
                coord.X = float.Parse(line.X1 == null ? "0" : line.X1) - midX;
                coord.Y = float.Parse(line.Y1 == null ? "0" : line.Y1) - midY;
                coordsForChar.Add(coord);
                coord = new Coords();
                coord.X = float.Parse(line.X2 == null ? "0" : line.X2) - midX;
                coord.Y = float.Parse(line.Y2 == null ? "0" : line.Y2) - midY;
                coordsForChar.Add(coord);
            }
            charCoords.Add(letter.Id, coordsForChar);
            
        }
       SvgToPath.svgFont.Add(fontName.Remove(fontName.Length-4,4), charCoords);
        FontDropDown.options.Add(new TMP_Dropdown.OptionData() { text = fontName.Remove(fontName.Length - 4, 4) });
    }
}
