using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnchor : MonoBehaviour
{
    [SerializeField] protected bool Distractable;
    protected SpotlightMovement spotlight;

    public virtual void AddSpotlight(SpotlightMovement spotlight)
    {
        this.spotlight = spotlight;
    }

    public virtual void RemoveSpotlight()
    {
        spotlight = null;
    }
}
