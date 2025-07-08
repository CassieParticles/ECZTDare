using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HackableDoorTrigger))]
public class DoorHackable : Hackable
{
    public AK.Wwise.Event Hack_Start;

    HackableDoorTrigger doorTrigger;

    private void Awake()
    {
        doorTrigger = GetComponent<HackableDoorTrigger>();
    }
    public override void OnHack()
    {
        base.OnHack();
        doorTrigger.DoorTrigger();
        Hack_Start.Post(gameObject);
    }
}
