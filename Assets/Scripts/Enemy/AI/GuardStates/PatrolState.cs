using System.Collections;
using UnityEngine;

public class PatrolState : BaseState
{
    private PatrolRoute patrolRoute;
    private Vector3 StartPosition;
    private float StartRotation;

    //Recalculatign the path doesn't recalculate everything instantly,
    //to avoid issues with distance recalc delay, don't recalculate immediately
    private bool recalcDelay;
    private bool paused;

    Coroutine LookAroundCoroutine;
    Coroutine PauseAtNodeCoroutine;
    bool lookAround;


    public PatrolState(GameObject guard, PatrolRoute patrolRoute) : base(guard)
    {
        this.patrolRoute = patrolRoute;
        this.StartPosition = Vector3.zero;
        this.StartRotation = 0;
    }

    public PatrolState(GameObject guard, Vector3 StartPosition, float StartRotation) : base(guard)
    {
        this.patrolRoute = null;
        this.StartPosition = StartPosition;
        this.StartRotation = StartRotation;
    }

    public override void Start()
    {
        if (patrolRoute)
        {
            Vector3 nextNode = patrolRoute.GetCurrNode(guardAttached).position;
            guardBehaviour.MoveTo(nextNode);
        }
        else
        {
            guardBehaviour.MoveTo(StartPosition);
        }
        recalcDelay = true;
        paused = false;
        lookAround = false;
    }

    public override void Stop()
    {
        guardBehaviour.StopMoving();
        if(LookAroundCoroutine!=null)
        {
            guardBehaviour.StopCoroutine(LookAroundCoroutine);
            LookAroundCoroutine=null;
        }
        if(PauseAtNodeCoroutine!=null)
        {
            guardBehaviour.StopCoroutine(PauseAtNodeCoroutine);
            PauseAtNodeCoroutine = null;
        }

    }

    public override GuardStates RunTick()
    {
        if (patrolRoute)
        {
            if (guardBehaviour.getDistLeft() < 0.1f && recalcDelay && !paused)
            {
                PauseAtNodeCoroutine = guardBehaviour.StartCoroutine(PauseAtNode(patrolRoute.GetCurrNode(guardAttached).delay));
            }
        }
        else
        {
            if (guardBehaviour.getDistLeft() < 0.1f)
            {
                guardBehaviour.Look(StartRotation);
            }
        }

        //If guard is high alert, wait a period of time before looking around
        if(guardBehaviour.suspicionState==BaseEnemyBehaviour.SuspicionState.HighAlert)
        {
            guardBehaviour.StartCoroutine(LookAround(Random.Range(10,20)));
        }
        else
        {   //If guard is no longe rhigh alert, stop coroutine
            if(LookAroundCoroutine!=null)
            {
                guardBehaviour.StopCoroutine(LookAroundCoroutine);
                LookAroundCoroutine = null;
            }
        }

        //Look around
        if(lookAround)
        {
            return GuardStates.LookAround;
        }

        guardBehaviour.CalcSuspicionDecay();

        //Has seen player, switch to observing them
        if (guardBehaviour.Player != null)
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

    private IEnumerator LookAround(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        lookAround = true;
    }
}
