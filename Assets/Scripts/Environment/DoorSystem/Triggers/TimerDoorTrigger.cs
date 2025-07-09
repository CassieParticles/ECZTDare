using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimerDoorTrigger : ITriggerDoor
{
    [SerializeField] private float DoorOpenTime;
    [SerializeField] private float DoorCloseTime;
    [SerializeField] private bool StartOpen;

    float timer;
    bool currentlyOpen;

    float currentTimeCheck;

    private new void Awake()
    {
        base.Awake();
        currentlyOpen = StartOpen;
    }

    private void Start()
    {
        SetState(StartOpen ? DoorAction.Unlock : DoorAction.Lock);
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
            SetState(currentlyOpen ? DoorAction.Unlock : DoorAction.Lock);
        }
    }
}
