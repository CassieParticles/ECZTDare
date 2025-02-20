using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : MonoBehaviour
{
    [SerializeField] private PatrolRoute patrolRoute;
    private bool recalcDelay = true;
    private bool patrolPaused = false;

    // Start is called before the first frame update
    private NavMeshAgent agent;



    private IEnumerator calcDelay()
    {
        recalcDelay = false;
        yield return new WaitForSeconds(0.1f);
        recalcDelay = true;
    }

    private IEnumerator pauseAtNode(float pause)
    {
        patrolPaused = true;
        yield return new WaitForSeconds(pause);
        patrolPaused = false;
        MoveTo(patrolRoute.GetNextNode(gameObject).position);
        StartCoroutine(calcDelay());
    }

    private void InterruptPatrol()
    {
        if (patrolPaused) { return; }
        MoveTo(transform.position);
        patrolPaused = true;
    }

    private void ResumePatrol()
    {
        if (!patrolPaused) { return; }
        MoveTo(patrolRoute.GetCurrNode(gameObject).position);
        StartCoroutine(calcDelay());
        patrolPaused = false;
    }

    private void MoveTo(Vector3 place)
    {
        agent.SetDestination(place);
        Vector3 moveDirection = place - transform.position;
        float moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, moveAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.remainingDistance < 0.01f && recalcDelay && !patrolPaused && false)
        {
            patrolPaused = true;
            StartCoroutine(pauseAtNode(patrolRoute.GetCurrNode(gameObject).delay));
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        if (patrolRoute != null)
        {
            patrolRoute.AddGuard(gameObject);
            MoveTo(patrolRoute.GetCurrNode(gameObject).position);
        }
        StartCoroutine(calcDelay());
    }
}
