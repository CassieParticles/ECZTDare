using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentAlarmTracker : MonoBehaviour
{
    public delegate void ChangeAlarm(AlarmSystem newAlarm);

    private ArrayList listeners;

    public void AddListener(ChangeAlarm listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(ChangeAlarm listener)
    {
        listeners.Remove(listener);
    }

    public void AlarmUpdate(AlarmSystem newAlarm)
    {
        foreach (ChangeAlarm listener in listeners)
        {
            listener(newAlarm);
        }
    }

    private void Awake()
    {
        listeners=new ArrayList();
    }
}
