using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPAnchor : BaseAnchor
{
    public override void AddSpotlight(SpotlightMovement spotlight)
    {
        base.AddSpotlight(spotlight);

        spotlight.transform.position = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
