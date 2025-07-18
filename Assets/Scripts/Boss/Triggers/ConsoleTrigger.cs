using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConsoleHackable))]
public class ConsoleTrigger : BaseTrigger
{
    private void TriggerFunction()
    {
        reciever.RecieveSignal(anchor);
        Debug.Log("Signal sent");
    }

    private void Start()
    {
        GetComponent<ConsoleHackable>().AddConsoleListener(TriggerFunction);
        Debug.Log("Function added");
    }
}
