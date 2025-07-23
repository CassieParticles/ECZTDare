using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : BaseDoor
{
    [SerializeField] private Vector2 Displacement;

    private Vector2 OpenPosition;
    private Vector2 ClosePosition;

    [SerializeField] private float speed;

    Vector2 desiredLocation;
    public override void Lock()
    {
        desiredLocation = ClosePosition;
        isLocked = true;

        observer.NotifyListeners(DoorAction.Lock);
    }

    public override void Unlock()
    {
        desiredLocation = OpenPosition;
        isLocked = false;

        observer.NotifyListeners(DoorAction.Unlock);
    }

    public override void ToggleState()
    {
        desiredLocation = isLocked ? OpenPosition : ClosePosition;
        isLocked = !isLocked;

        observer.NotifyListeners(DoorAction.Toggle);
    }

    private new void Awake()
    {
        base.Awake();
        if(startLocked)
        {
            OpenPosition = transform.position + (Vector3)Displacement;
            ClosePosition = transform.position;
        }
        else
        {
            OpenPosition = transform.position;
            ClosePosition = transform.position + (Vector3)Displacement;
        }
        desiredLocation = transform.position;
    }

    private void FixedUpdate()
    {
        //Do nothing if object is ther
        if (transform.position == (Vector3)desiredLocation)
        {
            return;
        }
        //Get if object will overshoot this frame
        Vector2 distance = (Vector3)desiredLocation - transform.position;
        if(distance.sqrMagnitude < (speed * Time.fixedDeltaTime * speed * Time.fixedDeltaTime))
        {
            transform.position = desiredLocation;
            return;
        }
        //Add to distance
        transform.position += (Vector3)distance.normalized * speed * Time.fixedDeltaTime;

    }

    private void OnDrawGizmosSelected()
    {
        if(startLocked)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color=Color.red;
        }
        Gizmos.DrawWireCube(transform.position + (Vector3)Displacement, transform.localScale * GetComponent<BoxCollider2D>().size);
    }
}
