using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpedState : BaseState
{

    private bool chase;

    private IEnumerator BeginChase()
    {
        yield return new WaitForSeconds(0.5f);
        chase = true;
        guardBehaviour.suspicion = 100;
    }

    public BumpedState(GameObject guard) : base(guard)
    {
    }



    public override void Start()
    {
        chase = false;
        guardBehaviour.StartCoroutine(BeginChase());
    }

    public override void Stop()
    {
        
    }

    public override GuardStates RunTick()
    {
        if(chase)
        {
            return GuardStates.Chase;
        }
        

        if(guardBehaviour.Player)
        {
            guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;
        }
        guardBehaviour.LookAt(guardBehaviour.PointOfInterest);

        return GuardStates.Bumped;
    }
}
