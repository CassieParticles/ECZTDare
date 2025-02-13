using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmMusicHandler : MonoBehaviour
{
   public AK.Wwise.Event music;
    AlarmSystem alarm;

    private void AlarmOn(Vector3 playerPosition)
    {
        //Sets the "Music" State Group's active State to "Alert"
        AkSoundEngine.SetState("Music", "Alert");
    }

    private void AlarmOff()
    {
        //Sets the "Music" State Group's active State to "Hidden"
        AkSoundEngine.SetState("Music", "Hidden");
    }

    private void Awake()
    {
        alarm = GameObject.Find("AlarmObject").GetComponent<AlarmSystem>();

    }

    private void Start()
    {
        alarm.AddAlarmEnableFunc(AlarmOn);
        alarm.AddAlarmDisableFunc(AlarmOff);

        music.Post(gameObject);
        //Sets the "Music" State Group's active State to "Hidden"
        AkSoundEngine.SetState("Music", "Hidden");
    }
}
