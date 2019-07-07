using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MessageBox : MonoBehaviour
{
    public Text title;
    public Text message;

    public Button yesButton;
    public Button cancelButton;

    public void SetText(string title, string message)
    {
        this.title.text = title;
        this.message.text = message;
    }

    public void HideBox()
    {
        gameObject.SetActive(false);
    }
}
