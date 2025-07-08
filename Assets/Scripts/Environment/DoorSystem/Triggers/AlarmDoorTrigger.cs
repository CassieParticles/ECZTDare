using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmDoorTrigger : ITriggerDoor
{
    [SerializeField] AlarmSystem alarm;

    private void AlarmOn(Vector3 playerPosition, GameObject alarmCaller)
    {
        State = DoorAction.Lock;
    }

    private void AlarmOff()
    {
        State = DoorAction.Unlock;
        
    }

    private void Start()
    {
        if(alarm)
        {
            alarm.AddAlarmEnableFunc(AlarmOn);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }

        State = DoorAction.Unlock;
    }
}
