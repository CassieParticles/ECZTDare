using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmDoorTrigger : ITriggerDoor
{
    [SerializeField] AlarmSystem alarm;
    DoorAction alarmOnAction;
    DoorAction alarmOffAction;

    private void AlarmOn(Vector3 playerPosition, GameObject alarmCaller)
    {
        SetState(alarmOnAction);
    }

    private void AlarmOff()
    {
        SetState(alarmOffAction);
        
    }

    private void Start()
    {
        if(alarm)
        {
            alarm.AddAlarmEnableFunc(AlarmOn);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }

        alarmOnAction = door.isLocked ? DoorAction.Unlock : DoorAction.Lock;
        alarmOffAction = door.isLocked ? DoorAction.Lock : DoorAction.Unlock;
    }
}
