using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new survey question", menuName = "Survey Question")]
public class SurveyQuestion : ScriptableObject
{
    [TextArea]
    public string Question = "A generic question";
    public QuestionType questionType;
    public List<string> responses;
}

public enum QuestionType
{
    multipleChoice,
    shortAnswer,
    longAnswer
}
