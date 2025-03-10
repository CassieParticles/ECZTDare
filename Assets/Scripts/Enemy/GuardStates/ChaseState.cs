using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BaseState
{
    Coroutine lookingForPlayer;
    bool callAlarm;
    private IEnumerator LookForPlayer()
    {
        yield return new WaitForSeconds(3);
        callAlarm = true;
    }

    public ChaseState(GameObject guard) : base(guard) { }

    public override void Start()
    {
        AlarmMusicHandler.GetMusicHandler().BeginChase(guardBehaviour);
        guardAttached.GetComponent<NavMeshAgent>().speed = guardBehaviour.chaseSpeed;
        callAlarm = false;
    }

    public override void Stop()
    {
        AlarmMusicHandler.GetMusicHandler().EndChase(guardBehaviour);
        guardAttached.GetComponent<NavMeshAgent>().speed = guardBehaviour.walkSpeed;
    }

    public override GuardStates RunTick()
    {
        //If player is not visible, start searching for them, continue chase if seen again
        if (!guardBehaviour.Player)
        {
            lookingForPlayer = guardBehaviour.StartCoroutine(LookForPlayer());
        }
        if(lookingForPlayer!=null && guardBehaviour.Player!=null)
        {
            guardBehaviour.StopCoroutine(lookingForPlayer);
        }

        if(guardBehaviour.Player)
        {
            guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;
        }
        
        if(callAlarm)
        {
            return GuardStates.RaiseAlarm;
        }

        guardBehaviour.MoveTo(guardBehaviour.PointOfInterest);
        return GuardStates.Chase;
    }
}
