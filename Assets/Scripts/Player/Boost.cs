using UnityEngine;

public class Boost
{
    MovementScript player;
    public Boost() {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }

    public void StartBoosting() {
        //If the player is sliding, take them out of it
        if (player.sliding) {
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
    }

    public void WhileBoosting() {
        player.spriteRenderer.color = Color.red;
        //Sets the RTPC Value of horizontalVelocity to the horizontalVelocity float value.
        AkSoundEngine.SetRTPCValue("horizontalVelocity", player.horizontalVelocity);
        if (player.boostCharge - player.boostRecharge * Time.deltaTime > 0) {
            player.boostCharge -= player.boostRecharge * Time.deltaTime;
        } else {
            player.boostCharge = 0;
        }
        player.effectiveAcceleration = player.boostAcceleration;
        player.boostingMaxRunSpeedMultiplier = player.boostMaxRunSpeedMultiplier;
    }

    public void NotBoosting() {
        player.spriteRenderer.color = Color.white;
        if (player.boostCharge + player.boostDepletion * Time.deltaTime < 100f) {
            player.boostCharge += player.boostDepletion * Time.deltaTime;
        } else {
            player.boostCharge = 100f;
        }
        player.boostingMaxRunSpeedMultiplier = 1;
        player.effectiveAcceleration = player.acceleration;
    }

}
