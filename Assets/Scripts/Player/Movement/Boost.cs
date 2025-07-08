using System;
using System.Collections;
using UnityEngine;

public class Boost
{
    MovementScript player;
    int dashDir;
    public Boost() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }

    public void StartBoosting() {
        //If the player is sliding, take them out of it if possible
        if (player.sliding) {
            if (!player.canEndSlide) {
                return;
            }
            //Stops the slide sound.
            player.playerSlide.Stop(player.gameObject);

            player.sliding = false;
            player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + player.colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
            player.collider.size = player.colliderSize;
            player.effectiveDeceleration = player.deceleration;
        }

        player.boosting = true;
        player.hasBoostCloaked = true;

        //Plays the boost sfx
        player.boostStart.Post(player.gameObject);
        player.boostRush.Post(player.gameObject);
    }

    public void StopBoosting() {
        //If the player was boosting before, end the boost sounds
        if (player.boosting) {
            player.boostRush.Stop(player.gameObject);
            //Plays the boost stop sound.
            player.boostStop.Post(player.gameObject);
        }
        player.boosting = false;
        //Change player colour, while respecting regular alpha value
        //Color color = Color.white;
        //color.a = player.spriteRenderer.color.a;
        //player.spriteRenderer.color = color;
        player.boostingMaxRunSpeedMultiplier = 1;
        //Will never be able to boost while in stealth mode, so can make it be the movement acceleration every time
        player.effectiveAcceleration = player.acceleration;
    }

    public void WhileBoosting() {
        //Change player colour, while respecting regular alpha value
        //Color color = Color.red;
        //color.a = player.spriteRenderer.color.a;
        //player.spriteRenderer.color = color;
        //Sets the RTPC Value of horizontalVelocity to the horizontalVelocity float value.
        AkSoundEngine.SetRTPCValue("horizontalVelocity", player.horizontalVelocity);
        if (player.batteryCharge - player.boostDepletion * Time.deltaTime > 0) {
            player.batteryCharge -= player.boostDepletion * Time.deltaTime;
        } else {
            player.batteryCharge = 0;
            StopBoosting();
        }
        player.effectiveAcceleration = player.boostAcceleration;
        player.boostingMaxRunSpeedMultiplier = player.boostMaxRunSpeedMultiplier;
    }

    public void NotBoosting() {
        
        if (player.batteryCharge + player.boostRecharge * Time.deltaTime < 100f) {
            player.batteryCharge += player.boostRecharge * Time.deltaTime;
        } else {
            player.batteryCharge = 100f;
        }
    }

    public void StartDashing() {
        //If the player is sliding, take them out of it if possible
        if (player.sliding) {
            if (!player.canEndSlide) {
                return;
            }
            //Stops the slide sound.
            player.playerSlide.Stop(player.gameObject);

            player.sliding = false;
            player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + player.colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
            player.collider.size = player.colliderSize;
            player.effectiveDeceleration = player.deceleration;
        }
        player.dashing = true;
        player.rb.gravityScale = 0f;
        dashDir = player.runInput == 0 ? (Convert.ToInt32(player.facingRight) * 2 - 1) : player.runInput; //Use the inputted direction, or the facing direction if no inputted direction exists
        player.rb.velocity = new Vector2(dashDir * player.dashSpeed, 0);
        player.batteryCharge -= player.dashBatteryCost;
        player.dashChargesRemaining--;
        player.hasBoostCloaked = true;
        player.InputLocked = true;
        player.StartCoroutine(WhileDashing());
    }

    public IEnumerator WhileDashing() {
        float timeElapsed = 0f;
        while (timeElapsed < player.dashDuration) {
            timeElapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        StopDashing();
    }

    public void StopDashing() {
        player.dashing = false;
        player.rb.gravityScale = 1f;
        player.InputLocked = false;
        player.StartCoroutine(DashCooldown());
        //Debug.Log("dashDir = " + dashDir + ", runInput = " + player.runInput);
        //if (player.runInput == -dashDir) {
        //    player.rb.velocityX = 0f;
        //}
    }

    public IEnumerator DashCooldown() {
        player.dashCooldownActive = true;
        float timeElapsed = 0f;
        while (timeElapsed < player.dashCooldown) {
            timeElapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        player.dashCooldownActive = false;
    }
}
