using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAlarmHandler : MonoBehaviour
{
    [NonSerialized]public AlarmSystem alarmSystem;
    GameObject unaware;
    GameObject alert;
    GameObject alarm;


    private void Awake()
    {
        unaware = transform.GetChild(0).gameObject;
        alert = transform.GetChild(1).gameObject;
        alarm = transform.GetChild(2).gameObject;
    }

    void Start()
    {
        if(alarmSystem)
        { 
            alarmSystem.AddAlarmEnableFunc(alarmOn);
            alarmSystem.AddAlarmDisableFunc(alarmOff);
        }

        CurrentAlarmTracker tracker = FindAnyObjectByType<CurrentAlarmTracker>();
        tracker.AddListener(changeAlarm);
    }

    // Update is called once per frame

    public void alarmOn(Vector3 playerPos)
    {
        alert.SetActive(false);
        alarm.SetActive(true);
    }
    public void alarmOff()
    {
        alarm.SetActive(false);
        alert.SetActive(true);
    }

    //can be used to change the alarm that is referenced by the script during functions.
    private void changeAlarm(AlarmSystem newAlarm)
    {
        Debug.Log("Hello, world!");
        //Deregister from old alarm, if it existts   
        if (alarmSystem)
        {
            alarmSystem.RemoveAlarmDisableFunc(alarmOff);
            alarmSystem.RemoveAlarmEnableFunc(alarmOn);
        }
        alarmSystem = newAlarm;
        //Register to new alar, if it exists
        if(alarmSystem)
        {
            alarmSystem.AddAlarmEnableFunc(alarmOn);
            alarmSystem.AddAlarmDisableFunc(alarmOff);
        }


        if(!alarmSystem || !alarmSystem.AlarmGoingOff())
        {
            alarmOff();
        }
        else
        {
            alarmOn(Vector3.zero);
        }
    }

}
