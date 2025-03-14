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
    }

    public override void Stop()
    {
        guardBehaviour.StopMoving();
    }

    public override GuardStates RunTick()
    {
        if (patrolRoute)
        {
            if (guardBehaviour.getDistLeft() < 0.1f && recalcDelay && !paused)
            {
                guardBehaviour.StartCoroutine(PauseAtNode(patrolRoute.GetCurrNode(guardAttached).delay));
            }
        }
        else
        {
            if (guardBehaviour.getDistLeft() < 0.1f)
            {
                guardBehaviour.Look(StartRotation);
            }
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
}
