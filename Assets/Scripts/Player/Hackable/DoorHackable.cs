using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHackable : Hackable
{
    public AK.Wwise.Event Hack_Start;

    LockableDoor door;

    private void Awake()
    {
        door = GetComponent<LockableDoor>();
    }
    public override void OnHack()
    {
        base.OnHack();
        door.ToggleState();
        Hack_Start.Post(gameObject);
    }
}
