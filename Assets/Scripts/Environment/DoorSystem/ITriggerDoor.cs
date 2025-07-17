using UnityEngine;
using UnityEngine.Serialization;

public abstract class ITriggerDoor : MonoBehaviour
{
    protected BaseDoor door;
    private DoorAction currentState;

    protected void Awake()
    {
        door = transform.parent.GetComponent<BaseDoor>();
    }

    public void SetState(DoorAction newState)
    {
        currentState = newState;
        TriggerDoor();
    }

    public void LockDoor()
    {
        currentState = DoorAction.Lock;
        door.Lock();
    }

    public void UnlockDoor()
    {
        currentState=DoorAction.Unlock;
        door.Unlock();
    }

    public void ToggleDoor()
    {
        currentState = DoorAction.Toggle;
        door.ToggleState();
    }

    public void TriggerDoor()
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
