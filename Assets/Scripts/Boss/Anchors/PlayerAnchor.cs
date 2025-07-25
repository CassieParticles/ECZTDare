using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnchor : BaseAnchor
{
    private void FixedUpdate()
    {
        if(!spotlight)
        {
            return;
        }
        spotlight.TeleportTo(transform.position);
        if (!FindAnyObjectByType<BackwallDetectability>().playerVisible)
        {
            spotlight.GetComponent<SpotlightAI>().BacktrackAnchor();
        }
    }
}
