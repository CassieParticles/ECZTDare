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
        //Change player colour, while respecting regular alpha value
        Color color = Color.red;
        color.a = player.spriteRenderer.color.a;
        player.spriteRenderer.color = color;
        //Sets the RTPC Value of horizontalVelocity to the horizontalVelocity float value.
        AkSoundEngine.SetRTPCValue("horizontalVelocity", player.horizontalVelocity);
        if (player.batteryCharge - player.boostDepletion * Time.deltaTime > 0) {
            player.batteryCharge -= player.boostDepletion * Time.deltaTime;
        } else {
            player.batteryCharge = 0;
        }
        player.effectiveAcceleration = player.boostAcceleration;
        player.boostingMaxRunSpeedMultiplier = player.boostMaxRunSpeedMultiplier;
    }

    public void NotBoosting() {
        //Change player colour, while respecting regular alpha value
        Color color = Color.white;
        color.a = player.spriteRenderer.color.a;
        player.spriteRenderer.color = color;
        if (player.batteryCharge + player.boostRecharge * Time.deltaTime < 100f) {
            player.batteryCharge += player.boostRecharge * Time.deltaTime;
        } else {
            player.batteryCharge = 100f;
        }
        player.boostingMaxRunSpeedMultiplier = 1;
        //Will never be able to boost while in stealth mode, so can make it be the movement acceleration every time
        player.effectiveAcceleration = player.acceleration;
    }

}
