using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CappingRunSpeed
{
    MovementScript player;
    public CappingRunSpeed() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }
}
