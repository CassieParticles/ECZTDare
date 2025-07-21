using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HackTrigger))]
public class TriggerHackable : Hackable
{


    public override void OnHack()
    {
        base.OnHack();
        GetComponent<HackTrigger>().Hacked();
    }
}
