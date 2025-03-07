using System.Collections.Generic;
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

    public void RemoveAlarmEnableFunc(AlarmEnable alarmFunc)
    {
        alarmEnableFuncs.Remove(alarmFunc);
    }

    public void RemoveAlarmDisableFunc(AlarmDisable alarmFunc)
    {
        alarmDisableFuncs.Remove(alarmFunc);
    }

    public bool AlarmGoingOff() { return alarm; }

    public void StartAlarm(Vector3 playerPosition)
    {
        for (int i = 0; i < alarmEnableFuncs.Count; ++i)
        {
            alarmEnableFuncs[i](playerPosition);
        }
        alarm = true;
    }

    public void StopAlarm()
    {
        for (int i = 0; i < alarmDisableFuncs.Count; ++i)
        {
            alarmDisableFuncs[i]();
        }
        alarm = false;
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
