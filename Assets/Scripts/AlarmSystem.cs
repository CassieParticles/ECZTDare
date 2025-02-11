using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    public delegate void AlarmEnable(Vector3 playerPosition);
    public delegate void AlarmDisable();
    
    public static AlarmSystem GetAlarmSystem()
    {
        return GameObject.Find("AlarmObject").GetComponent<AlarmSystem>();
    }

    public void AddAlarmEnableFunc(AlarmEnable alarmFunc)
    {
        alarmEnableFuncs.Add(alarmFunc);
    }

    public void AddAlarmDisableFunc(AlarmDisable alarmFunc)
    {
        alarmDisableFuncs.Add(alarmFunc);
    }

    public bool AlarmGoingOff() { return alarm; }

    public void StartAlarm(Vector3 playerPosition)
    {
        if(!alarm)
        {
            for (int i = 0; i < alarmEnableFuncs.Count; ++i)
            {
                alarmEnableFuncs[i](playerPosition);
            }
            alarm = true;
        }

    }

    public void StopAlarm()
    {
        if (alarm)
        {
            for (int i = 0; i < alarmDisableFuncs.Count; ++i)
            {
                alarmDisableFuncs[i]();
            }
            alarm = false;
        }
    }

    //List of functions to be called
    private List<AlarmEnable> alarmEnableFuncs;
    private List<AlarmDisable> alarmDisableFuncs;

    //Whether the alarm is current being sounded
    bool alarm;

    private void Awake()
    {
        alarm = false;
        alarmEnableFuncs = new List<AlarmEnable>();
        alarmDisableFuncs = new List<AlarmDisable>();
    }
}
