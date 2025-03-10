using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BaseState
{


    public ChaseState(GameObject guard) : base(guard) { }

    public override void Start()
    {
        AlarmMusicHandler.GetMusicHandler().BeginChase(guardBehaviour);
        guardAttached.GetComponent<NavMeshAgent>().speed = guardBehaviour.chaseSpeed;
    }

    public override void Stop()
    {
        AlarmMusicHandler.GetMusicHandler().EndChase(guardBehaviour);
        guardAttached.GetComponent<NavMeshAgent>().speed = guardBehaviour.walkSpeed;
    }

    public override GuardStates RunTick()
    {
        //If player is not visible, start searching for them, continue chase if seen again

        //If player is not visible, and at target, then call alarm
        if(!guardBehaviour.Player && (guardBehaviour.getCurrentDestination()-guardAttached.transform.position).sqrMagnitude < 0.15f)
        {
            return GuardStates.RaiseAlarm;
        }

       
        if(guardBehaviour.Player)
        {
            guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;
        }

        guardBehaviour.MoveTo(guardBehaviour.PointOfInterest);
        return GuardStates.Chase;
    }
}
