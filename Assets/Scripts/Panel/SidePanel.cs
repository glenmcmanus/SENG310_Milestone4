using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : Foldout
{
    public static SidePanel instance;

    [Header("Side Panel")]
    public RectTransform subjectContainer;
    public RectTransform levelContainer;
    public RectTransform keywordContainer;

    public Foldout foldoutButton;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        rect.anchorMin.Set(anchorFullMin.x, anchorFullMin.y);
        rect.anchorMax.Set(anchorFullMax.x, anchorFullMax.y);
    }

    public override void Fold()
    {
        base.Fold();

        if (isFolded)
        {
            foldoutButton.GetComponentInChildren<Text>().text = ">\n>\n>";
        }
        else
        {
            foldoutButton.GetComponentInChildren<Text>().text = "<\n<\n<";
        }
    }

    public void ToggleContent(GameObject go)
    {
        if(go.activeSelf)
        {
            go.SetActive(false);
            GetComponent<VerticalLayoutGroup>().spacing -= go.GetComponent<RectTransform>().rect.height;
        }
        else
        {
            go.SetActive(true);
            GetComponent<VerticalLayoutGroup>().spacing += go.GetComponent<RectTransform>().rect.height;
        }

    }

    public void ShowHide(Text text)
    {
        if (text.text == "Show")
            text.text = "Hide";
        else
            text.text = "Show";
    }

    public void SetSubjects(List<Subject> subjects)
    {
        for(int i = 0; i < subjectContainer.childCount; i++)
        {
            if(subjects.Contains(subjectContainer.GetChild(i).GetComponent<SubjectToggle>().subject))
                subjectContainer.GetChild(i).GetComponent<Toggle>().isOn = true;
            else
                subjectContainer.GetChild(i).GetComponent<Toggle>().isOn = false;
        }
    }

    public void SetLevels(List<Level> levels)
    {
        for (int i = 0; i < levelContainer.childCount; i++)
        {
            if (levels.Contains(levelContainer.GetChild(i).GetComponent<LevelToggle>().level))
                levelContainer.GetChild(i).GetComponent<Toggle>().isOn = true;
            else
                levelContainer.GetChild(i).GetComponent<Toggle>().isOn = false;
        }
    }

    public void ClearKeywords()
    {
        for(int i = keywordContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(keywordContainer.GetChild(i));
        }
    }
}
