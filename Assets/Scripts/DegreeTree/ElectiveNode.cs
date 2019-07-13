using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectiveNode : MonoBehaviour
{
    public RectTransform rect;
    public Elective elective;
    public CourseNode course;

    [Header("Unassigned")]
    public RectTransform unassignedContainer;
    public Text subject;
    public Text level;

    [Header("Assigned")]
    public RectTransform assignedContainer;
    public Button removeCourseButton;

    public void FindElective()
    {
        //set search criteria from elective, and search
        SidePanel.instance.SetSubjects(elective.subject);
        SidePanel.instance.SetLevels(elective.level);
        SidePanel.instance.ClearKeywords();

        NavBar.instance.courseSearch.isOn = true;
        CourseSearch.instance.FindCourses();
    }

    public void SetCriteria(Elective e)
    {
        elective = e;

        if(e.subject[0] == Subject.Any)
            subject.text = "Any subject";
        else
            subject.text = e.subject[0].ToString();

        for (int i = 1; i < e.subject.Count - 1; i++)
            subject.text += ", " + e.subject[i].ToString();

        if (e.level[0] == Level.Any)
            level.text = "Any level";
        else
            level.text = e.level[0].ToString();

        for (int i = 1; i < e.level.Count - 1; i++)
            level.text += ", " + e.level[i].ToString();
    }

    public void SetCourse(Course c)
    {
        course = Instantiate(DegreeTree.instance.courseNode);
        course.SetCourse(c);

        DegreeTree.instance.currentCourses.Add(c);

        course.transform.SetParent(assignedContainer, false);
        removeCourseButton.interactable = true;
    }

    public void RemoveCourse()
    {
        List<Course> retain = new List<Course>();
        foreach(Prereq prereq in course.course.prereqs)
        {
            foreach(Course p in prereq.equivalent)
            {
                bool skip = false;
                foreach(Course c in DegreeTree.instance.currentCourses)
                {
                    foreach(Prereq cp in c.prereqs)
                    {
                        foreach(Course cpe in cp.equivalent)
                        {
                            //if a course that's not this one in current courses shares prereq 'p', retain p
                            if(c != course.course && cpe == p)
                            {
                                skip = true;
                                retain.Add(p);
                                break;
                            }
                        }

                        if (skip)
                            break;
                    }

                    if (skip)
                        break;
                }
            }
        }

        //TODO: make this take into account other electives and 'oneOfs'
        foreach (Prereq prereq in course.course.prereqs)
        {
            foreach (Course p in prereq.equivalent)
            {
                if (retain.Contains(p))
                    continue;

                //bool removed = false;
                for(int i = DegreeTree.instance.nodes.Count - 1; i >= 0; i--)
                {
                    if (DegreeTree.instance.nodes[i].course == p)
                    {
                        Destroy(DegreeTree.instance.nodes[i].gameObject);
                        DegreeTree.instance.nodes.RemoveAt(i);
                        //removed = true;
                    }
                }

                //if remove == false
                    //loop through electives and 'oneOfs' to look for p
            }
        }

        //remove node from list in DegreeTree

        DegreeTree.instance.currentCourses.Remove(course.course);
        Destroy(course.gameObject);
        removeCourseButton.interactable = false;
    }
}

[System.Serializable]
public class Elective
{
    public List<Subject> subject;
    public List<Level> level;
    public int count;
}
