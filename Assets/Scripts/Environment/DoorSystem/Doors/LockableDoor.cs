
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public enum DoorAction
{
    Lock,
    Unlock,
    Toggle
};
[RequireComponent(typeof(DoorObserver))]
public class LockableDoor : BaseDoor
{
    public AK.Wwise.Event doorHum;



    [SerializeField] private bool startLocked = true;

    public bool isLocked { get; private set; }

    private SpriteRenderer spriteRenderer;
    private NavMeshObstacle obstacle;
    private BoxCollider2D boxCollider;
    private DoorObserver observer;

    public override void Lock()
    {
        if (isLocked){ return; }    //Already locked, exit early

        //Lock door
        obstacle.enabled = true;
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
        doorHum.Post(gameObject);

        isLocked = true;

        observer.NotifyListeners(DoorAction.Lock);
    }

    public override void Unlock()
    {
        if(!isLocked){ return; }    //Already unlocked, exit early

        //Unlock door
        obstacle.enabled = false;
        spriteRenderer.enabled = false;
        boxCollider.enabled= false;
        doorHum.Stop(gameObject);

        isLocked = false;

        observer.NotifyListeners(DoorAction.Unlock);
    }

    public override void ToggleState()
    {
        //Switch door
        obstacle.enabled = !obstacle.enabled;
        spriteRenderer.enabled = !spriteRenderer.enabled;
        boxCollider.enabled = !boxCollider.enabled;

        isLocked = !isLocked;

        if (isLocked)
        {
            doorHum.Post(gameObject);
        }
        else
        {
            doorHum.Stop(gameObject);
        }

        observer.NotifyListeners(isLocked ? DoorAction.Lock : DoorAction.Unlock);
    }

    private void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        isLocked = startLocked;

        observer = GetComponent<DoorObserver>();
    }

    private void Start()
    {
        obstacle.enabled = isLocked;
        spriteRenderer.enabled = isLocked;
        boxCollider.enabled = isLocked;
        if (isLocked)
        {
            doorHum.Post(gameObject);
        }
    }
}
