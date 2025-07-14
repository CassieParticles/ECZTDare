using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpotlightMovement),typeof(SignalReciever))]
public class SpotlightAI : MonoBehaviour,IRecieveSignals
{
    SpotlightMovement spotlight;
    BaseAnchor currentAnchor;

    private void Awake()
    {
        spotlight = GetComponent<SpotlightMovement>();
    }

    public void RecieveSignal(BaseAnchor anchor)
    {
        if(currentAnchor)
        {
            currentAnchor.RemoveSpotlight();
        }

        anchor.AddSpotlight(spotlight);
        currentAnchor = anchor;
    }
}
