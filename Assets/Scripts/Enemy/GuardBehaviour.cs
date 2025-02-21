using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState
{
    private PatrolRoute patrolRoute;

    //Recalculatign the path doesn't recalculate everything instantly,
    //to avoid issues with distance recalc delay, don't recalculate immediately
    private bool recalcDelay = true;
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

    }

    public override GuardStates RunTick()
    {
        if(guardBehaviour.getDistLeft() < 0.1f && recalcDelay)
        {
            guardBehaviour.StartCoroutine(RecalculatePath());
        }

        return GuardStates.Patrol;
    }

    private IEnumerator RecalculatePath()
    {
        guardBehaviour.MoveTo(patrolRoute.GetNextNode(guardAttached).position);
        recalcDelay = false;
        yield return new WaitForSeconds(0.1f);
        recalcDelay = true;
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
    }

    void Start()
    {
        patrolRoute.AddGuard(gameObject);
        guardBehaviour.Start(GuardStates.Patrol);
    }

    void Update()
    {
        guardBehaviour.BehaviourTick();
    }
}
