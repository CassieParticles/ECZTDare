using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloak
{
    MovementScript player;
    public Cloak() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
