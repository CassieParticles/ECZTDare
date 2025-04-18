using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleHackable : Hackable
{
    [SerializeField] Sprite HackedSprite;

    [SerializeField] DoorButton.DoorAction action=DoorButton.DoorAction.Unlock;
    // Start is called before the first frame update

    [SerializeField] private LockableDoor[] doors;

    public override void OnHack()
    {
        base.OnHack();

        if(HackedSprite)
        {
            GetComponent<SpriteRenderer>().sprite = HackedSprite;
        }

        switch (action)
        {
            case DoorButton.DoorAction.Unlock:
                foreach (LockableDoor door in doors)
                {
                    door.Unlock();
                }
                break;
            case DoorButton.DoorAction.Lock:
                foreach (LockableDoor door in doors)
                {
                    door.Lock();
                }
                break;
            case DoorButton.DoorAction.Toggle:
                foreach (LockableDoor door in doors)
                {
                    door.ToggleState();
                }
                break;
        }
        enabled = false;
    }
}
