using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerTrigger : BaseTrigger
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        MovementScript player = collision.GetComponent<MovementScript>();
        BaseAnchor anchor = FindAnyObjectByType<SpotlightAI>().getCurrentAnchor();
        //Already attached to player anchor
        if(anchor is PlayerAnchor)
        {
            return;
        }


        if (player && !player.cloaking)
        {
            BackwallDetectability backwall = FindAnyObjectByType<BackwallDetectability>();
            if(!backwall || backwall.playerVisible)
            {
                if (GetComponent<SpotlightAI>().IsDistractible())
                {
                    reciever.RecieveSignal(player.GetComponent<PlayerAnchor>());
                }
            }

        }
    }
}
