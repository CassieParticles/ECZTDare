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
        if (player.boosting) {
            AudioDetectionSystem.getAudioSystem().PlaySound(player.transform.position, player.boostSlideSoundRange, player.boostSlideSoundSuspicionIncrease, AudioSource.Player);
        }
        player.sliding = true;
        player.hasSlid = true;
        player.collider.size = new Vector2(player.colliderSize.x * 1.5f, player.colliderSize.y * 0.3f);
        player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - player.colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
        player.effectiveDeceleration = player.slideDeceleration;
        player.tempGroundedTimer = 0.02f;
    }
    public void StopSliding() {
        //Stops the slide sound.
        player.playerSlide.Stop(player.gameObject);
        player.sliding = false;
        player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + player.colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
        player.collider.size = player.colliderSize;
        if (player.inStealthMode) {
            player.effectiveDeceleration = player.stealthDeceleration;
        } else {
            player.effectiveDeceleration = player.deceleration;
        }
    }
    public void WhileSliding() {
        //Sets the RTPC Value of horizontalVelocity to the horizontalVelocity float value.
        AkSoundEngine.SetRTPCValue("horizontalVelocity", player.horizontalVelocity);
    }

    public void CanStopSliding() {

    }
}
