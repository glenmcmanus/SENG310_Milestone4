using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeColumn : MonoBehaviour
{
    public List<CourseNode> nodes;

    public void AddNode(CourseNode node)
    {
        nodes.Add(node);

    }
}
