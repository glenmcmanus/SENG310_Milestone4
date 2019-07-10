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

    [Header("Content Scaling")]
    public float scaleUpDelta = 1.5f;
    public float scaleDownDelta = 0.5f;
    public float maxScale = 3f;
    public float minScale = .4f;

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
    public Color lineColour = Color.blue;
    public Color lineHighlight = Color.cyan;
    public Vector3 lineOffset;
    public float columnOffset;
    public float courseHeight;

    [Header("Prefabs")]
    public GameObject treeColumn;
    public GameObject treeRow;
    public CourseNode courseNode;
    public UILineRenderer linePrefab;
    public ElectiveNode electivePrefab;
    public OneOfNode oneOfPrefab;

    #region TestingVars
    List<CourseNode> basis = new List<CourseNode>();
    List<OneOfNode> oneOfBasis = new List<OneOfNode>();

    List<CourseNode> curNodes = new List<CourseNode>();
    List<OneOfNode> curOneOfs = new List<OneOfNode>();

    List<CourseNode> queue = new List<CourseNode>();
    List<OneOfNode> oneOfQueue = new List<OneOfNode>();

    //course chain is a row or column, depending on A or B version
    List<GameObject> courseChain = new List<GameObject>();
    #endregion

    List<Course> pending = new List<Course>();

    bool initialized;

    /* TODO make horizontal layout groups for each course chain -> no prereqs: unlocks -> next course -> etc..
     */ 

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
        if (!MainPanel.instance)
            return;

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

    private void Update()
    {
        if (Input.mouseScrollDelta.y < 0 && content.localScale.x > minScale)
        {
            ZoomOut();
        }
        else if (Input.mouseScrollDelta.y > 0 && content.localScale.x < maxScale)
        {
            ZoomIn();
        }
    }

    public void ZoomIn()
    {
        content.localScale = new Vector3(content.localScale.x * scaleUpDelta, content.localScale.y * scaleUpDelta, 1);
        if (content.localScale.x > maxScale)
            content.localScale = new Vector3(maxScale, maxScale, 1);
    }

    public void ZoomOut()
    {
        content.localScale = new Vector3(content.localScale.x * scaleDownDelta, content.localScale.y * scaleDownDelta, 1);
        if (content.localScale.x < minScale)
            content.localScale = new Vector3(minScale, minScale, 1);
    }

    /* Loop through all courses in major
     * Find courses that have no prereqs, make a list of them: 'basis'
     * when a course has prereqs, check if the prereq is in the list  'basis'
     * 
     * when all courses have been checked, we can add nodes / [rows / cols] for the basis first,
     * to determine if a new [row / col] needs to be added, see if basis[x].unlocks.count == basis[y].unlocks.count
     * if they aren't equal, but all of unlocks in [x] are in [y], then they should occur in the same [row / col]
     * 
     * while resolving basis, build up a list of the next courses that are unlocked
     * rinse -> repeat
     * 
     * electives occupy their own row? As they are assigned, they move into the appropriate place in the tree
     */

    #region Testing

    public void Initialize()
    {
        BuildBasis();

    }

    public void BuildBasis()
    {
        BuildBasis(major.firstYearCore);
        BuildBasis(major.secondYearCore);
        BuildBasis(major.thirdYearCore);
        BuildBasis(major.fourthYearCore);

        foreach (Prereq ooc in major.oneOf)
        {
            OneOfNode oon = Instantiate(oneOfPrefab);
            bool isBase = true;
            foreach (Course c in ooc.equivalent)
            {
                if (c.prereqs.Count > 0)
                    isBase = false;
            }

            if (isBase)
                oneOfBasis.Add(oon);
            else
                oneOfQueue.Add(oon);
        }

        //resolve unlocks [basis]
        foreach (CourseNode cn in queue)
        {
            foreach (Prereq pq in cn.course.prereqs)
            {
                foreach (Course c in pq.equivalent)
                {
                    bool _continue = false;
                    foreach (CourseNode b in basis)
                    {
                        if (c == b.course)
                        {
                            b.unlocks.Add(cn);
                            _continue = true;
                            break;
                        }
                    }
                    if (_continue)
                        continue;

                    foreach (OneOfNode o in oneOfBasis)
                    {
                        foreach (CourseNode ocn in o.courses)
                        {
                            if (c == ocn.course)
                            {
                                o.unlocks.Add(cn);
                                _continue = true;
                                break;
                            }
                        }
                        if (_continue)
                            break;
                    }
                }
            }
        }

        List<CourseNode> processed = new List<CourseNode>();
        List<TreeAxis> axis = new List<TreeAxis>();
        foreach(CourseNode bcn1 in basis)
        {
            foreach(CourseNode bcn2 in basis)
            {
                if (bcn1 == bcn2)
                    continue;

                if (processed.Contains(bcn1) && processed.Contains(bcn2))
                    continue;

                if (bcn1.unlocks.Count == bcn2.unlocks.Count)
                {
                    if (!u1IsSubsetOfU2(bcn1.unlocks, bcn2.unlocks))
                        continue;
                }
                else if(bcn1.unlocks.Count < bcn2.unlocks.Count)
                {
                    if (!u1IsSubsetOfU2(bcn1.unlocks, bcn2.unlocks))
                        continue;
                }
                else if(bcn1.unlocks.Count > bcn2.unlocks.Count)
                {
                    if (!u1IsSubsetOfU2(bcn2.unlocks, bcn1.unlocks))
                        continue;
                }

                if(axis.Count == 0)
                {
                    axis.Add(new TreeAxis(bcn1));
                    axis[0].parallel.Add(bcn2);
                    processed.Add(bcn1);
                    processed.Add(bcn2);
                    continue;
                }

                foreach(TreeAxis ta in axis)
                {
                    if(!processed.Contains(bcn2) && ta.parallel.Contains(bcn1))
                    {
                        ta.parallel.Add(bcn2);
                        processed.Add(bcn2);
                        break;
                    }
                    else if(!processed.Contains(bcn1) && ta.parallel.Contains(bcn2))
                    {
                        ta.parallel.Add(bcn1);
                        processed.Add(bcn1);
                        break;
                    }
                }
            }
        }

        foreach(TreeAxis ta in axis)
        {
            GameObject courseChain = Instantiate(treeRow);
            courseChain.transform.SetParent(content, false);

            GameObject chainLink = Instantiate(treeColumn);
            chainLink.transform.SetParent(courseChain.transform, false);

            foreach(CourseNode cn in ta.parallel)
            {
                cn.transform.SetParent(chainLink.transform, false);

                List<CourseNode> nextLinks = AddChainLink(cn);
                for(int i = nextLinks.Count - 1; i >= 0; i--)
                {
                    List<CourseNode> next_next = AddChainLink(nextLinks[i]);
                    nextLinks.RemoveAt(i);

                    foreach (CourseNode next_cn in next_next)
                        nextLinks.Add(next_cn);

                    if (nextLinks.Count > 0 && i == 0)
                        i = nextLinks.Count - 1;
                }
            }
        }
    }

    bool u1IsSubsetOfU2(List<CourseNode> u1, List<CourseNode> u2)
    {
        if(u1.Count > u2.Count)
        {
            Debug.Log("u1 is > u2!!!");
            return false;
        }

        foreach (CourseNode u in u1)
        {
            if (!u2.Contains(u))
            {
                return false;
            }
        }

        return true;
    }

    void BuildBasis(List<Course> set)
    {
        foreach (Course c in set)
        {
            CourseNode cn = Instantiate(courseNode);
            cn.SetCourse(c);

            if (c.prereqs.Count > 0)
                queue.Add(cn);
            else
                basis.Add(cn);
        }
    }

    void ResolveUnlocks()
    {
        foreach(CourseNode cn in queue)
        {
            foreach(Prereq pq in cn.course.prereqs)
            {
                foreach(Course c in pq.equivalent)
                {
                    bool _continue = false;
                    foreach (CourseNode b in curNodes)
                    {
                        if(c == b.course && !b.unlocks.Contains(cn))
                        {
                            b.unlocks.Add(cn);
                            _continue = true;
                            break;
                        }
                    }
                    if (_continue)
                        continue;

                    foreach(OneOfNode o in curOneOfs)
                    {
                        foreach(CourseNode ocn in o.courses)
                        {
                            if(c == ocn.course && !o.unlocks.Contains(cn))
                            {
                                o.unlocks.Add(cn);
                                _continue = true;
                                break;
                            }
                        }
                        if (_continue)
                            break;
                    }
                }
            }
        }
    }

    List<CourseNode> AddChainLink(CourseNode cn)
    {
        ResolveUnlocks();

        List<CourseNode> next_next = new List<CourseNode>();
        foreach(CourseNode next in cn.unlocks)
        {
            if (curNodes.Contains(next))
                continue;

            int parentIndex = 0;
            bool pqSatisfied = false;
            foreach (Prereq pq in next.course.prereqs)
            {
                foreach(Course c in pq.equivalent)
                {
                    pqSatisfied = false;
                    foreach(CourseNode cur in curNodes)
                    {
                        if (c == cur.course)
                        {
                            if (cur.transform.parent.GetSiblingIndex() + 1 > parentIndex)
                                parentIndex = cur.transform.parent.GetSiblingIndex() + 1;

                            pqSatisfied = true;
                            break;
                        }
                    }
                    if (!pqSatisfied)
                        break;

                }
                if (!pqSatisfied)
                    break;
            }

            //check if cn.transform.parent.childCount > cn.transform.parent.siblingNum
            while(cn.transform.parent.childCount <= parentIndex)
            {
                GameObject chainLink = Instantiate(treeColumn);
                chainLink.transform.SetParent(cn.transform.parent.parent, false);
            }

            next.transform.SetParent(cn.transform.parent.parent.GetChild(parentIndex), false);
            queue.Remove(next);
            curNodes.Add(next);

            foreach (CourseNode nn in next.unlocks)
                next_next.Add(nn);
        }

        return next_next;
    }

    #endregion

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

        ProcessOneOfs(major.oneOf);

        ProcessCourses(major.firstYearCore);
        ProcessCourses(major.secondYearCore);
        ProcessCourses(major.thirdYearCore);
        ProcessCourses(major.fourthYearCore);

        ProcessElectives(major.electives);

        int prevCount;
        do
        {
            prevCount = pending.Count;
            List<Course> removeMe = new List<Course>();
            foreach (Course c in pending)
            {
                int col = ResolvePrereqs(c);
                if (col >= 0)
                    AddCourseNode(c, col);

                removeMe.Add(c);
            }

            foreach (Course c in removeMe)
            {
                pending.Remove(c);
            }

            removeMe.Clear();
        }
        while (pending.Count != prevCount && pending.Count > 0);

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

                //columns[0].GetComponent<VerticalLayoutGroup>().spacing += eNode.rect.sizeDelta.y * 1.5f;
            }
        }
    }

    public void ProcessOneOfs(List<Prereq> oneOfs)
    {
        foreach(Prereq o in oneOfs)
        {
            OneOfNode oNode = Instantiate(oneOfPrefab);

            List<CourseNode> prereqNodes = new List<CourseNode>();

            int colID = 0;
            foreach (Course c in o.equivalent)
            {
                currentCourses.Add(c);
                oNode.AddCourse(c);

                int pId = ResolvePrereqs(c);
                if (pId + 1 > colID)
                    colID = pId + 1;

                foreach(CourseNode cn in nodes)
                {
                    foreach(Prereq cp in c.prereqs)
                    {
                        foreach(Course cpc in cp.equivalent)
                        {
                            bool skip = false;
                            foreach(CourseNode checkNode in prereqNodes)
                            {
                                foreach(CourseNode checkEq in checkNode.equivalent)
                                {
                                    if (checkEq.course == cpc)
                                    {
                                        skip = true;
                                        break;
                                    }
                                }
                                if (skip)
                                    break;
                            }
                            if (skip)
                                continue;

                            if (cpc == cn.course && !prereqNodes.Contains(cn))
                            {
                                if (cn.columnID + 1 > colID)
                                    colID = cn.columnID + 1;

                                prereqNodes.Add(cn);
                            }
                        }
                    }
                }
            }

            if (colID >= columns.Count)
                AddColumn(colID - columns.Count + 1);

            oNode.rect.SetParent(columns[colID].transform, false);
            foreach (CourseNode cn in oNode.courses)
            {
                nodes.Add(cn);
                
                foreach(CourseNode ocn in oNode.courses)
                {
                    if(ocn != cn)
                    {
                        ocn.equivalent.Add(cn);
                    }
                }
            }

            foreach (CourseNode cn in prereqNodes)
            {
                if(cn.equivalent.Count > 0)
                   oNode.backEdge.Add(DrawLine(oNode.rect, cn.rect.parent.GetComponent<RectTransform>()));
                else
                    oNode.backEdge.Add(DrawLine(oNode.rect, cn.rect));
            }

            columns[colID].GetComponent<VerticalLayoutGroup>().spacing += 32 * o.equivalent.Count;
        }
    }

    public void ProcessCourses(List<Course> courseList)
    {
        foreach (Course c in courseList)
        {
            if (currentCourses.Contains(c))
                continue;

            if (c.prereqs.Count == 0)
            {
               // Debug.Log("Add " + c.name);
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
                       // Debug.Log(prereq.equivalent[i].name + " in tree?");

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
                        }

                        if (columnID == -1)
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
                   // Debug.Log("Add " + c.name);
                    AddCourseNode(c, columnID);
                }
            }
        }
    }

    public void ResolvePrereqs(ElectiveNode elec)
    {
        int columnID = 0;
        foreach (Prereq prereq in elec.course.course.prereqs)
        {
            foreach(Course p in prereq.equivalent)
            {
                if (!currentCourses.Contains(p))
                {
                    if (p.prereqs.Count > 0)
                    {
                        int colId = ResolvePrereqs(p);
                        if (colId + 1 > columnID)
                            columnID = colId + 1;

                        AddCourseNode(p, colId);
                    }
                    else
                    {
                        AddCourseNode(p, 0);
                    }
                }
            }
        }
    }

    public int ResolvePrereqs(Course c)
    {
        int colId = -1;
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
                        if (result >= 0)
                        {
                            if (result > colId)
                                colId = result;

                            if (colId < 0)
                                colId = 0;

                            AddCourseNode(prereq.equivalent[i], colId);
                        }
                    }
                }
                else
                {
                    foreach(CourseNode cn in nodes)
                    {
                        if (cn.course == prereq.equivalent[i] && cn.columnID + 1 > colId)
                            colId = cn.columnID + 1;
                    }
                }
            }
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

        columns[columnID].GetComponent<VerticalLayoutGroup>().spacing += cn.rect.sizeDelta.y * 1.5f;

        cn.column = columns[columnID].GetComponent<RectTransform>();
        cn.columnID = columnID;

        nodes.Add(cn);
        currentCourses.Add(course);

        if (cn.course.prereqs.Count > 0)
        {
            //look for prereq and join their row

            List<Course> done = new List<Course>();
            foreach(Prereq prereq in cn.course.prereqs)
            {
                foreach(Course c in prereq.equivalent)
                {
                    bool skip = false;

                    foreach (CourseNode node in nodes)
                    {
                        foreach (CourseNode checkEq in node.equivalent)
                        {
                            if ( done.Contains( checkEq.course ) )
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (skip)
                            break;

                        if (node.course == c)
                        {
                            done.Add(c);
                            if (node.equivalent.Count > 0)
                            {
                                UILineRenderer edge = DrawLine(node.rect.parent.GetComponent<RectTransform>(), cn.rect);
                                cn.backEdge.Add(edge);
                                node.rect.parent.GetComponent<OneOfNode>().forwardEdge.Add(edge);
                            }
                            else
                            {
                                UILineRenderer edge = DrawLine(node.rect, cn.rect);
                                cn.backEdge.Add(edge);
                                node.forwardEdge.Add(edge);
                            }
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            //start a row if we are the only prereq for a course, and a row doesn't already exist for that req chain

        }
    }

    UILineRenderer DrawLine(RectTransform start, RectTransform end)
    {
        GameObject go = Instantiate(linePrefab.gameObject);
        UILineConnector connector = go.GetComponent<UILineConnector>();
        connector.canvas = content.GetComponent<RectTransform>();

        go.transform.SetParent(lineContainer.GetComponent<RectTransform>(), false);

        UILineRenderer line = go.GetComponent<UILineRenderer>();
        line.enabled = true;

        connector.transforms = new RectTransform[] { start, end };
        connector.enabled = true;

        return line;
    }
}


#region TestingClasses

public class TreeAxis
{
    public List<CourseNode> parallel = new List<CourseNode>();

    public TreeAxis(CourseNode cn)
    {
        parallel.Add(cn);
    }
}
#endregion