using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : MonoBehaviour
{
    [SerializeField] private PatrolRoute patrolRoute;
    private bool recalcDelay = true;

    // Start is called before the first frame update
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        if(patrolRoute != null)
        {
            patrolRoute.AddGuard(gameObject);
            agent.SetDestination(patrolRoute.GetCurrNode(gameObject));
        }
        StartCoroutine(calcDelay());
    }

    private IEnumerator calcDelay()
    {
        recalcDelay = false;
        yield return new WaitForSeconds(0.1f);
        recalcDelay = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.remainingDistance < 0.01f && recalcDelay)
        {
            agent.SetDestination(patrolRoute.GetNextNode(gameObject));
            StartCoroutine(calcDelay());
        }

    }
}
