using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_Course", menuName = "Course")]
public class Course : ScriptableObject
{
    public int number;
    public Department department;
    //public List<string> tags;

    [TextArea]
    public string description;

    public CourseMetric mean;
    public CourseMetric hoursPerWeek;
    public CourseMetric difficulty;
}

[System.Serializable]
public enum Department
{
    CSC,
    MATH,
    SENG,
    STAT
}
