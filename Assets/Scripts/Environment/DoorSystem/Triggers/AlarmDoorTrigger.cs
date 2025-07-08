using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmDoorTrigger : ITriggerDoor
{
    [SerializeField] AlarmSystem alarm;

    private void AlarmOn(Vector3 playerPosition, GameObject alarmCaller)
    {
        action = DoorAction.Lock;
        DoorTrigger();
    }

    private void AlarmOff()
    {
        action = DoorAction.Unlock;
        DoorTrigger();
    }

    private new void Awake()
    {
        if(alarm)
        {
            alarm.AddAlarmEnableFunc(AlarmOn);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }
    }
}
