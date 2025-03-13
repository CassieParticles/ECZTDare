using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAlarmHandler : MonoBehaviour
{
    public AlarmSystem alarmSystem;
    [SerializeField] GameObject unaware;
    [SerializeField] GameObject alert;
    [SerializeField] GameObject alarm;
    void Start()
    {
        alarmSystem.AddAlarmEnableFunc(alarmOn);
        alarmSystem.AddAlarmDisableFunc(alarmOff);
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
    public void changeAlarm(AlarmSystem newAlarm)
    { 
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
