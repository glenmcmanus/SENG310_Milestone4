using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBar : MonoBehaviour
{
    public static NavBar instance;

    public Toggle home;
    public Toggle calendar;
    public Toggle courseSearch;
    public Toggle degreeTracker;
    public Toggle courseComparison;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
