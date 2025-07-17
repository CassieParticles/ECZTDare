using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Guard pods are a kind of door, just not for the player
public class GuardPod : BaseDoor
{
    PodGuardBehaviour podGuard;

    private bool playerOverlapping;
    private bool spawnBlocked = false;
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
        if(playerOverlapping)
        {
            spawnBlocked = true;
            return;
        }
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
        podGuard.gameObject.SetActive(false);
        isLocked = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MovementScript>())
        {
            playerOverlapping = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MovementScript>())
        {
            playerOverlapping=false;
            if(spawnBlocked)
            {
                spawnBlocked = false;
                Unlock();
            }
        }
    }
}
