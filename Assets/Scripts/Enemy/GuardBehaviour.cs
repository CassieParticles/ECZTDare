using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : BaseEnemyBehaviour
{
    [SerializeField] private PatrolRoute patrolRoute;
    private bool recalcDelay = true;
    private bool patrolPaused = false;

    // Start is called before the first frame update
    private NavMeshAgent agent;

    GuardStates currentState;
    Vector3 pointOfInterest;    //Used for any position the guard is interested in (look at, investigate,etc)

    bool alarmRaiseBegin=false;

    StateMachine stateMachine = new StateMachine();


    private void InterruptPatrol()
    {
        if (patrolPaused) { return; }
        Stop();
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

    private void Stop()
    {
        agent.SetDestination(transform.position);
    }


    private void PatrolBehaviour()
    {
        //If it has reached it's current patrol node, travel to the next one
        if (agent.remainingDistance < 0.01f && recalcDelay && !patrolPaused)
        {
            patrolPaused = true;
            StartCoroutine(pauseAtNode(patrolRoute.GetCurrNode(gameObject).delay));
        }

        if(suspicion > minimumSuspicion)
        { 
            suspicion -= suspicionDecayRate * Time.fixedDeltaTime;
        }

        //Exit patrol into observing if it sees the player
        if(Player)
        {
            currentState = GuardStates.Observe;
            InterruptPatrol();
        }
    }

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
        if(!Player)
        {
            Stop();
            currentState = GuardStates.RaiseAlarm;
        }
    }

    private void RaiseAlarmBehaviour()
    {
        //If there is no alarm, immediately leave state
        if(!alarm)
        {
            currentState = GuardStates.Patrol;
            ResumePatrol();
            return;
        }
        if(!alarmRaiseBegin)
        {
            alarmRaiseBegin = true;
            StartCoroutine(RaiseAlarm());
        }
        //Once alarm goes off, resume behaviour
        if(!alarm.AlarmGoingOff())
        {
            currentState = GuardStates.Patrol;
            ResumePatrol();
        }
    }

    private IEnumerator RaiseAlarm()
    {
        yield return new WaitForSeconds(1);
        alarm.StartAlarm(pointOfInterest);
    }

    private void AlarmOn(Vector3 playerPosition)
    {
        minimumSuspicion = 90;
        suspicion = Mathf.Min(suspicion, minimumSuspicion);
        Debug.Log("Alarm on");
    }

    private void AlarmOff()
    {
        Debug.Log("Alarm off");
    }
    

    private void Awake()
    {
        Setup();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentState = GuardStates.Patrol;

        if(alarm)
        {
            alarm.AddAlarmEnableFunc(AlarmOn);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }

        stateMachine.AddState(GuardStates.Patrol, new PatrolState());
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
            case GuardStates.RaiseAlarm:
                RaiseAlarmBehaviour();

                break;
        }
    }
}
