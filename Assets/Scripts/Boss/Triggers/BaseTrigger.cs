using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTrigger:MonoBehaviour
{
    [SerializeField] protected BaseAnchor anchor;
    [SerializeField] protected SignalReciever reciever; //Level of indirection, since interfaces aren't serializable
}

public interface IRecieveSignals
{
    public abstract void RecieveSignal(BaseAnchor anchor);
}

