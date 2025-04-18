using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableConsoleEnable : MonoBehaviour
{
    Hackable hackable;

    private void Awake()
    {
        hackable = GetComponent<Hackable>();
        hackable.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((ConsoleHackable)hackable && ((ConsoleHackable)hackable).hasBeenHacked)
        {
            return;
        }
        if(collision.name=="Player")
        {
            hackable.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.name=="Player")
        {
            hackable.enabled=false;
        }
    }
}
