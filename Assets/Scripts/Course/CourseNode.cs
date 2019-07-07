using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class CourseNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Course course;
    public Text title;
    public RectTransform rect;
    public RectTransform column;
    public int columnID;
    public int rowID;
    public List<CourseNode> equivalent = new List<CourseNode>();
    public List<UILineRenderer> connectingLine = new List<UILineRenderer>();

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        title.text = course.subject.ToString() + " " + course.number;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse enter " + name);
        MainPanel.instance.hoverDetails.SetDetails(course);
        MainPanel.instance.hoverDetails.gameObject.SetActive(true);

        if(equivalent.Count == 0)
        {
            foreach (UILineRenderer line in connectingLine)
                line.color = DegreeTree.instance.lineHighlight;
        }
        else
        {
            OneOfNode on = transform.parent.GetComponent<OneOfNode>();
            if(on != null)
            {
                foreach (UILineRenderer line in on.connectingLine)
                    line.color = DegreeTree.instance.lineHighlight;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exit " + name);
        //StartCoroutine(DelayedDisable());

        if (equivalent.Count == 0)
        {
            foreach (UILineRenderer line in connectingLine)
                line.color = DegreeTree.instance.lineColour;
        }
        else
        {
            OneOfNode on = transform.parent.GetComponent<OneOfNode>();
            if (on != null)
            {
                foreach (UILineRenderer line in on.connectingLine)
                    line.color = DegreeTree.instance.lineColour;
            }
        }
    }

    IEnumerator DelayedDisable()
    {
        yield return new WaitForFixedUpdate();

        if (MainPanel.instance.hoverDetails.hasFocus)
            yield break;

        MainPanel.instance.hoverDetails.gameObject.SetActive(false);
    }

    public void SetCourse(Course course)
    {
        this.course = course;
        title.text = course.subject + " " + course.number;
    }
}
