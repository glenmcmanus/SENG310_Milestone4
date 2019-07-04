using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="test", menuName = "test")]
public class Test : ScriptableObject
{
    public List<Prereq> prereq;
}

