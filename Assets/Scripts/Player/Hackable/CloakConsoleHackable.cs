using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakConsoleHackable : Hackable
{
    UIModeChange modeChanger;
    Subtitle upgradeText;

    private void Start() {
        modeChanger = GameObject.Find("GameController").GetComponent<UIModeChange>();
        upgradeText = GetComponent<Subtitle>();
    }
    public override void OnHack()
    {
        base.OnHack();
        //Unlock door

        //if (!modeChanger.player.boostCloakUnlocked) {
            //Give cloak
            modeChanger.CollectUpgrade();
            upgradeText.StartSubtitle("You have unlocked the cloak! " +
                                      "Press RMB to activate.");

        //}
        //Disable hacking
        enabled = false;

        GetComponent<PolygonCollider2D>().enabled = false;
    }
}
