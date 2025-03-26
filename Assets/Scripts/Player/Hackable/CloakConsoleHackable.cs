using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakConsoleHackable : Hackable
{
    MovementScript player;
    [SerializeField] LockableDoor door;

    private void Start() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }
    public override void OnHack()
    {
        base.OnHack();
        //Unlock door
        door.Unlock();

        //Give cloak
        player.boostCloakUnlocked = true;
    }
}
