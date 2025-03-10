using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseAlarmState : BaseState
{
    private AlarmSystem alarm;
    bool alarmRaised;
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
            guardBehaviour.StartCoroutine(RaiseAlarm());
        }
    }

    public override void Stop() { }

    public override GuardStates RunTick()
    {
        if (!alarm)
        {
            return GuardStates.Patrol;
        }
        if (alarmRaised)
        {
            alarm.StartAlarm(guardBehaviour.PointOfInterest);
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