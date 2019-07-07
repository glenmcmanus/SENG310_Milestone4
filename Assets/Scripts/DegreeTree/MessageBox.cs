using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public string titleContent;
    public string messageContent;

    public GameObject title;
    public GameObject message;

    public GameObject yesButton;
    public GameObject cancelButton;

    private void Awake()
    {
        title.GetComponent<Text>().text = titleContent;
        message.GetComponent<Text>().text = messageContent;
    }
}
