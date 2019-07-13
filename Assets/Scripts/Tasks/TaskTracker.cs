using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTracker : MonoBehaviour
{
    public List<Task> tasks;
    public int id = 0;
    public Text instructions;
    public Image border;

    public float flashDuration = 2f;
    public float flashFrequency = .1f;
    bool flashing;

    private void Awake()
    {
        if(tasks.Count > 0)
        {
            instructions.text = tasks[0].instructions;
        }

        foreach(Task t in tasks)
        {
            if (t.GetType() == typeof(TriggerTask))
                ((TriggerTask)t).condition = false;
        }
    }

    private void Update()
    {
        //Debug.Log("Update task tracker");
        if (tasks.Count == 0 || id >= tasks.Count)
        {
            //Debug.Log("task tracker early out");
            return;
        }

        if(tasks[id].Condition() == true)
        {
            //Debug.Log(tasks[id].name + " complete!");
            NextTask();
        }
    }

    public void NextTask()
    {
        id++;

        if(id >= tasks.Count)
        {
            instructions.text = "All tasks completed!";
            instructions.color = Color.green;
            return;
        }

        instructions.text = tasks[id].instructions;
        StartCoroutine(FlashBorder());
    }

    public IEnumerator FlashBorder()
    {
        if (flashing)
            yield break;

        flashing = true;
        float endTime = Time.time + flashDuration;
        while(flashing && Time.time < endTime)
        {
            border.enabled = false;
            yield return new WaitForSeconds(flashFrequency);
            border.enabled = true;
            yield return new WaitForSeconds(flashFrequency);
        }

        border.enabled = true;

        flashing = false;
    }
}
