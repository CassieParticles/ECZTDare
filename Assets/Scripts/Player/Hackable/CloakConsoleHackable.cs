using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakConsoleHackable : Hackable
{
    UIModeChange modeChanger;
    [SerializeField] LockableDoor door;

    private void Start() {
        modeChanger = GameObject.Find("GameController").GetComponent<UIModeChange>();
    }
    public override void OnHack()
    {
        base.OnHack();
        //Unlock door
        door.Unlock();

        //Give cloak
        modeChanger.CollectUpgrade();

        //Disable hacking
        enabled = false;

        GetComponent<PolygonCollider2D>().enabled = false;
    }
}
