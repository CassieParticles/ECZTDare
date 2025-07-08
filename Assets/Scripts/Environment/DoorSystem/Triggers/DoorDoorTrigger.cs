using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDoorTrigger : ITriggerDoor
{
    [SerializeField] DoorObserver observer;

    void DoorListener(DoorAction action)
    {
        if (action == DoorAction.Toggle){ action = DoorAction.Lock; }
        this.action = (DoorAction)(1 - (int)action);
        DoorTrigger();
    }

    private new void Awake()
    {
        base.Awake();
        observer.AddListener(DoorListener);
    }
}
