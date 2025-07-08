using UnityEngine;


public class HackableDoorTrigger : ITriggerDoor
{
    [SerializeField] protected DoorAction action;
    public void Trigger()
    {
        State = action;
    }
}
