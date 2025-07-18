using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDoorTrigger : ITriggerDoor
{
    [SerializeField] protected DoorAction action;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetState(action);
    }
}
