using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : BaseEnemyBehaviour
{
    public enum GuardStates
    {
        Patrol,
        Observe,
        Chase
    };


    [SerializeField] private PatrolRoute patrolRoute;
    private bool recalcDelay = true;
    private bool patrolPaused = false;

    // Start is called before the first frame update
    private NavMeshAgent agent;

    GuardStates currentState;
    Vector3 pointOfInterest;    //Used for any position the guard is interested in (look at, investigate,etc)

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

    private void MoveTo(Vector3 point)
    {
        agent.SetDestination(point);
        LookAt(point);
    }

    private void LookAt(Vector3 point)
    {
        Vector3 moveDirection = point - transform.position;
        float moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, moveAngle);
    }


    private void PatrolBehaviour()
    {
        //If it has reached it's current patrol node, travel to the next one
        if (agent.remainingDistance < 0.01f && recalcDelay && !patrolPaused)
        {
            patrolPaused = true;
            StartCoroutine(pauseAtNode(patrolRoute.GetCurrNode(gameObject).delay));
        }

        suspicion -= suspicionDecayRate * Time.fixedDeltaTime;

        //Exit patrol into observing if it sees the player
        if(Player)
        {
            currentState = GuardStates.Observe;
            InterruptPatrol();
        }
    }

    private void ObserveBehaviour()
    {
        //If it loses track of the player
        if(!Player)
        {
            currentState = GuardStates.Patrol;
            ResumePatrol();
        }
        if(suspicion > 100)
        {
            currentState = GuardStates.Chase;
        }

        LookAt(pointOfInterest);

        //TODO: Make scale based on distance
        suspicion += suspicionScaleRate * Time.fixedDeltaTime;

    }
    private void ChaseBehaviour()
    {
        if(Player)
        {
            pointOfInterest = Player.transform.position;
        }
        MoveTo(pointOfInterest);
    }



    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GuardStates.Patrol:
                PatrolBehaviour();
            break;
            case GuardStates.Observe:
                ObserveBehaviour();
            break;
            case GuardStates.Chase:
                ChaseBehaviour();
            break;
        
        }



    }

    private void Awake()
    {
        Setup();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentState = GuardStates.Patrol;
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
