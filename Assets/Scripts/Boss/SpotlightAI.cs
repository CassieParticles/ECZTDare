using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpotlightMovement),typeof(SignalReciever))]
public class SpotlightAI : MonoBehaviour,IRecieveSignals
{
    SpotlightMovement spotlight;

    private void Awake()
    {
        spotlight = GetComponent<SpotlightMovement>();
    }

    public void RecieveSignal(BaseAnchor anchor)
    {
        anchor.AddSpotlight(spotlight);
    }
}
