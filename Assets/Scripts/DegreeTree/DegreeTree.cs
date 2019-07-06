using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DegreeTree : MonoBehaviour
{
    public static DegreeTree instance;

    public HoverPreset hoverPreset;

    [Header("Major and courses")]
    public Major major;
    public List<Course> currentCourses;
    public List<Course> prereqs;

    [Header("Content")]
    public Transform content;
    public List<GameObject> columns;
    public List<CourseNode> nodes;
    public List<ElectiveNode> electiveNodes;
    public List<OneOfNode> oneOfNodes;

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
    public ElectiveNode electivePrefab;
    public OneOfNode oneOfPrefab;

    List<Course> pending = new List<Course>();

    bool initialized;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        SetupTree();
    }

    private void OnEnable()
    {
        if (!initialized)
            SetupTree();

        MainPanel.instance.hoverDetails.transform.SetParent(MainPanel.instance.sideRect);
        MainPanel.instance.hoverDetails.gameObject.SetActive(true);
        MainPanel.instance.hoverDetails.SetPreset(hoverPreset);
        MainPanel.instance.hoverDetails.SetDetails(major.firstYearCore[0]);
        MainPanel.instance.rect.anchorMin *= 2;
    }

    private void OnDisable()
    {
        MainPanel.instance.hoverDetails.gameObject.SetActive(false);
        MainPanel.instance.rect.anchorMin *= 0.5f;
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

        ProcessOneOfs(major.oneOf);
        ProcessElectives(major.electives);

        initialized = true;
    }

    public void ProcessElectives(List<Elective> electives)
    {
        foreach(Elective e in electives)
        {
            for(int i = 0; i < e.count; i++)
            {
                ElectiveNode eNode = Instantiate(electivePrefab);
                electiveNodes.Add(eNode);
                eNode.SetCriteria(e);

                eNode.transform.SetParent(columns[0].transform, false);
            }
        }
    }

    public void ProcessOneOfs(List<Prereq> oneOfs)
    {
        foreach(Prereq o in oneOfs)
        {
            OneOfNode oNode = Instantiate(oneOfPrefab);
            
            foreach(Course c in o.equivalent)
            {
                oNode.AddCourse(c);
            }


        }
    }

    public void ProcessCourses(List<Course> courseList)
    {
        foreach (Course c in courseList)
        {
            if (c.prereqs.Count == 0)
            {
                Debug.Log("Add " + c.name);
                AddCourseNode(c, 0);
            }
            else
            {
                int columnID = -1;
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
                            //break;
                        }

                        if (columnID == -1) //i == prereq.equivalent.Count - 1)
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
                    AddCourseNode(c, columnID);
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
                    AddCourseNode(p, 0);
                }

                //currentCourses.Add(p);
            }
        }
    }

    public void ResolvePrereqs(ElectiveNode elec)
    {
        foreach (Prereq prereq in elec.course.course.prereqs)
        {
            foreach(Course p in prereq.equivalent)
            {
                if (!currentCourses.Contains(p))
                {
                    if (p.prereqs.Count > 0)
                        ResolvePrereqs(p);
                    else
                    {
                        AddCourseNode(p, 0);
                    }

                    //currentCourses.Add(p);
                }
            }
        }
    }

    public void ResolvePrereqs(OneOfNode oneOf)
    {
    }

    public int ResolvePrereqs(Course c)
    {
        int colId = -1;
        bool resolved = true;
        foreach (Prereq prereq in c.prereqs)
        {
            for (int i = 0; i < prereq.equivalent.Count; i++)
            {
                if (!currentCourses.Contains(prereq.equivalent[i]))
                {
                    if (prereq.equivalent[i].prereqs.Count == 0)
                    {

                        AddCourseNode(prereq.equivalent[i], 0);
                    }
                    else
                    {
                        int result = ResolvePrereqs(prereq.equivalent[i]);
                        if (result < 0)
                            resolved = false;
                        else if (result > colId)
                            colId = result;
                    }
                    //currentCourses.Add(prereq.equivalent[i]);
                }
            }
        }

        if(resolved)
        {
            if (colId < 0)
                colId = 0;

            AddCourseNode(c, colId);
        }

        return colId;
    }

    void AddColumn()
    {
        GameObject tc = Instantiate(treeColumn);
        tc.transform.SetParent(content, false);
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

    void AddCourseNode(Course course, int columnID)
    {
        if(columnID >= columns.Count)
            AddColumn(columnID - columns.Count + 1);

        CourseNode cn = Instantiate(courseNode);
        cn.SetCourse(course);
        cn.rowID = columns[columnID].transform.childCount;
        cn.transform.SetParent(columns[columnID].transform, false);

        columns[columnID].GetComponent<VerticalLayoutGroup>().spacing += cn.rect.sizeDelta.y;

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

        connector.transforms = new RectTransform[] { start.rect, end.rect };
        connector.enabled = true;
    }
}
