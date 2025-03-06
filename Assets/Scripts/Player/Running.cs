using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running
{
    MovementScript player;
    public Running() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }

    public void Accelerate(int runInput) {
        player.facingRight = Convert.ToBoolean((runInput + 1) / 2);
        player.rb.velocityX += runInput * player.acceleration * Time.deltaTime;
        if (Mathf.Sign(player.rb.velocityX) == runInput * -1) {
            player.rb.velocityX += player.effectiveDeceleration * -player.rb.velocityX * Time.deltaTime;
        }
    }

    public void Decelerate() {
        player.rb.velocityX += player.effectiveDeceleration * -player.rb.velocityX * Time.deltaTime; //Decelerate when not holding left or right
        if (Mathf.Abs(player.rb.velocityX) < 0.1) {
            player.rb.velocityX = 0;
        }
    }
}
