
using NavMeshPlus.Components;
using UnityEngine;


public class LockableDoor : MonoBehaviour
{
    [SerializeField] NavMeshSurface surface;
    [SerializeField] private bool startLocked = true;

    public bool isLocked { get; private set; }

    private BoxCollider2D boxCollider;

    public void Lock()
    {
        if (isLocked){ return; }    //Already locked, exit early

        //Lock door
        boxCollider.enabled = true;
        RebuildNavMesh();

        isLocked = true;
    }

    public void Unlock()
    {
        if(!isLocked){ return; }    //Already unlocked, exit early

        //Unlock door
        boxCollider.enabled = false;
        RebuildNavMesh();

        isLocked = false;
    }

    public void ToggleState()
    {
        //Switch door
        boxCollider.enabled = !boxCollider.enabled;
        RebuildNavMesh();

        isLocked = !isLocked;
    }

    void RebuildNavMesh()
    {
        if (surface)
        {
            surface.BuildNavMesh();
        }
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        isLocked = startLocked;
        if(surface)
        {
            transform.SetParent(surface.transform);
        }
    }

    private void Start()
    {
        if (isLocked)
        {
            RebuildNavMesh();
        }
    }
}
