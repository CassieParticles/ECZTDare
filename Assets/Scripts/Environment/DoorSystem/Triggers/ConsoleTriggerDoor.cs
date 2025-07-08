using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleTriggerDoor:ITriggerDoor
{
    [SerializeField] private ConsoleHackable console;

    private new void Awake()
    {
        base.Awake();
        //Register listener function
        if(console)
        {
            console.AddConsoleListener(DoorTrigger);
        }
    }
}
