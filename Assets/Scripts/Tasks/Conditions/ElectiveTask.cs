using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Elective Task", menuName = "Task/Elective Task")]
public class ElectiveTask : Task
{
    public List<Elective> electives;

    public override bool Condition()
    {
        int id = 0;
        int counter = 0;
        foreach(ElectiveNode en in DegreeTree.instance.electiveNodes)
        {
            bool skip = false;
            foreach (Level l in electives[id].level)
            {
                if (!en.elective.level.Contains(l))
                {
                    skip = true;
                    break;
                }
            }
            if (skip)
                continue;

            foreach(Subject s in electives[id].subject)
            {
                if(!en.elective.subject.Contains(s))
                {
                    skip = true;
                    break;
                }
            }
            if (skip)
                continue;

            if(en.course != null)
            {
                counter++;

                Debug.Log(counter);

                if (counter < electives[id].count)
                    continue;

                counter = 0;
                id++;

                Debug.Log(counter);

                if (id >= electives.Count)
                    return true;
            }
        }

        return false;
    }
}
