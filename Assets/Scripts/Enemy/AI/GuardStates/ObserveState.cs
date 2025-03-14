using System.Collections;
using UnityEngine;

public class ObserveState : BaseState
{
    public ObserveState(GameObject guard) : base(guard) { }

    Coroutine observePointCoroutine;
    bool observationFinished = false;

    private IEnumerator ObservePoint()
    {
        yield return new WaitForSeconds(3);
        observationFinished = true;
    }

    public override void Start()
    {
        observationFinished = false;
    }

    public override void Stop()
    {
        if (observePointCoroutine != null)
        {
            guardBehaviour.StopCoroutine(observePointCoroutine);
            observePointCoroutine = null;
        }
    }

    public override GuardStates RunTick()
    {
        //If the guard can see the player, the player is the point of interest now
        if (guardBehaviour.Player)
        {
            //End the guard observing coroutine if it sees the player again
            if (observePointCoroutine != null)
            {
                guardBehaviour.StopCoroutine(observePointCoroutine);
                observePointCoroutine = null;
            }
            guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;
            if (guardBehaviour.suspicionState == BaseEnemyBehaviour.SuspicionState.HighAlert)
            {
                return GuardStates.Investigate;
            }
        }
        //If it cannot see the player, start a 3 second countdown, if it reaches this limit, move to patrol 
        else if (observePointCoroutine == null)
        {
            observePointCoroutine = guardBehaviour.StartCoroutine(ObservePoint());
        }

        if (observationFinished)
        {
            return GuardStates.Patrol;
        }


        if (guardBehaviour.suspicion > guardBehaviour.SuspicionLevel[3])
        {
            return GuardStates.Chase;
        }

        guardBehaviour.LookAt(guardBehaviour.PointOfInterest);



        return GuardStates.Observe;
    }
}
