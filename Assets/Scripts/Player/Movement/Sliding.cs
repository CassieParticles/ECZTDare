using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Sliding
{
    MovementScript player;
    public Sliding() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }

    public void StartSliding() {
        //Plays the slide sound.
        player.playerSlide.Post(player.gameObject);
        //if (player.boosting) {
        //    AudioDetectionSystem.getAudioSystem().PlaySound(player.transform.position, player.boostSlideSoundRange, player.boostSlideSoundSuspicionIncrease, AudioSource.Player);
        //}
        player.sliding = true;
        player.playerCollider.size = new Vector2(player.colliderSize.x * 1.5f, player.colliderSize.y * 0.3f);
        player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - player.colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
        player.rb.velocityY = 0f;
        player.effectiveDeceleration = player.slideDeceleration;
        player.tempGroundedTimer = 0.02f;
    }
    public void StandUp() {
        //Stops the slide sound.
        player.playerSlide.Stop(player.gameObject);
        player.sliding = false;
        player.crouching = false;
        player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + player.colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
        player.playerCollider.size = player.colliderSize;
        player.effectiveDeceleration = player.deceleration;
        player.effectiveAcceleration = player.acceleration;
        player.effectiveMaxRunSpeed = player.maxRunSpeed;
    }
    public void WhileSliding() {
        //Sets the RTPC Value of horizontalVelocity to the horizontalVelocity float value.
        AkSoundEngine.SetRTPCValue("horizontalVelocity", player.horizontalVelocity);
    }

    public void Crouch() {
        player.crouching = true;
        player.animator.SetBool("Crouching", true);
        player.playerCollider.size = new Vector2(player.colliderSize.x * 1.5f, player.colliderSize.y * 0.3f);
        player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - player.colliderSize.y * 0.31f); //Lower the player so they arent midair when crouching
        player.rb.velocityY = 0f;
        player.effectiveDeceleration = player.crouchDeceleration;
        player.effectiveAcceleration = player.crouchAcceleration;
        player.effectiveMaxRunSpeed = player.maxCrouchSpeed;
        player.tempGroundedTimer = 0.02f;
    }

    public void SlideToCrouch() {
        player.playerSlide.Stop(player.gameObject);
        player.sliding = false;
        player.crouching = true;
        player.animator.SetBool("Crouching", true);
        player.effectiveDeceleration = player.crouchDeceleration;
        player.effectiveAcceleration = player.crouchAcceleration;
        player.effectiveMaxRunSpeed = player.maxCrouchSpeed;
    }
}
