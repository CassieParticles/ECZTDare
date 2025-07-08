using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleDoorTrigger:ITriggerDoor
{
    [SerializeField] protected DoorAction action;
    [SerializeField] private ConsoleHackable console;

    private void ConsoleListenerFunction()
    {
        State = action;
        
    }

    private new void Awake()
    {
        base.Awake();
        //Register listener function
        if(console)
        {
            console.AddConsoleListener(ConsoleListenerFunction);
        }
    }
}
