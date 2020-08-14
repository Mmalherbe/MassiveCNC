using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDragController : MonoBehaviour
{
    [SerializeField] CNC_Settings Cnc_Settings;
    [SerializeField] gcLineBuilder GcLineBuilder;
    [SerializeField] RectTransform cnc_Screen;
    [SerializeField] private Vector2 cnc_Image_Size;
    private bool sizeSet = false;
    [SerializeField] float scaleX;
    [SerializeField] float scaleY;
    [SerializeField] Vector3 MouseDownPos;
    [SerializeField] Vector3 MouseUpPos;
    [SerializeField] Rect draggedRect;
    private bool isDragging = false;
    private void Start()
    {
        
    }


    private void OnGUI()
    {
        if (isDragging)
        {
            MouseUpPos = Input.mousePosition;
            draggedRect = ScreenHelper.GetScreenRect(MouseDownPos, MouseUpPos);
            ScreenHelper.DrawScreenRect(draggedRect, Color.red);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!sizeSet)
        {
            cnc_Image_Size = cnc_Screen.rect.size;
            sizeSet = checkSize();
            if (sizeSet)
            {
                scaleX = Cnc_Settings.WidthInMM / cnc_Image_Size.x;
                scaleY = Cnc_Settings.HeightInMM / cnc_Image_Size.y;
            }
        }

        Vector2 localMousePosition = cnc_Screen.InverseTransformPoint(Input.mousePosition);
        if (cnc_Screen.rect.Contains(localMousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseDownPos = Input.mousePosition;
                isDragging = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                GcLineBuilder.GetLinesFromInDrag(draggedRect);
           
            }
        }
        else
        {
            isDragging = false;
        }
    }

    internal bool checkSize()
    {
        Debug.Log("checking");
        return cnc_Image_Size.x > 0 && cnc_Image_Size.y > 0;
    }
}

