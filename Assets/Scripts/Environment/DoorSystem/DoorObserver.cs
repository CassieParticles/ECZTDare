using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObserver : MonoBehaviour
{
    public delegate void DoorStateCallback(LockableDoor.DoorAction action);

    private ArrayList callbacks;

    public void NotifyListeners(LockableDoor.DoorAction action)
    {
        foreach (DoorStateCallback callback in callbacks)
        {
            callback(action);
        }
    }

    public void AddListener(DoorStateCallback callback)
    {
        callbacks.Add(callback);
    }

    public void RemoveListener(DoorStateCallback callback)
    {
        callbacks.Remove(callback);
    }


    private void Awake()
    {
        callbacks = new ArrayList();
    }

}
