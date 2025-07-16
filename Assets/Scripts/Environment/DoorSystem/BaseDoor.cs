using UnityEngine;


[RequireComponent(typeof(DoorObserver))]
public abstract class BaseDoor : MonoBehaviour
{
    [SerializeField] protected bool startLocked = true;

    public bool isLocked { get; protected set; }

    protected DoorObserver observer;


    public abstract void Lock();
    public abstract void Unlock();
    public abstract void ToggleState();

    protected void Awake()
    {
        isLocked = startLocked;

        observer = GetComponent<DoorObserver>();
    }
}
