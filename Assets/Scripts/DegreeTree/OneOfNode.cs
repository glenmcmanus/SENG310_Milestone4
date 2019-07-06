using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class OneOfNode : MonoBehaviour
{
    public RectTransform rect;
    public Vector2 toggleOffset;
    List<CourseNode> courses = new List<CourseNode>();

    public void AddCourse(Course course)
    {
        CourseNode cn = Instantiate(DegreeTree.instance.courseNode);
        cn.transform.SetParent(rect, false);

        cn.SetCourse(course);
        courses.Add(cn);

        Toggle t = Instantiate(new GameObject()).AddComponent<Toggle>();
        t.transform.SetParent(cn.transform, false);
        t.transform.position += (Vector3)toggleOffset;
        t.group = GetComponent<ToggleGroup>();
    }
}
