using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtoB : MonoBehaviour
{
    public GameObject verticalDt;
    public GameObject horizontalDt;

    public Toggle navToggle;
    public Button dtHomeButton;

    public void Initialize()
    {
        switch(Random.Range(0, 2))
        {
            case 0:
                break;

            case 1:
                break;
        }
    }

    public void SetDt(GameObject dt)
    {
        //change navToggle event
        //dtHomeButton.onClick.AddListener();
    }
}
