using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateState : BaseState
{
    private bool lookingAround;
    private bool finished;
    private bool calcDistLeft;

    public InvestigateState(GameObject guard) : base(guard) { }

    public override void Start()
    {
        guardBehaviour.MoveTo(guardBehaviour.PointOfInterest);
        finished = false;
        lookingAround = false;
        calcDistLeft = false;
        guardBehaviour.StartCoroutine(WaitForDistCalc());
    }

    public override void Stop()
    {
        guardBehaviour.StopMoving();
    }

    public override GuardStates RunTick()
    {
        //If player is visible
        if (calcDistLeft && guardBehaviour.Player)
        {
            guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;
            //If guard is no longer on high alert
            if (guardBehaviour.suspicionState != BaseEnemyBehaviour.SuspicionState.HighAlert)
            {
                return GuardStates.Observe;
            }
        }

        if (guardBehaviour.suspicion > 100 && guardBehaviour.Player)
        {
            return GuardStates.Chase;
        }
        

        guardBehaviour.LookAt(guardBehaviour.PointOfInterest);


        if (guardBehaviour.getDistLeft() < 0.1f && !lookingAround)
        {
            guardBehaviour.StartCoroutine(lookAround());
        }
        if (finished)
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

    private IEnumerator WaitForDistCalc()
    {

        yield return new WaitForSeconds(0.1f);
        calcDistLeft = true;
    }
}
