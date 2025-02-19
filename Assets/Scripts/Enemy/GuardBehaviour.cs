using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject target;
    // Start is called before the first frame update
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.transform.position);

        float distRemain = agent.remainingDistance;

        //Get shortest distance between points, to compare to distance on path, if shortest distance is shorter then distance to destination, player is not reacahble
        float sqrShortestDistance = ((Vector2)target.transform.position - (Vector2)transform.position).sqrMagnitude;

        if(distRemain != Mathf.Infinity && sqrShortestDistance - distRemain * distRemain  > 0.1f)
        {
            Debug.Log("Cannot be reached");
        }
        else 
        {
            //Debug.Log("Can be reached");
        }
    }
}
