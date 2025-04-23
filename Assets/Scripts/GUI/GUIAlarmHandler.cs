using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAlarmHandler : MonoBehaviour
{
    [NonSerialized] public AlarmSystem alarmSystem;
    Animator eyeAnimator;
    int state; //0 is hidden, 1 is alert, 2 is alarm

    int enemySeenCount = 0;
    public void EnemySeePlayer()
    {
        enemySeenCount++;

        //If first enemy, and not currently in alarm, change to alert
        if (enemySeenCount == 1 && state != 2)
        {
            ToAlert();
        }
    }

    public void EnemyLosePlayer()
    {
        if (enemySeenCount == 0) { return; }
        enemySeenCount--;
        if(enemySeenCount == 0 && state !=2)
        {
            ToHidden();
        }
    }
    private void Awake()
    {
        eyeAnimator = GetComponent<Animator>();
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

    public void alarmOn(Vector3 playerPos)
    {
        ToAlarm();
    }
    public void alarmOff()
    {
        ToHidden();
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

    void ToAlarm()
    {
        state = 2;
        eyeAnimator.SetInteger("State", state);
    }

    void ToAlert() 
    {
        state = 1;
        eyeAnimator.SetInteger("State", state);
    }

    void ToHidden() 
    {
        state = 0;
        eyeAnimator.SetInteger("State", state);
    }

}
