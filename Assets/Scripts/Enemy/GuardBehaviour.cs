using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        Vector3 nextNode = patrolRoute.GetCurrNode(guardAttached).position;
        guardBehaviour.MoveTo(nextNode);
    }

    public override void Stop()
    {
        guardBehaviour.StopMoving();
    }

    public override GuardStates RunTick()
    {
        if(guardBehaviour.getDistLeft() < 0.1f && recalcDelay && !paused)
        {
            guardBehaviour.StartCoroutine(PauseAtNode(patrolRoute.GetNextNode(guardAttached).delay));
        }

        if(guardBehaviour.suspicion > guardBehaviour.minimumSuspicion)
        {
            guardBehaviour.suspicion -= guardBehaviour.suspicionDecayRate * Time.fixedDeltaTime;
        }

        //Has seen player, switch to observing them
        if(guardBehaviour.Player!=null)
        {
            return GuardStates.ObservePlayer;
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
        guardBehaviour.MoveTo(patrolRoute.GetCurrNode(guardAttached).position);
        recalcDelay = false;
        yield return new WaitForSeconds(0.1f);
        recalcDelay = true;
    }
}

public class ObservePlayerState : BaseState
{
    public ObservePlayerState(GameObject guard) : base(guard)
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
        if(!guardBehaviour.Player)
        {
            return GuardStates.Patrol;
        }

        if(guardBehaviour.suspicion > 100)
        {
            return GuardStates.Chase;
        }

        guardBehaviour.LookAt(guardBehaviour.Player.transform.position);

        guardBehaviour.suspicion += calcSuspiconIncreaseRate();

        return GuardStates.ObservePlayer;
    }

    private float calcSuspiconIncreaseRate()
    {
        Vector3 playerPos = guardBehaviour.Player.transform.position;
        Vector3 enemyPos = guardAttached.transform.position;

        //Get a scalar from 1 to 0 based for player's distance affecting scale rate
        float distance = (playerPos - enemyPos).magnitude;
        float visionConeLength = guardBehaviour.visionCone.distance;
        float distScalar = 1 - distance / visionConeLength;

        return distScalar * guardBehaviour.suspicionScaleRate * Time.fixedDeltaTime;
    }
}

public class ChaseState : BaseState
{
    public ChaseState(GameObject guard) : base(guard)
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
        if(!guardBehaviour.Player)
        {
            return GuardStates.Patrol;
        }

        guardBehaviour.MoveTo(guardBehaviour.Player.transform.position);
        return GuardStates.Chase;
    }
}

public class GuardBehaviour : BaseEnemyBehaviour
{
    [SerializeField] private PatrolRoute patrolRoute;

    // Start is called before the first frame update
    private NavMeshAgent agent;
    private StateMachine guardBehaviour = new StateMachine();

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

    private void Awake()
    {
        Setup();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        guardBehaviour.AddState(GuardStates.Patrol,new PatrolState(gameObject,patrolRoute));
        guardBehaviour.AddState(GuardStates.ObservePlayer,new ObservePlayerState(gameObject));
        guardBehaviour.AddState(GuardStates.Chase, new ChaseState(gameObject));
    }

    void Start()
    {
        patrolRoute.AddGuard(gameObject);
        guardBehaviour.Start(GuardStates.Patrol);
    }

    void Update()
    {
        guardBehaviour.BehaviourTick();
        //Put guard on "high alert" (won't go lower)
        if(suspicion > 80)
        {
            minimumSuspicion = 80;
        }
    }
}
