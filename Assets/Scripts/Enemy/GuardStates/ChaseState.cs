using UnityEngine;

public class ChaseState : BaseState
{
    public ChaseState(GameObject guard) : base(guard) { }

    public override void Start()
    {
        AlarmMusicHandler.GetMusicHandler().BeginChase(guardBehaviour);
    }

    public override void Stop()
    {
        AlarmMusicHandler.GetMusicHandler().EndChase(guardBehaviour);
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
