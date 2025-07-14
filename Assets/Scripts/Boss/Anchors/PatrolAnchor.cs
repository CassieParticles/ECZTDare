using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolAnchor : BaseAnchor
{
    //Identical to structure in patrol route for guard
    public struct PatrolNodeDat
    {
        public Vector3 position;
        public float delay;
    }

    private PatrolNodeDat[] patrolNodes;
    int currentNode;

    Coroutine waitCoroutine;

    private void Awake()
    {
        //Get patrol nodes that make up the patrol route
        int childCount = transform.childCount;
        patrolNodes = new PatrolNodeDat[childCount];
        for (int i = 0; i < childCount; i++)
        {
            patrolNodes[i].position = transform.GetChild(i).position;
            PatrolNode delay = transform.GetChild(i).GetComponent<PatrolNode>();
            if (delay)
            {
                patrolNodes[i].delay = delay.getDelay();
            }
        }

    }


    public override void AddSpotlight(SpotlightMovement spotlight)
    {
        base.AddSpotlight(spotlight);

        spotlight.MoveTo(patrolNodes[0].position);
    }

    public override void RemoveSpotlight()
    {
        base.RemoveSpotlight();

        currentNode = 0;
        if(waitCoroutine!=null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }

    private IEnumerator WaitForNextNode(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentNode++;
        currentNode = currentNode % patrolNodes.Length;
        spotlight.MoveTo(patrolNodes[currentNode].position);
        waitCoroutine = null;
    }

    public void FixedUpdate()
    {
        if (!spotlight){ return; }
        Vector3 position = patrolNodes[currentNode].position;
        
        //Move to next node
        if((spotlight.transform.position-position).sqrMagnitude < 0.1f && waitCoroutine == null)
        {
            waitCoroutine = StartCoroutine(WaitForNextNode(patrolNodes[currentNode].delay));
        }
    }
}
