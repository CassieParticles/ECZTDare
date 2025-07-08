using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimerDoorTrigger : ITriggerDoor
{
    [SerializeField] private float DoorOpenTime;
    [SerializeField] private float DoorCloseTime;

    float timer;
    bool currentlyOpen;

    float currentTimeCheck;

    private new void Awake()
    {
        base.Awake();

        if(action==DoorAction.Toggle)
        {
            action = DoorAction.Lock;
        }

        currentlyOpen = action == DoorAction.Unlock;
        currentTimeCheck = currentlyOpen ? DoorOpenTime : DoorCloseTime;

        DoorTrigger();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        //Check if door should change
        if(timer > currentTimeCheck)
        {
            //Set timer to start timing next state
            timer = 0;
            currentlyOpen = !currentlyOpen;
            currentTimeCheck = currentlyOpen ? DoorOpenTime : DoorCloseTime;

            //Change door state
            action = currentlyOpen ? DoorAction.Unlock : DoorAction.Lock;
            DoorTrigger();
        }
    }
}
