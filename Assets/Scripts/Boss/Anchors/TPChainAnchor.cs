using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnchorTrigger))]
public class TPChainAnchor : BaseAnchor
{

    private IEnumerator MoveOn()
    {
        yield return new WaitForFixedUpdate();
        GetComponent<AnchorTrigger>().SendSignal();
    }
    public override void AddSpotlight(SpotlightMovement spotlight)
    {
        base.AddSpotlight(spotlight);

        spotlight.transform.position = transform.position;
        spotlight.MoveTo(transform.position);
        StartCoroutine(MoveOn());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
