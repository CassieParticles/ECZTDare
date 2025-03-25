using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakConsoleHackable : Hackable
{
    [SerializeField] LockableDoor door;
    public override void OnHack()
    {
        base.OnHack();
        //Unlock door
        door.Unlock();

        //Give cloak
    }
}
