using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hasFocus;
    public RectTransform rect;
    Semester semester = Semester.Fall;
    public Text semesterText;
    public Text courseText;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        semesterText.text = semester.ToString();
    }

    public void SetDetails(Course course, RectTransform trigger)
    {
        rect.position = trigger.position;
        courseText.text = course.department + " " + course.number;
    }

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
