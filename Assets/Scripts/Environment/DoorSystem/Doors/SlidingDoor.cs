using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : BaseDoor
{
    [SerializeField] private Vector2 OpenPosition;
    [SerializeField] private Vector2 ClosePosition;

    [SerializeField] private float speed;

    Vector2 desiredLocation;
    public override void Lock()
    {
        desiredLocation = ClosePosition;
        isLocked = true;
    }

    public override void Unlock()
    {
        desiredLocation = OpenPosition;
        isLocked = false;
    }

    public override void ToggleState()
    {
        desiredLocation = isLocked ? OpenPosition : ClosePosition;
        isLocked = !isLocked;
    }

    private new void Awake()
    {
        base.Awake();
        if(startLocked)
        {
            transform.position = ClosePosition;
        }
        else
        {
            transform.position = OpenPosition;
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
        }
        //Add to distance
        transform.position += (Vector3)distance.normalized * speed * Time.fixedDeltaTime;

    }
}
