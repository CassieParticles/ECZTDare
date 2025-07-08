using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDoorTrigger : ITriggerDoor
{
    [SerializeField] DoorObserver observer;

    void DoorListener(DoorAction action)
    {
        if (action == DoorAction.Toggle){ action = DoorAction.Lock; }
        State = (DoorAction)(1 - (int)action);
    }

    private new void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        observer.AddListener(DoorListener);
    }
}
