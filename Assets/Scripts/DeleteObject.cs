using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public GameObject toDelete;

    public void DestroyKeyord()
    {
        Destroy(toDelete);
    }
}
