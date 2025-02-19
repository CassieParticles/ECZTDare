using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    public struct PatrolNode
    {
        public Vector3 position;
        public float delay;
    }


    private PatrolNode[] patrolNodes;

    private readonly Dictionary<GameObject, int> guardPatrolNode = new();
    public void AddGuard(GameObject guard)
    {
        guardPatrolNode.Add(guard, 0);
    }

    //Get the node this guard is trying to reach
    public PatrolNode GetCurrNode(GameObject guard)
    {
        return patrolNodes[guardPatrolNode[guard]];
    }
    //Increment the node the guard is reaching, then get the new current
    public PatrolNode GetNextNode(GameObject guard)
    {
        //Increment the node that guard neds to reach next, wrap around if needed
        guardPatrolNode[guard] = ++guardPatrolNode[guard] % patrolNodes.Length;

        return GetCurrNode(guard);
    }

    private void Awake()
    {
        //Get patrol nodes that make up the patrol route
        int childCount = transform.childCount;
        patrolNodes = new PatrolNode[childCount];
        for (int i = 0; i < childCount; i++)
        {
            patrolNodes[i].position = transform.GetChild(i).position;
            PatrolNodeDelay delay = transform.GetChild(i).GetComponent<PatrolNodeDelay>();
            if (delay)
            {
                patrolNodes[i].delay = delay.getDelay();
            }
        }
    }
}
