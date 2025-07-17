using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorTrigger : BaseTrigger
{
    public void SendSignal()
    {
        reciever.RecieveSignal(anchor);
    }
}
