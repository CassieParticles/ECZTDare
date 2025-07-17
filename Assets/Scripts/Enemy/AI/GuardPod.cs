using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Guard pods are a kind of door, just not for the player
public class GuardPod : BaseDoor
{
    PodGuardBehaviour podGuard;
    public override void Lock()
    {
        if (isLocked){ return; }
        podGuard.ReturnToPod();
        //Tell the guard to return to base
        isLocked = true;
    }

    public override void Unlock()
    {
        if(!isLocked){ return; }
        //Enable the guard to wander about
        podGuard.gameObject.SetActive(true);
        podGuard.StartGuard();

        isLocked = false;
    }

    public override void ToggleState()
    {
        //Toggle between the states
        if (isLocked){ Unlock(); }
        else{ Lock(); }
    }

    private new void Awake()
    {
        base.Awake();
        podGuard = GetComponentInChildren<PodGuardBehaviour>();
        podGuard.SetPod(this);
        podGuard.gameObject.SetActive(false);
        isLocked = true;
    }
}
