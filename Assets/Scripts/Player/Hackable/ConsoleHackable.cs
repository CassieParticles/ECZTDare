using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleHackable : Hackable
{
    [SerializeField] LockableDoor.DoorAction action= LockableDoor.DoorAction.Unlock;
    // Start is called before the first frame update

    [SerializeField] private LockableDoor[] doors;

    public bool hasBeenHacked = false;

    public override void OnHack()
    {
        base.OnHack();

        

        switch (action)
        {
            case LockableDoor.DoorAction.Unlock:
                foreach (LockableDoor door in doors)
                {
                    door.Unlock();
                }
                break;
            case LockableDoor.DoorAction.Lock:
                foreach (LockableDoor door in doors)
                {
                    door.Lock();
                }
                break;
            case LockableDoor.DoorAction.Toggle:
                foreach (LockableDoor door in doors)
                {
                    door.ToggleState();
                }
                break;
        }

        GetComponent<PolygonCollider2D>().enabled = false;
        enabled = false;
    }
}
