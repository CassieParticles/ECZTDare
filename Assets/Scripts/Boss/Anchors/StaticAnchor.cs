using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAnchor : BaseAnchor
{
    public override void AddSpotlight(SpotlightMovement spotlight)
    {
        base.AddSpotlight(spotlight);

        spotlight.MoveTo(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
