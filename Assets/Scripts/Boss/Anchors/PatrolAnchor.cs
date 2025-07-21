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
    Coroutine travelCoroutine;

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

        //Start travelling
        travelCoroutine = StartCoroutine(TravelToNode());
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
        if(travelCoroutine!=null)
        {
            StopCoroutine(travelCoroutine);
            travelCoroutine = null;
        }
    }

    private IEnumerator WaitForNextNode(float delay)
    {
        //Wait for delay
        yield return new WaitForSeconds(delay);

        //Increment and wrap new node
        currentNode++;
        currentNode = currentNode % patrolNodes.Length;

        //Wait over, start travel
        waitCoroutine = null;
        travelCoroutine = StartCoroutine(TravelToNode());
    }

    private IEnumerator TravelToNode()
    {
        //Get desination and distance
        Vector3 destination = patrolNodes[currentNode].position;

        //Start the travel
        spotlight.MoveTo(destination);
        yield return new WaitForSeconds(spotlight.travelTime);

        //Reached destination, wait for next node
        travelCoroutine = null;
        waitCoroutine = StartCoroutine(WaitForNextNode(patrolNodes[currentNode].delay));
    }
}
