using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BaseState
{
    private Coroutine raiseAlarmCoroutine;
    private IEnumerator raiseAlarm()
    {
        yield return new WaitForSeconds(guardBehaviour.chaseAlarmTimer);
        shouldRaiseAlarm = true;
    }
    bool shouldRaiseAlarm;

    AlarmSystem alarm;

    public ChaseState(GameObject guard, AlarmSystem alarm) : base(guard) { this.alarm = alarm; }

    public override void Start()
    {
        AlarmMusicHandler.GetMusicHandler().BeginChase(guardBehaviour);
        raiseAlarmCoroutine = guardBehaviour.StartCoroutine(raiseAlarm());
        shouldRaiseAlarm = false;
    }

    public override void Stop()
    {
        AlarmMusicHandler.GetMusicHandler().EndChase(guardBehaviour);
        //If chase is exited early, stop the co-routine
        if(raiseAlarmCoroutine != null ) 
        {
            guardBehaviour.StopCoroutine(raiseAlarmCoroutine);
        }
    }

    public override GuardStates RunTick()
    {
        //If player is not visible, start searching for them, continue chase if seen again

        //If player is not visible, and at target, then call alarm
        if(!guardBehaviour.Player && (guardBehaviour.getCurrentDestination()-guardAttached.transform.position).sqrMagnitude < 0.15f)
        {
            return GuardStates.RaiseAlarm;
        }

        if (guardAttached.GetComponent<NavMeshAgent>().speed > guardBehaviour.chaseSpeed)
        {
            guardAttached.GetComponent<NavMeshAgent>().speed = guardBehaviour.chaseSpeed;
        }
        else if (guardAttached.GetComponent<NavMeshAgent>().speed < guardBehaviour.chaseSpeed) 
        {
            guardAttached.GetComponent<NavMeshAgent>().speed += guardBehaviour.acceleration * Time.fixedDeltaTime;
        }

        if(guardBehaviour.Player)
        {
            guardBehaviour.PointOfInterest = guardBehaviour.Player.transform.position;
        }

        //Raise the alarm while chasing
        if(shouldRaiseAlarm)
        {
            raiseAlarmCoroutine = null;
            if(alarm)
            {
                alarm.StartAlarm(guardBehaviour.PointOfInterest);
                return GuardStates.StateChangedExternally;
            }
        }

        guardBehaviour.MoveTo(guardBehaviour.PointOfInterest);
        return GuardStates.Chase;
    }
}
