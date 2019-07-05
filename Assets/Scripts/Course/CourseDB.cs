using System.IO;
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
    [Header("Offerings")]
    public List<CourseOffering> cscOffering;
    public List<CourseOffering> sengOffering;
    public List<CourseOffering> mathOffering;
    public List<CourseOffering> statOffering;
    public List<CourseOffering> englOffering;
    public List<CourseOffering> engrOffering;
    public List<CourseOffering> greeOffering;
    public List<CourseOffering> mediOffering;
    public List<CourseOffering> philOffering;
    public List<CourseOffering> psychOffering;

    [Header("Courses")]
    public List<Course> cscCourse;
    public List<Course> sengCourse;
    public List<Course> mathCourse;
    public List<Course> statCourse;
    public List<Course> englCourse;
    public List<Course> engrCourse;
    public List<Course> greeCourse;
    public List<Course> mediCourse;
    public List<Course> philCourse;
    public List<Course> psychCourse;

    public void ClearDB()
    {
        cscOffering.Clear();
        sengOffering.Clear();
        mathOffering.Clear();
        statOffering.Clear();
        englOffering.Clear();
        engrOffering.Clear();
        greeOffering.Clear();
        mediOffering.Clear();
        philOffering.Clear();
        psychOffering.Clear();

        cscCourse.Clear();
        sengCourse.Clear();
        mathCourse.Clear();
        statCourse.Clear();
        englCourse.Clear();
        engrCourse.Clear();
        greeCourse.Clear();
        mediCourse.Clear();
        philCourse.Clear();
        psychCourse.Clear();
    }
}

[CustomEditor(typeof(CourseDB))]
public class CourseDB_Editor : Editor
{
    CourseDB db { get { return (CourseDB)target; } }
    string dbPath { get { string path = AssetDatabase.GetAssetPath(db);
                          return path.Substring(0, path.Length - (db.name.Length + 6)); } }

    string[] subjects = new string[] { "CSC", "SENG", "MATH", "STAT", "ENGL", "ENGR", "GREE", "MEDI", "PHIL", "PSYCH" };

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Populate DB"))
        {
            db.ClearDB();
            PopulateCourses();
            PopulateOfferings();
        }

        EditorGUILayout.Space();

        DrawDefaultInspector();

    }

    void PopulateCourses()
    {
        string path = dbPath + "Courses/";

        foreach(string s in subjects)
        {
            Debug.Log("Finding courses for subject: " + s);
            List<string> assets = GetAssetNamesAtPath(path + s + "/");
            List<Course> courses = new List<Course>();

            foreach(string a in assets)
            {
                courses.Add((Course)AssetDatabase.LoadAssetAtPath(a, typeof(Course)));
            }

            if (courses.Count == 0)
                continue;

            Debug.Log("Found " + courses.Count + " courses for subject: " + s);

            switch(courses[0].subject)
            {
                case Subject.CSC:
                    foreach(Course c in courses)
                        db.cscCourse.Add(c);
                    break;

                case Subject.MATH:
                    foreach (Course c in courses)
                        db.mathCourse.Add(c);
                    break;

                case Subject.SENG:
                    foreach (Course c in courses)
                        db.sengCourse.Add(c);
                    break;

                case Subject.STAT:
                    foreach (Course c in courses)
                        db.statCourse.Add(c);
                    break;
                case Subject.ENGL:
                    foreach (Course c in courses)
                        db.englCourse.Add(c);
                    break;
                case Subject.ENGR:
                    foreach (Course c in courses)
                        db.engrCourse.Add(c);
                    break;
                case Subject.GREE:
                    foreach (Course c in courses)
                        db.greeCourse.Add(c);
                    break;
                case Subject.MEDI:
                    foreach (Course c in courses)
                        db.mediCourse.Add(c);
                    break;
                case Subject.PHIL:
                    foreach (Course c in courses)
                        db.philCourse.Add(c);
                    break;
                case Subject.PSYCH:
                    foreach (Course c in courses)
                        db.psychCourse.Add(c);
                    break;
            }
        }
    }

    void PopulateOfferings()
    {
        string path = dbPath + "CourseOfferings/";

        List<string> assets = GetAssetNamesAtPath(path);
        List<CourseOffering> offerings = new List<CourseOffering>();

        foreach (string s in assets)
        {
            offerings.Add((CourseOffering)AssetDatabase.LoadAssetAtPath(s, typeof(CourseOffering)));
        }

        Debug.Log(offerings.Count);

        foreach (CourseOffering co in offerings)
        {
            if (co == null)
                Debug.Log("Course offering is null!");
            else if (co.course == null)
            {
                Debug.Log(co.name + " is missing a course attribute");

                foreach(string s in subjects)
                {
                    if(co.name.Contains(s))
                    {
                        Debug.Log(co.name + " subject is " + s);

                        int num = -1;
                        int.TryParse(co.name.Substring(s.Length, 3), out num);

                        if(num == -1)
                        {
                            Debug.Log("Couldn't resolve course reference for " + co.name);
                        }

                        Debug.Log(co.name + " num is " + num);

                        List<Course> cl = CourseListBySubject(s);

                        foreach(Course c in cl)
                        {
                            if(c.number == num)
                            {
                                Debug.Log(co.name + "  course resolved: " + c.name);
                                co.course = c;
                                break;
                            }
                        }

                        break;
                    }
                }
            }
        }

        foreach (CourseOffering co in offerings)
        {
            if (co.course == null)
            {
                Debug.Log("Skipping " + co.name + ", couldn't resolve course attribute");
                continue;
            }

            switch (co.course.subject)
            {
                case Subject.CSC:
                    db.cscOffering.Add(co);
                    break;

                case Subject.MATH:
                    db.mathOffering.Add(co);
                    break;

                case Subject.SENG:
                    db.sengOffering.Add(co);
                    break;

                case Subject.STAT:
                    db.statOffering.Add(co);
                    break;
                case Subject.ENGL:
                    db.englOffering.Add(co);
                    break;
                case Subject.ENGR:
                    db.engrOffering.Add(co);
                    break;
                case Subject.GREE:
                    db.greeOffering.Add(co);
                    break;
                case Subject.MEDI:
                    db.mediOffering.Add(co);
                    break;
                case Subject.PHIL:
                    db.philOffering.Add(co);
                    break;
                case Subject.PSYCH:
                    db.psychOffering.Add(co);
                    break;
            }
        }
    }

    List<string> GetAssetNamesAtPath(string path)
    {
        string[] fileEntries = Directory.GetFiles(Application.dataPath.Substring(
                                                                     0, Application.dataPath.Length - 6) + "/" + path);
        List<string> assets = new List<string>();
        foreach(string s in fileEntries)
        {
            if (s.Contains("asset.meta"))
                continue;

            assets.Add(s.Substring(Application.dataPath.Length - 5));
        }

        return assets;
    }

    List<Course> CourseListBySubject(string s)
    {
        switch(s)
        {
            case "CSC":
                return db.cscCourse;

            case "MATH":
                return db.mathCourse;

            case "SENG":
                return db.sengCourse;

            case "STAT":
                return db.statCourse;

            case "ENGL":
                return db.englCourse;

            case "ENGR":
                return db.engrCourse;

            case "GREE":
                return db.greeCourse;

            case "MEDI":
                return db.mediCourse;

            case "PHIL":
                return db.philCourse;

            case "PSYCH":
                return db.psychCourse;

            default:
                Debug.Log("Subject couldn't be resolved for course list from string; returning default: csc");
                return db.cscCourse;
        }
    }
}