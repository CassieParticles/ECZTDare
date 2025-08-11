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

    public int StartDashing() {
        //If the player is sliding, take them out of it if possible
        if (player.sliding || player.crouching) {
            if (!player.canStandUp) {
                return 0; //Dash doesnt happen
            }
            player.slideScript.StandUp();
        }
        //Plays the Player_Dash sound
        AkSoundEngine.PostEvent("Player_Dash", player.gameObject);
        player.dashing = true;
        player.animator.SetBool("Dashing", true); //>>>>>>>>Mark Addition<<<<<<<<<<
        player.rb.gravityScale = 0f;
        if (!player.onWall) {
            dashDir = player.runInput == 0 ? (Convert.ToInt32(player.facingRight) * 2 - 1) : player.runInput; //Use the inputted direction, or the facing direction if no inputted direction exists
        } else {
            dashDir = -(Convert.ToInt32(player.onRightWall) * 2 - 1);
            player.facingRight = !player.onRightWall;
        }
        player.rb.velocity = new Vector2(dashDir * player.dashSpeed, 0);
        player.batteryCharge -= player.dashBatteryCost;
        player.dashChargesRemaining--;
        player.hasDashed = true;
        player.InputLocked = true;
        
        player.StartCoroutine(WhileDashing());
        return dashDir;
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
        player.animator.SetBool("Dashing", false); //>>>>>>>>Mark Addition<<<<<<<<<<
        //Make sure you cant dash past cutscenes
        if (GameObject.FindAnyObjectByType<MenuScript>().currentCameraPan == null) {
            
            player.InputLocked = false;
        }
        
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
