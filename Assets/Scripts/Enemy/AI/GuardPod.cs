using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Guard pods are a kind of door, just not for the player
public class GuardPod : BaseDoor
{
    GuardBehaviour
    public override void Lock()
    {
        if (isLocked){ return; }
        //Tell the guard to return to base
        isLocked = true;
    }

    public override void Unlock()
    {
        if(isLocked){ return; }
        //Enable the guard to wander about
        isLocked = false;
    }

    public override void ToggleState()
    {
        //Toggle between the states
        isLocked = !isLocked;
    }
}
