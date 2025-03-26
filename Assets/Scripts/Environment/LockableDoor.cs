
using NavMeshPlus.Components;
using UnityEngine;


public class LockableDoor : MonoBehaviour
{
    public AK.Wwise.Event doorHum;

    [SerializeField] NavMeshSurface surface;
    [SerializeField] private bool startLocked = true;

    public bool isLocked { get; private set; }

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    public void Lock()
    {
        if (isLocked){ return; }    //Already locked, exit early

        //Lock door
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;

        RebuildNavMesh();

        isLocked = true;
    }

    public void Unlock()
    {
        if(!isLocked){ return; }    //Already unlocked, exit early

        //Unlock door
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        doorHum.Stop(gameObject);
        RebuildNavMesh();

        isLocked = false;
    }

    public void ToggleState()
    {
        //Switch door
        boxCollider.enabled = !boxCollider.enabled;
        spriteRenderer.enabled = !spriteRenderer.enabled;
        RebuildNavMesh();

        isLocked = !isLocked;
    }

    void RebuildNavMesh()
    {
        Debug.Log("locked");
        if (surface)
        {
            surface.BuildNavMesh();
        }
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            doorHum.Post(gameObject);
        }
    }
}
