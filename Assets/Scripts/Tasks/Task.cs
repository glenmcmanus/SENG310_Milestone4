using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : ScriptableObject, TaskCondition
{
    [TextArea]
    public string instructions;

    public virtual bool Condition()
    {
        return false;
    }
}
