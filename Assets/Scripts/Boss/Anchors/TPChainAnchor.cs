using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnchorTrigger))]
public class TPChainAnchor : BaseAnchor
{
    public override void AddSpotlight(SpotlightMovement spotlight)
    {
        base.AddSpotlight(spotlight);

        spotlight.transform.position = transform.position;
        GetComponent<AnchorTrigger>().SendSignal();
    }
}
