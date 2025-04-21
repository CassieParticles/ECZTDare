using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakConsoleHackable : Hackable
{
    UIModeChange modeChanger;
    Subtitle upgradeText;
    [SerializeField] LockableDoor door;

    private void Start() {
        modeChanger = GameObject.Find("GameController").GetComponent<UIModeChange>();
        upgradeText = GetComponent<Subtitle>();
    }
    public override void OnHack()
    {
        base.OnHack();
        //Unlock door
        door.Unlock();

        if (!modeChanger.player.boostCloakUnlocked) {
            //Give cloak
            modeChanger.CollectUpgrade();
            upgradeText.StartSubtitle("You have unlocked the cloak and boost! " +
                                      "The Ability you have access to depends on the mode you're in. " +
                                      "Press Shift to activate.");

        }
        //Disable hacking
        enabled = false;

        GetComponent<PolygonCollider2D>().enabled = false;
    }
}
