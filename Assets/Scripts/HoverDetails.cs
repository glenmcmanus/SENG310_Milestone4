using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverDetails : CourseDetails, IPointerEnterHandler, IPointerExitHandler
{
    public bool hasFocus;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        hasFocus = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hasFocus = false;
        gameObject.SetActive(false);
    }
}
