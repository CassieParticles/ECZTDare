using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnchorTrigger))]
public class TriggerAnchor : BaseAnchor
{
    Coroutine travelTime;
    private IEnumerator TravelTime()
    {
        spotlight.MoveTo(transform.position);
        yield return new WaitForSeconds(spotlight.travelTime);
        //Send signal
        FindAnyObjectByType<AnchorTrigger>().SendSignal();
    }
    public override void AddSpotlight(SpotlightMovement spotlight)
    {
        //Spotlight moves to position
        base.AddSpotlight(spotlight);
        travelTime = StartCoroutine(TravelTime());
    }

    public override void RemoveSpotlight()
    {
        base.RemoveSpotlight();
        if(travelTime!=null)
        {
            StopCoroutine(travelTime);
            travelTime = null;
        }
    }
}
