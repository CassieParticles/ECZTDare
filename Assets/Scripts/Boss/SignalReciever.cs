using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalReciever : MonoBehaviour
{
    private IRecieveSignals reciever;

    private void Awake()
    {
        reciever = GetComponent<IRecieveSignals>();
    }

    public void RecieveSignal(BaseAnchor anchor)
    {
        if (reciever != null){ reciever.RecieveSignal(anchor); }
    }
}
