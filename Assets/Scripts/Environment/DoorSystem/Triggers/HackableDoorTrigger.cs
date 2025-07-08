using UnityEngine;


public class HackableDoorTrigger : ITriggerDoor
{
    [SerializeField] private DoorAction action;
    LockableDoor door;

    public void Awake()
    {
        door = transform.parent.GetComponent<LockableDoor>();
    }

   
    public void DoorTrigger()
    {
        switch (action)
        {
            case DoorAction.Lock:
                door.Lock();
                break;
            case DoorAction.Unlock:
                door.Unlock();
                break;
            case DoorAction.Toggle:
                door.ToggleState();
                break;
        }
    }
}
