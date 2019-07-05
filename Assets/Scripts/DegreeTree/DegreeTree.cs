using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DegreeTree : MonoBehaviour
{
    [Header("Major and courses")]
    public Major major;
    public List<Course> currentCourses;
    public List<Course> prereqs;

    [Header("Content")]
    public Transform content;
    public List<GameObject> columns;
    public List<CourseNode> nodes;

    [Header("Line")]
    public Transform lineContainer;
    public float thickness = 3f;
    public Vector2Int lineResolution = new Vector2Int(32, 32);
    public Color lineColour = Color.black;
    public Vector3 lineOffset;
    public float columnOffset;
    public float courseHeight;

    [Header("Prefabs")]
    public GameObject treeColumn;
    public CourseNode courseNode;
    public UILineRenderer linePrefab;

    List<Course> pending = new List<Course>();

    bool initialized;

    private void Awake()
    {
        SetupTree();
    }

    private void OnEnable()
    {
        if (!initialized)
            SetupTree();
    }

    public void SetupTree()
    {
        for(int i = content.childCount - 1; i >= 0; i--)
        {
            if (content.GetChild(i).transform == lineContainer)
                continue;

            Destroy(content.GetChild(i).gameObject);
        }

        columns = new List<GameObject>();
        AddColumn(4);

        ProcessCourses(major.firstYearCore);
        ProcessCourses(major.secondYearCore);
        ProcessCourses(major.thirdYearCore);
        ProcessCourses(major.fourthYearCore);

        initialized = true;
    }

    public void ProcessCourses(List<Course> courseList)
    {
        foreach (Course c in courseList)
        {
            if (c.prereqs.Count == 0)
            {
                Debug.Log("Add " + c.name);
                AddNode(c, 0);
            }
            else
            {
                int columnID = 0;
                bool skip = false;
                foreach (Prereq prereq in c.prereqs)
                {
                    for (int i = 0; i < prereq.equivalent.Count; i++)
                    {
                        Debug.Log(prereq.equivalent[i].name + " in tree?");

                        if (currentCourses.Contains(prereq.equivalent[i]))
                        {
                            foreach(CourseNode cn in nodes)
                            {
                                if(cn.course == prereq.equivalent[i])
                                {
                                    if (cn.columnID + 1 > columnID)
                                        columnID = cn.columnID + 1;
                                    break;
                                }
                            }
                            break;
                        }

                        if (i == prereq.equivalent.Count - 1)
                            skip = true;
                    }
                }

                if (skip)
                {
                    Debug.Log("Skip " + c.name);
                    pending.Add(c);
                    continue;
                }
                else
                {
                    Debug.Log("Add " + c.name);
                    AddNode(c, columnID);
                }
            }
        }

        int prevCount;
        do
        {
            prevCount = pending.Count;
            List<Course> removeMe = new List<Course>();
            foreach (Course c in pending)
            {
                ResolvePrereqs();
                currentCourses.Add(c);
                removeMe.Add(c);
            }

            foreach (Course c in removeMe)
            {
                pending.Remove(c);
            }

            removeMe.Clear();
        }
        while (pending.Count != prevCount && pending.Count > 0);

        foreach (Course c in pending)
            Debug.Log(c.name + " pending");
    }

    public void ResolvePrereqs()
    {
        foreach (Course p in prereqs)
        {
            if (!currentCourses.Contains(p))
            {
                if(p.prereqs.Count > 0)
                    ResolvePrereqs(p);
                else
                {
                    AddNode(p, 0);
                }

                currentCourses.Add(p);
            }
        }
    }

    public void ResolvePrereqs(Course c)
    {
        foreach (Prereq prereq in c.prereqs)
        {
            for (int i = 0; i < prereq.equivalent.Count; i++)
            {
                if (!currentCourses.Contains(prereq.equivalent[i]))
                {
                    if (prereq.equivalent[i].prereqs.Count == 0)
                    {
                        AddNode(prereq.equivalent[i], 0);
                    }
                    else
                        ResolvePrereqs(prereq.equivalent[i]);

                    currentCourses.Add(prereq.equivalent[i]);
                }
            }
        }
    }

    void AddColumn()
    {
        GameObject tc = Instantiate(treeColumn);
        tc.transform.SetParent(content, false);
        //tc.transform.localScale = Vector3.one;
        columns.Add(tc);
        tc.transform.position = Vector3.zero;
    }

    void AddColumn(int count)
    {
        for(int i = 0; i < count; i++)
        {
            AddColumn();
        }
    }

    void AddNode(Course course, int columnID)
    {
        CourseNode cn = Instantiate(courseNode);
        cn.SetCourse(course);
        cn.rowID = columns[columnID].transform.childCount;
        cn.transform.SetParent(columns[columnID].transform, false);
        cn.column = columns[columnID].GetComponent<RectTransform>();
        cn.columnID = columnID;
        nodes.Add(cn);
        currentCourses.Add(course);

        if (cn.course.prereqs.Count > 0)
        {
            foreach(Prereq prereq in cn.course.prereqs)
            {
                foreach(Course c in prereq.equivalent)
                {
                    foreach (CourseNode node in nodes)
                    {
                        if (node.course == c)
                        {
                            DrawLine(node, cn);
                        }
                    }
                }
            }
        }
    }

    void DrawLine(CourseNode start, CourseNode end)
    {
        GameObject go = Instantiate(linePrefab.gameObject);
        UILineConnector connector = go.GetComponent<UILineConnector>();
        connector.canvas = content.GetComponent<RectTransform>();


        go.transform.SetParent(lineContainer.GetComponent<RectTransform>(), false);

        UILineRenderer line = go.GetComponent<UILineRenderer>();
        line.enabled = true;
        /*
        Debug.Log(start.rect.position);
        
        Vector3 pos = start.rect.position - lineOffset;

        Debug.Log(pos);

        go.transform.position = pos;

        Vector2[] endpoints = new Vector2[] { new Vector2(0,0), new Vector2(columnOffset * end.columnID, courseHeight * end.rowID) };
        line.Points = endpoints;*/

        connector.transforms = new RectTransform[] { start.rect, end.rect };
        connector.enabled = true;

        //line.transform.position = Vector3.zero;
    }
}
