using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleHackable : Hackable
{
    [SerializeField] DoorAction action= DoorAction.Unlock;
    // Start is called before the first frame update

    [SerializeField] private LockableDoor[] doors;

    public bool hasBeenHacked = false;

    public override void OnHack()
    {
        base.OnHack();

        

        switch (action)
        {
            case DoorAction.Unlock:
                foreach (LockableDoor door in doors)
                {
                    door.Unlock();
                }
                break;
            case DoorAction.Lock:
                foreach (LockableDoor door in doors)
                {
                    door.Lock();
                }
                break;
            case DoorAction.Toggle:
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
