using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        if (player.horizontalVelocity < 0.1) {
            player.rb.velocityX = 0;
        }
    }

    //Decide what the max velocity is and cap the player if necessary
    public void CapRunningSpeed() {
        //If the player is falling, slow them down slightly
        if (player.rb.velocityY < 0) {
            player.dynamicMaxRunSpeed = player.maxRunSpeed * //Base max run speed
                                 player.boostingMaxRunSpeedMultiplier * //Boosting makes this multiplier not 1
                                 (1 - (player.fallSlowsRunMult * -player.rb.velocityY / player.maxFallSpeed)); //Falling slows down the horizontal speed
        } else { //Otherwise, calculate the max run speed as normal
            player.dynamicMaxRunSpeed = player.maxRunSpeed * player.boostingMaxRunSpeedMultiplier;
        }
        //Only cap the running speed if the player is not sliding
        if (player.horizontalVelocity > player.dynamicMaxRunSpeed && !player.sliding) {
            player.rb.velocityX -= (player.rb.velocityX - (player.dynamicMaxRunSpeed * Mathf.Sign(player.rb.velocityX))) * player.snapToMaxRunSpeedMult / 10; //Sets the speed to maxRunSpeed
        }
    }

    public void FootstepSounds() {
        if (player.horizontalVelocity > 0.1 && player.grounded) {
            player.footstepCount += (player.horizontalVelocity * player.footstepRateScaler) * player.footstepRate * Time.deltaTime;
            if (player.horizontalVelocity < 20f) {
                if (player.footstepCount > 1) {
                    player.footstepRate = 0.1f;
                    player.playerFootstep.Post(player.gameObject);
                    player.footstepCount--;
                    //Alert noise
                    if (player.boosting) {
                        AudioDetectionSystem.getAudioSystem().PlaySound(player.transform.position, player.boostFootStepSoundRange, player.boostFootStepSoundSuspicionIncrease);
                    }
                }
            } else if (player.horizontalVelocity >= 20f) {
                if (player.footstepCount > 1) {
                    player.footstepRate = 0.04f;
                    player.playerFootstep.Post(player.gameObject);
                    player.footstepCount--;
                    //Alert noise
                    if (player.boosting) {
                        AudioDetectionSystem.getAudioSystem().PlaySound(player.transform.position, player.boostFootStepSoundRange, player.boostFootStepSoundSuspicionIncrease);
                    }
                }
            }
        }
    }
}
