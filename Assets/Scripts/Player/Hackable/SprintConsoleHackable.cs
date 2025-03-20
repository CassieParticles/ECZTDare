using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintConsoleHackable : Hackable
{
    [SerializeField] private LockableDoor door;
    public override void OnHack()
    {
        //Unlock door
        door.Unlock();
        //Unlock sprint
    }
}
