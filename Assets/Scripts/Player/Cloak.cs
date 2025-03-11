using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cloak
{
    MovementScript player;

    private bool cloaked;

    public Cloak() 
    {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }
    /// <summary>
    /// Called the frame that cloaking starts
    /// </summary>
    public void Start()
    {

    }

    /// <summary>
    /// Called the frame that cloaking stops
    /// </summary>
    public void Stop()
    {

    }

    /// <summary>
    /// Called every tick in which the cloaking is happening
    /// </summary>
    public void OnTick()
    {

    }
}
