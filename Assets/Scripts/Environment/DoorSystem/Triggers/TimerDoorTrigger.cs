using UnityEngine;

public class TimerDoorTrigger : ITriggerDoor
{
    [SerializeField] private float DoorOpenTime = 3.0f;
    [SerializeField] private float DoorCloseTime = 3.0f;
    [SerializeField] private bool repeatedlySwitch = true;

    float timer;
    bool currentlyOpen;
    bool hasSwitched;

    float currentTimeCheck;

    private new void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        currentlyOpen = door.isLocked;
        SetState(currentlyOpen ? DoorAction.Unlock : DoorAction.Lock);
    }

    private void FixedUpdate()
    {
        if (hasSwitched && !repeatedlySwitch){ return; }

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
