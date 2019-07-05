using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Course lists organized by subject
/// </summary>
[CreateAssetMenu(fileName = "CourseDB", menuName = "Course Database")]
public class CourseDB : ScriptableObject
{
    public List<CourseOffering> csc;
    public List<CourseOffering> seng;
    public List<CourseOffering> math;
    public List<CourseOffering> stat;
    public List<CourseOffering> engl;
    public List<CourseOffering> engr;
    public List<CourseOffering> gree;
    public List<CourseOffering> medi;
    public List<CourseOffering> phil;
    public List<CourseOffering> psych;
}

[CustomEditor(typeof(CourseDB))]
[ExecuteInEditMode]
public class CourseDB_Editor : Editor
{
    CourseDB db { get { return (CourseDB)target; } }

    public override void OnInspectorGUI()
    {
        //do your stuff here
        DrawDefaultInspector();

        if (GUILayout.Button("Click me"))
        {
            Debug.Log("Hello world.");
        }
    }
}