using System;
using System.Collections;
using UnityEngine;

public class Dash
{
    MovementScript player;
    int dashDir;
    public Dash() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
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
        player.hasDashed = true;
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
