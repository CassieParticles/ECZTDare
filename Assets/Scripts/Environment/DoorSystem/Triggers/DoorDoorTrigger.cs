using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDoorTrigger : ITriggerDoor
{
    [SerializeField] DoorObserver observer;
    [SerializeField] bool mimicDoor = false;

    void DoorListener(DoorAction action)
    {
        //If door needs to toggle, toggle it
        if (action == DoorAction.Toggle)
        { ToggleDoor(); }

        SetState((action == DoorAction.Lock) != mimicDoor ? DoorAction.Unlock : DoorAction.Lock);
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
