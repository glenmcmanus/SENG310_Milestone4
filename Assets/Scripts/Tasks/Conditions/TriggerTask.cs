using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Trigger Task", menuName = "Task/Trigger Task")]
public class TriggerTask : Task
{
    public bool condition = false;

    public void OnTrigger()
    {
        condition = true;
    }

    public override bool Condition()
    {
        return condition;
    }
}
