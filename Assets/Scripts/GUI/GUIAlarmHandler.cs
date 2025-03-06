using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAlarmHandler : MonoBehaviour
{
    public AlarmSystem alarmSystem;
    void Start()
    {
        alarmSystem.AddAlarmEnableFunc(alarmOn);
        alarmSystem.AddAlarmDisableFunc(alarmOff);
    }

    // Update is called once per frame

    public void alarmOn(Vector3 playerPos)
    {

    }
    public void alarmOff()
    {

    }

    //can be used to change the alarm that is referenced by the script during functions.
    public void changeAlarm(AlarmSystem newAlarm)
    {
        if (alarmSystem)
        {
            alarmSystem.RemoveAlarmDisableFunc(alarmOff);
            alarmSystem.RemoveAlarmEnableFunc(alarmOn);
        }
        alarmSystem = newAlarm;
        alarmSystem.AddAlarmEnableFunc(alarmOn);
        alarmSystem.AddAlarmDisableFunc(alarmOff);
    }

}
