using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost
{
    MovementScript player;
    public Boost() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }

    public void StartBoosting() {
        player.boosting = true;
        player.hasBoosted = true;

        //Plays the boost sfx
        player.boostStart.Post(player.gameObject);
        player.boostRush.Post(player.gameObject);
    }

    public void StopBoosting() {

    }

    public void WhileBoosting() {

    }


}
