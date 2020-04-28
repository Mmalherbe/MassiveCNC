using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject TextToGCodePanel;
    [SerializeField] GameObject SVGToGCodePanel;
    [SerializeField] GameObject PredefinedGCodePanel;
    [SerializeField] TMP_Dropdown MenuDropDown;

    public void SwapMenu()
    {
        HideAllMenus();
        switch (MenuDropDown.value)
        {
            default:
            case 0:
                TextToGCodePanel.SetActive(true);
                break;
            case 1:
                SVGToGCodePanel.SetActive(true);
                break;
            case 2:
                PredefinedGCodePanel.SetActive(true);
                break;
        }

    }
    private void HideAllMenus()
    {

        TextToGCodePanel.SetActive(false);
        SVGToGCodePanel.SetActive(false);
        PredefinedGCodePanel.SetActive(false);
    }
}
