using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Survey : MonoBehaviour
{
    public int id = 0;

    public List<SurveyQuestion> questions = new List<SurveyQuestion>();
    public Button previousButton;
    public Button nextButton;
    public Button submitButton;
    public List<SurveyFrame> frames = new List<SurveyFrame>();

    [Header("Prefabs")]
    public SurveyFrame surveyFramePrefab;

    private void Awake()
    {
        SetQuestion(0);
    }

    private void Update()
    {
        if (id >= frames.Count || nextButton.interactable)
            return;

        switch(questions[id].questionType)
        {
            case QuestionType.multipleChoice:
                foreach (Toggle t in frames[id].toggleGroup.GetComponentsInChildren<Toggle>())
                {
                    if (t.isOn)
                    {
                        submitButton.interactable = nextButton.interactable = true;
                        return;
                    }
                }
                nextButton.interactable = false;
                break;
            case QuestionType.shortAnswer:
                if (frames[id].shortResponseField.text != "")
                    submitButton.interactable = nextButton.interactable = true;
                break;
            case QuestionType.longAnswer:
                if (frames[id].longResponseField.text != "")
                    submitButton.interactable = nextButton.interactable = true;
                break;
        }
    }

    public void Submit()
    {
        MainPanel.instance.mailLog.LogSurvey(this);
        gameObject.SetActive(false);
    }

    public void SetQuestion(int delta)
    {
        submitButton.interactable = nextButton.interactable = false;

        if (id < frames.Count)
            frames[id].gameObject.SetActive(false);

        id += delta;
        
        if(id == 0)
        {
            previousButton.gameObject.SetActive(false);
        }
        else if(id == 1)
        {
            previousButton.gameObject.SetActive(true);
        }

        if(id == questions.Count - 1)
        {
            submitButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }
        else if(id == questions.Count - 2)
        {
            submitButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }

        if (frames.Count <= id)
        {
            frames.Add(Instantiate(surveyFramePrefab));
            frames[id].transform.SetParent(transform, false);
            frames[id].SetQuestion(questions[id]);
        }
        else
            frames[id].gameObject.SetActive(true);
    }
}
