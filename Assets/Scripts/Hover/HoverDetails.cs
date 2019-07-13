﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class HoverDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rect;

    public float offsetScale = 200f;

    public Image image;
    public List<CourseOffering> offerings = new List<CourseOffering>();

    [Header("Content")]
    public Text courseText;
    public Text descriptionText;
    public Semester semester = Semester.Fall;
    public Text semesterText;
    public Text sectionText;
    public Text daysText;
    public Text timeText;
    public Text prereqText;
    public CourseOffering nilOffering;
    
    [Header("Content Fit")]
    public ContentSizeFitter contentSizeFitter;
    public VerticalLayoutGroup verticalLayoutGroup;

    [Header("State")]
    public Course currentCourse;
    public bool hasFocus;
    public HoverPreset preset;
    public bool disableOnExit;

    int sectionIndex;

    List<RectTransform> spacer = new List<RectTransform>();

    public void Awake()
    {
        rect = GetComponent<RectTransform>();
        semesterText.text = semester.ToString();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hasFocus = true;

        //sectionIndex = 0;

        PopulateOfferings();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasFocus = false;

        currentCourse = null;
        StartCoroutine(DelayedDisable());
    }

    IEnumerator DelayedDisable()
    {
        yield return null;

        if (currentCourse)
            yield break;

        gameObject.SetActive(!disableOnExit);
        ClearSpacer();
    }

    private void OnDestroy()
    {
        foreach (RectTransform rt in spacer)
            Debug.Log("spacer.name = " + rt.name);

        Debug.Log("HoverDetails.parent = " + rect.parent);
    }

    public void PopulateOfferings()
    {
        offerings.Clear();
        foreach (CourseOffering co in CourseSearch.instance.offerings)
        {
            if (currentCourse == co.course && semester == co.semester)
                offerings.Add(co);
        }
    }

    public void ClearSpacer()
    {
        if (spacer.Count > 0)
        {
            for (int i = spacer.Count - 1; i > -1; i--)
            {
                if (spacer[i] == rect)
                    continue;

                if (spacer[i])
                {
                    Destroy(spacer[i].gameObject);
                }
            }

            spacer.Clear();
        }
    }

    public void SetDetails()
    {
        if(offerings.Count == 0)
        {
            sectionText.text = "";
            daysText.text = "";
            timeText.text = "";
            return;
        }

        if (sectionIndex < 0)
            sectionIndex = offerings.Count - 1;
        else if(sectionIndex >= offerings.Count)
            sectionIndex = 0;

        SetDetails(offerings[sectionIndex]);
    }

    public virtual void SetDetails(Course course)
    {
        courseText.text = course.subject + " " + course.number;
        sectionText.text = "";
        daysText.text = "";
        timeText.text = "";

        if (course.prereqs.Count > 0)
        {
            prereqText.text = "";

            for (int i = 0; i < course.prereqs.Count; i++)
            {
                if (i > 0)
                    prereqText.text += ", ";

                if (course.prereqs[i].equivalent.Count > 0)
                {
                    prereqText.text += course.prereqs[i].equivalent[0].subject
                                       + " " + course.prereqs[i].equivalent[0].number;
                    for (int k = 1; k < course.prereqs[i].equivalent.Count - 1; k++)
                    {
                        prereqText.text += " or " + course.prereqs[i].equivalent[k].subject 
                                                  + " " + course.prereqs[i].equivalent[k].number;
                    }
                   
                    if(0 < course.prereqs[i].equivalent.Count - 1)
                        prereqText.text += " or " +
                                           course.prereqs[i].equivalent[course.prereqs[i].equivalent.Count - 1].subject +
                                           " " +
                                           course.prereqs[i].equivalent[course.prereqs[i].equivalent.Count - 1].number;
                }
            }
        }
        else
            prereqText.text = "None";

        descriptionText.text = course.description;
    }

    public void SetDetails(CourseOffering course)
    {
        SetDetails(course.course);
        sectionText.text = course.Section;

        if(course.days.Count > 0)
        {
            daysText.text = course.days[0].ToString();

            for (int i = 1; i < course.days.Count; i++)
            {
                daysText.text += ", ";
                daysText.text += course.days[i].ToString();
            }
        }

        timeText.text = course.startTime.x + ":" + course.startTime.y
                      + " - " + course.endTime.x + ":" + course.endTime.y;
    }

    public void SetDetails(CourseResult course)
    {
        if (hasFocus && currentCourse == course)
            return;

        int side = 0;

        ClearSpacer();

        if (Input.mousePosition.x <= Screen.width / 2)
        {
            rect.anchorMin = preset.anchorMin;
            rect.anchorMax = preset.anchorMax;
            transform.SetParent(course.column.transform, false);

            rect.anchoredPosition = Vector3.zero;
        }
        else
        {
            hasFocus = true;

            rect.anchorMin = preset.anchorMin;
            rect.anchorMax = preset.anchorMax;
            transform.SetParent(course.column.transform, false);

            rect.anchoredPosition = new Vector3(-585, 0, 0);

            side = -1;
        }

        currentCourse = course.course.course;

        for (int i = 0; i < (CourseSearch.instance.columns.Count >= 7 ? 2 : 1); i++)
        {
            spacer.Add(new GameObject().AddComponent<RectTransform>());
            spacer[i].transform.SetParent(CourseSearch.instance.columnSpace.transform, false);
            spacer[i].transform.SetSiblingIndex(course.column.transform.GetSiblingIndex() + 1 + side);
        }

        SetDetails(course.course);
    }

    public void ClearDetails()
    {
        currentCourse = null;
        gameObject.SetActive(false);
    }

    public void SetPreset(HoverPreset preset)
    {
        this.preset = preset;

        GetComponent<LayoutElement>().ignoreLayout = preset.ignoreLayout;
        disableOnExit = preset.disableOnExit;

        image.color = preset.colour;
        image.sprite = preset.sprite;
        image.type = preset.imageType;

        contentSizeFitter.horizontalFit = preset.horizontalFit;
        contentSizeFitter.verticalFit = preset.verticalFit;
        verticalLayoutGroup.spacing = preset.spacing;

        rect.anchorMin = preset.anchorMin;
        rect.anchorMax = preset.anchorMax;

        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
    }

    public void ShiftSectionIndex(int shift)
    {
        sectionIndex += shift;

        SetDetails();
    }

    public void ShiftSemester(int shift)
    {
        int index = (int)semester + shift;
        if (index < 0)
            index = (int)Semester.Fall;
        else if (index > (int)Semester.Fall)
            index = 0;

        semester = (Semester)index;

        semesterText.text = semester.ToString();

        PopulateOfferings();
        SetDetails();
    }
}
