using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Jumping
{
    MovementScript player;
    public Jumping() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }

    public void BasicJump() {
        player.rb.velocityY = player.jumpStrength;
        //Plays the Player_Jump sound
        AkSoundEngine.PostEvent("Player_Jump", player.gameObject);
        if (player.boosting) {
            AudioDetectionSystem.getAudioSystem().PlaySound(player.transform.position, player.boostJumpSoundRange, player.boostJumpSoundSuspicionIncrease);
        }
        player.animator.SetBool("Grounded", false);
        player.hasJumped = true;
        player.StartCoroutine(player.MinJumpDuration());
    }

    public void WallJump() {
        int whichWallJump = Convert.ToInt32(player.onRightWall) * 2 - 1;
        player.hasJumped = true;
        player.facingRight = !player.onRightWall;
        if (whichWallJump == -1 && player.jumpedOffWall != -1) { //Jumping off a left wall
            player.rb.velocityX = player.horizontalWalljumpStrength;
            player.rb.velocityY = player.verticalWalljumpStrength;
            player.jumpedOffWall = -1;
            //Plays the Player_Jump sound
            AkSoundEngine.PostEvent("Player_Jump", player.gameObject);
            player.StartCoroutine("WalljumpInputDelay", -1);
        } else if (whichWallJump == 1 && player.jumpedOffWall != 1) { //Jumping off a right wall
            player.rb.velocityX = -player.horizontalWalljumpStrength;
            player.rb.velocityY = player.verticalWalljumpStrength;
            player.jumpedOffWall = 1;
            //Plays the Player_Jump sound
            AkSoundEngine.PostEvent("Player_Jump", player.gameObject);
            player.StartCoroutine("WalljumpInputDelay", 1);
        }
    }
}
