using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseAlarmState : BaseState
{
    private AlarmSystem alarm;
    bool alarmRaised;

    private Coroutine raiseAlarmCoroutine;
    public RaiseAlarmState(GameObject guard, AlarmSystem alarm) : base(guard)
    {
        this.alarm = alarm;
    }

    public override void Start()
    {
        guardBehaviour.StopMoving();
        alarmRaised = false;
        if (alarm)
        {
            //Start coroutine to set off alarm
            raiseAlarmCoroutine = guardBehaviour.StartCoroutine(RaiseAlarm());
            guardBehaviour.alarmActivationSound.Post(guardAttached);
        }

        GUIAlarmHandler alarmHandler = GameObject.FindAnyObjectByType<GUIAlarmHandler>();
        if (alarmHandler)
        {
            alarmHandler.EnemySeePlayer();
        }
    }

    public override void Stop() {
        if(raiseAlarmCoroutine!=null)
        {
            guardBehaviour.StopCoroutine(raiseAlarmCoroutine);
            raiseAlarmCoroutine = null;
        }
        GUIAlarmHandler alarmHandler = GameObject.FindAnyObjectByType<GUIAlarmHandler>();
        if (alarmHandler)
        {
            alarmHandler.EnemyLosePlayer();
        }
    }

    public override GuardStates RunTick()
    {
        if (!alarm)
        {
            return GuardStates.Patrol;
        }
        //If it sees the player again, don't raise alarm, just chase the player
        if(guardBehaviour.Player)
        {
            return GuardStates.Chase;
        }

        if (alarmRaised)
        {
            raiseAlarmCoroutine = null;
            alarm.StartAlarm(guardBehaviour.PointOfInterest,guardAttached);
            return GuardStates.StateChangedExternally;
        }
        return GuardStates.RaiseAlarm;
    }

    private IEnumerator RaiseAlarm()
    {
        yield return new WaitForSeconds(3);
        alarmRaised = true;
    }
}