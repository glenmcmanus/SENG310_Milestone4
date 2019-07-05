using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddKeyword : MonoBehaviour
{
    public GameObject keyword;
    public GameObject keywordContent;
    public GameObject keywordText;

    public void addInputAsKeyword()
    {
        keyword.GetComponent<Text>().text = keywordText.GetComponent<InputField>().text;
        Instantiate(keyword, keywordContent.transform);
    }
}
