using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleHackable : Hackable
{
    public delegate void ConsoleListenerFunction();

    public ArrayList ConsoleListeners=new ArrayList();

    public void AddConsoleListener(ConsoleListenerFunction listener)
    {
        ConsoleListeners.Add(listener);
    }

    public void RemoveConsoleListener(ConsoleListenerFunction listener)
    {
        ConsoleListeners.Remove(listener);
    }

    public bool hasBeenHacked = false;

    public override void OnHack()
    {
        base.OnHack();

        foreach (ConsoleListenerFunction listener in ConsoleListeners)
        {
            listener();
        }


        GetComponent<PolygonCollider2D>().enabled = false;
        enabled = false;
    }
}
