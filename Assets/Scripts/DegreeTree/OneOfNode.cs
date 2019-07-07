using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(ToggleGroup))]
public class OneOfNode : MonoBehaviour
{
    public RectTransform rect;
    public Toggle togglePrefab;
    public Vector2 toggleOffset;
    public List<CourseNode> courses = new List<CourseNode>();
    public List<UILineRenderer> connectingLine = new List<UILineRenderer>();

    public void AddCourse(Course course)
    {
        CourseNode cn = Instantiate(DegreeTree.instance.courseNode);
        cn.transform.SetParent(rect, false);

        cn.SetCourse(course);
        courses.Add(cn);

        Toggle t = Instantiate(togglePrefab);
        t.transform.SetParent(cn.transform, false);
        t.transform.position += (Vector3)toggleOffset;
        t.group = GetComponent<ToggleGroup>();
    }
}
