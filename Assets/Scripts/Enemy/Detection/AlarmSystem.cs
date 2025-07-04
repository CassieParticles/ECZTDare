using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    //Get the lights that are intersecting with an alarm (lights that will be turned on/off with an alarm)
    public List<Light2D> GetLights()
    {
        //Get relevant lights
        List<Collider2D> results = new List<Collider2D>();
        GetComponent<BoxCollider2D>().Overlap(results);

        List<Light2D> lights = new List<Light2D>();

        foreach (Collider2D collider in results)
        {
            Light2D light=collider.GetComponent<Light2D>();
            if(light)
            {
                lights.Add(light);
            }
        }

        return lights;
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
