using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SignalReciever))]
public class DelayTrigger : BaseTrigger,IRecieveSignals
{
    [SerializeField] private float DelayTime;
    private IEnumerator SendSignal(float delay,BaseAnchor anchor)
    {
        yield return new WaitForSeconds(delay);
        reciever.RecieveSignal(anchor);
    }

    public void RecieveSignal(BaseAnchor anchor)
    {
        StartCoroutine(SendSignal(DelayTime, anchor));
    }

    
}
