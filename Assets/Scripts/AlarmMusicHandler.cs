using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmMusicHandler : MonoBehaviour
{
    AlarmSystem alarm;

    private void AlarmOn(Vector3 playerPosition)
    {
        //Sets the "Music" State Group's active State to "Alarm"
        AkSoundEngine.SetState("Music", "Alarm");
    }

    private void AlarmOff()
    {
        //Sets the "Music" State Group's active State to "Hidden"
        AkSoundEngine.SetState("Music", "Hidden");
    }

    private void Awake()
    {
        alarm = AlarmSystem.GetAlarmSystem();
        alarm.AddAlarmEnableFunc(AlarmOn);
        alarm.AddAlarmDisableFunc(AlarmOff);
    }
}
