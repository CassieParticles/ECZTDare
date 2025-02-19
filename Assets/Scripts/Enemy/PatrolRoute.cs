using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    private Vector3[] patrolNodes;
    private void Awake()
    {
        int childCount = transform.childCount;
        patrolNodes = new Vector3[childCount];
    }
}
