using UnityEngine;
using UnityEngine.Serialization;

public abstract class ITriggerDoor:MonoBehaviour
{
    protected LockableDoor door;
    private DoorAction currentState;
    public DoorAction State
    {
        get 
        { 
            return currentState;
        }
        set 
        { 
            currentState = value;
            DoorTrigger();
        }
    }

    protected void Awake()
    {
        door = transform.parent.GetComponent<LockableDoor>();
    }

    private void DoorTrigger()
    {
        switch (currentState)
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
