using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PatrolState : BaseState
{
    private PatrolRoute patrolRoute;

    //Recalculatign the path doesn't recalculate everything instantly,
    //to avoid issues with distance recalc delay, don't recalculate immediately
    private bool recalcDelay = true;
    private bool paused = false;
    public PatrolState(GameObject guard, PatrolRoute patrolRoute) : base(guard)
    {
        this.patrolRoute = patrolRoute;
    }

    public override void Start()
    {
        if(patrolRoute)
        {
            Vector3 nextNode = patrolRoute.GetCurrNode(guardAttached).position;
            guardBehaviour.MoveTo(nextNode);
        }
    }

    public override void Stop()
    {
        guardBehaviour.StopMoving();
    }

    public override GuardStates RunTick()
    {
        if(patrolRoute)
        {
            if (guardBehaviour.getDistLeft() < 0.1f && recalcDelay && !paused)
            {
                guardBehaviour.StartCoroutine(PauseAtNode(patrolRoute.GetCurrNode(guardAttached).delay));
            }
        }

        guardBehaviour.CalcSuspicionDecay();

        //Has seen player, switch to observing them
        if (guardBehaviour.Player!=null)
        {
            guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;
            return GuardStates.Observe;
        }

        return GuardStates.Patrol;
    }

    private IEnumerator PauseAtNode(float pauseTime)
    {
        paused = true;
        yield return new WaitForSeconds(pauseTime);
        paused = false;
        guardBehaviour.StartCoroutine(RecalculatePath());
    }
    private IEnumerator RecalculatePath()
    {
        guardBehaviour.MoveTo(patrolRoute.GetNextNode(guardAttached).position);
        recalcDelay = false;
        yield return new WaitForSeconds(0.1f);
        recalcDelay = true;
    }
}

public class HeardNoiseState : BaseState
{
    public HeardNoiseState(GameObject guard) : base(guard)
    {
    }



    public override void Start()
    {
        
    }

    public override void Stop()
    {
        
    }

    public override GuardStates RunTick()
    {
        if(guardBehaviour.suspicionState == BaseEnemyBehaviour.SuspicionState.Chase)
        {
            return GuardStates.Chase;
        }
        else if(guardBehaviour.suspicionState == BaseEnemyBehaviour.SuspicionState.HighAlert)
        {
            return GuardStates.Investigate; //Enemy is on edge, investigate
        }
        else
        {
            return GuardStates.Observe;
        }
    }
}

public class ObserveState : BaseState
{
    public ObserveState(GameObject guard) : base(guard){}

    public override void Start(){}

    public override void Stop(){}

    public override GuardStates RunTick()
    {
        if(!guardBehaviour.Player)
        {
            if(guardBehaviour.suspicionState == BaseEnemyBehaviour.SuspicionState.HighAlert)
            {
                return GuardStates.Investigate;
            }
            return GuardStates.Patrol;
        }

        if(guardBehaviour.suspicion > guardBehaviour.SuspicionLevel[3])
        {
            return GuardStates.Chase;
        }

        guardBehaviour.LookAt(guardBehaviour.PointOfInterest);

        guardBehaviour.CalcSuspicionIncrease();

        guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;

        return GuardStates.Observe;
    }
}

public class InvestigateState : BaseState
{
    private bool lookingAround=false;
    private bool finished = false;

    public InvestigateState(GameObject guard) : base(guard){}

    public override void Start()
    {
        guardBehaviour.MoveTo(guardBehaviour.PointOfInterest);
    }

    public override void Stop()
    {
        guardBehaviour.StopMoving();
    }

    public override GuardStates RunTick()
    {
        //If it sees the player
        if(guardBehaviour.Player)
        {
            return GuardStates.Observe;
        }

        if(guardBehaviour.getDistLeft() < 0.1f && !lookingAround)
        {
            guardBehaviour.StartCoroutine(lookAround());
        }
        if(finished)
        {
            return GuardStates.Patrol;
        }

        return GuardStates.Investigate;
    }

    private IEnumerator lookAround()
    {
        lookingAround = true;
        yield return new WaitForSeconds(3);
        finished = true;
    }
}


public class ChaseState : BaseState
{
    public ChaseState(GameObject guard) : base(guard){}

    public override void Start()
    {

    }

    public override void Stop(){}

    public override GuardStates RunTick()
    {
        if(!guardBehaviour.Player)
        {
            return GuardStates.RaiseAlarm;
        }

        guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;

        guardBehaviour.MoveTo(guardBehaviour.Player.transform.position);
        return GuardStates.Chase;
    }
}

public class RaiseAlarmState : BaseState
{
    private AlarmSystem alarm;
    public RaiseAlarmState(GameObject guard,AlarmSystem alarm) : base(guard)
    {
        this.alarm = alarm;
    }

    public override void Start()
    {
        guardBehaviour.StopMoving();
        if(alarm && !alarm.AlarmGoingOff())
        {
            //Start coroutine to set off alarm
            guardBehaviour.StartCoroutine(RaiseAlarm());
        }
    }

    public override void Stop(){}

    public override GuardStates RunTick()
    {
        if(!alarm || alarm.AlarmGoingOff())
        {
            return GuardStates.Patrol;
        }
        return GuardStates.RaiseAlarm;
    }

    private IEnumerator RaiseAlarm()
    {
        yield return new WaitForSeconds(3);
        alarm.StartAlarm(guardBehaviour.PointOfInterest);
    }
}

public class GuardBehaviour : BaseEnemyBehaviour
{
    [SerializeField] private PatrolRoute patrolRoute;

    //The speed at which footstep sounds are triggered. Whenever footstepRate is 1 a footstep is played
    [SerializeField][Range(0.01f, 3.0f)] private float footstepRate = 1f;

    //How much the velocity of the player affects the footstep frequency
    [SerializeField][Range(0.01f, 3.0f)] private float footstepRateScaler = 1f;

    //Used to determine when to trigger footstep sounds.
    private float footstepCount = 0.0f;

    public AK.Wwise.Event guardFootstep;

    // Start is called before the first frame update
    private NavMeshAgent agent;
    private StateMachine guardBehaviour = new StateMachine();

    public Vector3 PointOfInterest;

    public void MoveTo(Vector3 position)
    {
        agent.SetDestination(position);
        LookAt(position);
    }

    public void LookAt(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        visionCone.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void StopMoving()
    {
        agent.SetDestination(transform.position);
    }

    public float getDistLeft()
    {
        return agent.remainingDistance;
    }

    private void AlarmOn(Vector3 playerPosition)
    {
        SetSuspicionState(SuspicionState.HighAlert);
    }

    private void AlarmOff()
    {

    }

    private void HearNoise(Vector3 noiseLocation, float suspicionIncrease)
    {
        PointOfInterest = noiseLocation;
        suspicion += suspicionIncrease;
        if(suspicion > 100)
        {
            suspicion = 99;
        }
        guardBehaviour.MoveToState(GuardStates.HearNoise);
    }

    private void CatchPlayer()
    {
        if(suspicionState == SuspicionState.Chase)
        {
            SceneManager.LoadScene("LoseScene");
        }
    }

    private void Awake()
    {
        Setup();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        guardBehaviour.AddState(GuardStates.Patrol,new PatrolState(gameObject,patrolRoute));
        guardBehaviour.AddState(GuardStates.HearNoise,new HeardNoiseState(gameObject));
        guardBehaviour.AddState(GuardStates.Observe,new ObserveState(gameObject));
        guardBehaviour.AddState(GuardStates.Investigate,new InvestigateState(gameObject));
        guardBehaviour.AddState(GuardStates.Chase, new ChaseState(gameObject));
        guardBehaviour.AddState(GuardStates.RaiseAlarm, new RaiseAlarmState(gameObject, alarm));

        
    }



    void Start()
    {
        if (patrolRoute){ patrolRoute.AddGuard(gameObject); }
        
        guardBehaviour.Start(GuardStates.Patrol);

        if(alarm)
        {
            alarm.AddAlarmEnableFunc(AlarmOn);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }

        AudioDetectionSystem.getAudioSystem().AddListener(gameObject, HearNoise);
    }

    void FixedUpdate()
    {
        BaseUpdate();

        guardBehaviour.BehaviourTick();

        if (agent.velocity != Vector3.zero)
        {
            //Footstep sound effect
            if (Mathf.Abs(agent.velocity.x) > 0.1)
            {
                footstepCount += (Mathf.Abs(agent.velocity.x) * footstepRateScaler) * footstepRate * Time.deltaTime;
                if (footstepCount > 1)
                {
                    guardFootstep.Post(gameObject);
                    footstepCount--;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CatchPlayer();
        }
    }

}
