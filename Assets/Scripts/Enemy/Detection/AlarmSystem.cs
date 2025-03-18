using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    [SerializeField] private float alarmCooloffTime=30.0f;

    public delegate void AlarmEnable(Vector3 playerPosition);
    public delegate void AlarmDisable();

    public Coroutine AlarmCoolOffTimer;

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

    private IEnumerator AlarmCooloff()
    {
        yield return new WaitForSeconds(alarmCooloffTime);
        StopAlarm();
    }

    public bool AlarmGoingOff() { return alarm; }

    public void StartAlarm(Vector3 playerPosition)
    {
        if(!alarm)
        {
            StealthScoreTracker.GetTracker().RemoveScore(500);
        }
        for (int i = 0; i < alarmEnableFuncs.Count; ++i)
        {
            alarmEnableFuncs[i](playerPosition);
        }
        alarm = true;

        //Exit currently running coroutine 
        if(AlarmCoolOffTimer!=null)
        {
            StopCoroutine(AlarmCoolOffTimer);
            AlarmCoolOffTimer = null;
        }
        //Start/restart coroutine
        AlarmCoolOffTimer = StartCoroutine(AlarmCooloff());
    }

    public void StopAlarm()
    {
        for (int i = 0; i < alarmDisableFuncs.Count; ++i)
        {
            alarmDisableFuncs[i]();
        }
        alarm = false;

        //Stop alarm cooloff timer
        if (AlarmCoolOffTimer != null)
        {
            StopCoroutine(AlarmCoolOffTimer);
            AlarmCoolOffTimer = null;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Is player
        if(collision.GetComponent<MovementScript>())
        {
            collision.GetComponent<CurrentAlarmTracker>().AlarmUpdate(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Is player
        if (collision.GetComponent<MovementScript>())
        {
            collision.GetComponent<CurrentAlarmTracker>().AlarmUpdate(null);
        }
    }
}
