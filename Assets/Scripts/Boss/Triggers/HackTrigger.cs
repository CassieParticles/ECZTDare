using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackTrigger : BaseTrigger
{
    public void Hacked()
    {
        reciever.RecieveSignal(anchor);
    }
}
