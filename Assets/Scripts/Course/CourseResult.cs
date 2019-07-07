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
    public ResultColumn column;
    public Button addToDT;
    public Button removeFromDT;

    public void Initialize(CourseOffering course)
    {
        this.course = course;
        title.text = course.course.subject.ToString() + " " + course.course.number;

        foreach(Course c in DegreeTree.instance.currentCourses)
        {
            if(course.course == c)
            {
                addToDT.interactable = false;
                return;
            }
        }

        foreach(OneOfNode oon in DegreeTree.instance.oneOfNodes)
        {
            foreach(CourseNode cn in oon.courses)
            {
                if(cn.course == course.course)
                {
                    addToDT.interactable = false;
                    return;
                }
            }
        }

        foreach(ElectiveNode en in DegreeTree.instance.electiveNodes)
        {
            if(course.course == en.course)
            {
                //TODO: change the button to 'remove course from DT'
                addToDT.gameObject.SetActive(false);
                removeFromDT.gameObject.SetActive(true);
                return;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse enter " + name);

        MainPanel.instance.hoverDetails.offerings.Clear();
        MainPanel.instance.hoverDetails.SetDetails(this); //course.course);
        MainPanel.instance.hoverDetails.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exit " + name);

        if(!MainPanel.instance.hoverDetails.hasFocus)
        {
            StartCoroutine(DelayedDisable());
        }
    }

    IEnumerator DelayedDisable()
    {
        yield return null;

        if (MainPanel.instance.hoverDetails.hasFocus)
            yield break;

        MainPanel.instance.hoverDetails.ClearDetails();
        MainPanel.instance.hoverDetails.ClearSpacer();
    }

    public void AddToDegreeTree()
    {
        ElectiveNode any = null;
        ElectiveNode subj = null;
        ElectiveNode lv = null;
        ElectiveNode both = null;

        foreach(ElectiveNode en in DegreeTree.instance.electiveNodes)
        {
            if (en.course == null)
            {
                if(en.elective.subject.Count == 1 && en.elective.level.Count == 1
                    && en.elective.subject[0] == course.course.subject && en.elective.level[0] == course.course.level)
                {
                    en.SetCourse(course.course);
                    return;
                }
                else if(both == null && en.elective.subject.Contains(course.course.subject) 
                                && en.elective.level.Contains(course.course.level))
                {
                    both = en;
                }
                else if (subj == null && en.elective.subject.Contains(course.course.subject)
                                      && !en.elective.level.Contains(course.course.level))
                {
                    subj = en;
                }
                else if(lv == null && !en.elective.subject.Contains(course.course.subject)
                                   && en.elective.level.Contains(course.course.level))
                {
                    lv = en;
                }
                else if (any == null && en.elective.subject[0] == Subject.Any && en.elective.level[0] == Level.Any)
                    any = en;
            }
        }

        if (both)
            both.SetCourse(course.course);
        else if (subj)
            both.SetCourse(course.course);
        else if (lv)
            lv.SetCourse(course.course);
        else if (any)
            any.SetCourse(course.course);
        else
            Debug.Log("No elective nodes available to add course " + course.course.name + " to degree tree");

        addToDT.gameObject.SetActive(false);
        removeFromDT.gameObject.SetActive(true);
    }

    public void RemoveFromDegreeTree()
    {
        foreach (ElectiveNode en in DegreeTree.instance.electiveNodes)
        {
            if (course.course == en.course)
            {
                en.RemoveCourse();
                return;
            }
        }
    }
}
