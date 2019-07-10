using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurveyFrame : MonoBehaviour
{
    [Header("References")]
    public Text question;
    public InputField shortResponseField;
    public InputField longResponseField;
    public ToggleGroup toggleGroup;
    public List<Toggle> choices = new List<Toggle>();

    [Header("Prefab")]
    public Toggle togglePrefab;

    public void SetQuestion(SurveyQuestion surveyQuestion)
    {
        question.text = surveyQuestion.Question;

        if (surveyQuestion.questionType == QuestionType.multipleChoice)
        {
            toggleGroup.gameObject.SetActive(true);
            foreach (string s in surveyQuestion.responses)
            {
                Toggle toggle = Instantiate(togglePrefab);
                toggle.GetComponentInChildren<Text>().text = s;
                toggle.transform.SetParent(toggleGroup.transform, false);
                toggle.group = toggleGroup;
                //toggleGroup.GetComponent<VerticalLayoutGroup>().spacing += toggle.GetComponent<RectTransform>().sizeDelta.y;
            }
        }
        else if (surveyQuestion.questionType == QuestionType.shortAnswer)
        {
            shortResponseField.gameObject.SetActive(true);
        }
        else if (surveyQuestion.questionType == QuestionType.longAnswer)
        {
            longResponseField.gameObject.SetActive(true);
        }
    }
}
