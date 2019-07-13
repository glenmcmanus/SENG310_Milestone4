using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneOfToggle : MonoBehaviour
{
    public void OnValueChange()
    {
        if(GetComponent<Toggle>().isOn)
            MainPanel.instance.mailLog.LogClick(gameObject);
    }
}
