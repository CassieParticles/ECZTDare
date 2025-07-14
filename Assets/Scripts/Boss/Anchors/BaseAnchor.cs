using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAnchor : MonoBehaviour
{
    [SerializeField] protected bool Distractable;
    protected SpotlightMovement spotlight;

    public virtual void AddSpotlight(SpotlightMovement spotlight)
    {
        this.spotlight = spotlight;
        spotlight.gameObject.SetActive(true);
    }

    public virtual void RemoveSpotlight()
    {
        spotlight = null;
    }
}
