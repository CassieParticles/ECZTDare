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
        if (!guardBehaviour.Player)
        {
            return GuardStates.RaiseAlarm;
        }

        guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;

        guardBehaviour.MoveTo(guardBehaviour.Player.transform.position);
        return GuardStates.Chase;
    }
}
