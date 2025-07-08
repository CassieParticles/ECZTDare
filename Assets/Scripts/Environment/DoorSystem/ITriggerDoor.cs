using UnityEngine;

public abstract class ITriggerDoor:MonoBehaviour
{
    [SerializeField] protected DoorAction action;
    protected LockableDoor door;

    protected void Awake()
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
