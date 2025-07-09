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
            if (guardBehaviour.suspicionState < BaseEnemyBehaviour.SuspicionState.HighAlert)
            {
                return GuardStates.Observe;
            }
        }

        if (guardBehaviour.suspicion > 100 && guardBehaviour.Player)
        {
            return GuardStates.Chase;
        }


        //Get a line from the guard to the point of interest, and check for intersection
        Vector2 POIDirection = guardBehaviour.PointOfInterest - guardBehaviour.visionCone.transform.position;
        RaycastHit2D rayHit = Physics2D.Raycast(guardAttached.transform.position, POIDirection, POIDirection.magnitude, 0b0110011);

        if (!rayHit)
        {
            guardBehaviour.MoveTo(guardBehaviour.PointOfInterest);
        }
        else
        {
            guardBehaviour.StopMoving();
            return GuardStates.LookAround;
        }


        if (guardBehaviour.getDistLeft() < 0.1f && !lookingAround)
        {
            guardBehaviour.StartCoroutine(lookAround());
        }
        if (finished)
        {
            return GuardStates.LookAround;
        }

        return GuardStates.Investigate;
    }

    private IEnumerator lookAround()
    {
        lookingAround = true;
        yield return new WaitForSeconds(1);
        finished = true;
    }

    private IEnumerator WaitForDistCalc()
    {

        yield return new WaitForSeconds(0.1f);
        calcDistLeft = true;
    }
}
