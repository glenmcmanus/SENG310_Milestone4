using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class CourseResult : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CourseOffering course;
    public Text title;

    public void Initialize(CourseOffering course)
    {
        this.course = course;
        //set ui values
        title.text = course.course.subject.ToString() + " " + course.course.number;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse enter " + name);
        MainPanel.instance.hoverDetails.SetDetails(course.course, GetComponent<RectTransform>());
        MainPanel.instance.hoverDetails.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exit " + name);
        StartCoroutine(DelayedDisable());
    }

    IEnumerator DelayedDisable()
    {
        yield return new WaitForFixedUpdate();

        if (MainPanel.instance.hoverDetails.hasFocus)
            yield break;

        MainPanel.instance.hoverDetails.gameObject.SetActive(false);
    }
}
