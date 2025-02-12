using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmMusicHandler : MonoBehaviour
{
    AlarmSystem alarm;

    private void AlarmOn(Vector3 playerPosition)
    {
        //Add alarm on music here (ignore the vector3, that has to be there
    }

    private void AlarmOff()
    {
        //Add alarm off music here
    }

    private void Awake()
    {
        alarm = AlarmSystem.GetAlarmSystem();
        alarm.AddAlarmEnableFunc(AlarmOn);
        alarm.AddAlarmDisableFunc(AlarmOff);
    }
}
