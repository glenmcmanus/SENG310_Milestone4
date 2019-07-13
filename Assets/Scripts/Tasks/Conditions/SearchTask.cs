using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Search Task", menuName = "Task/Search Task")]
public class SearchTask : Task
{
    public Semester semester;
    public List<Subject> subjects;
    public List<Level> levels;
    public int keywords;

    public override bool Condition()
    {
        if (semester != CourseSearch.instance.semester)
            return false;

        foreach(Subject s in subjects)
        {
            if (!CourseSearch.instance.subjects.Contains(s))
                return false;
        }

        foreach(Level l in levels)
        {
            if (!CourseSearch.instance.levels.Contains(l))
                return false;
        }

        if (CourseSearch.instance.keywordContentParent.transform.childCount > keywords)
            return false;

        return true;
    }
}
