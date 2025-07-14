using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAnchor : BaseAnchor
{
    public override void AddSpotlight(SpotlightMovement spotlight)
    {
        base.AddSpotlight(spotlight);

        spotlight.transform.position = transform.position;
        spotlight.gameObject.SetActive(false);
    }
}
