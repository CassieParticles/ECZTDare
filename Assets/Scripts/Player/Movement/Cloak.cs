using UnityEngine;

public class Cloak
{
    MovementScript player;

    private bool cloaked;

    public Cloak() 
    {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
    }
    /// <summary>
    /// Called the frame that cloaking starts
    /// </summary>
    public void Enable()
    {
        player.cloakStart.Post(player.gameObject);
        player.cloaking = true;
        player.hasBoostCloaked = true;
        //Make player transparent
        Color color = player.GetComponent<SpriteRenderer>().color;
        color.a = 0.2f;
        player.GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// Called the frame that cloaking stops
    /// </summary>
    public void Disable()
    {
        player.cloakStop.Post(player.gameObject);
        player.cloaking = false;
        Color color = player.GetComponent<SpriteRenderer>().color;
        color.a = 1.0f;
        player.GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// Called every tick in which the cloaking is happening
    /// </summary>
    public void OnTick()
    {
        if (player.batteryCharge - player.cloakDepletion * Time.deltaTime > 0) {
            player.batteryCharge -= player.cloakDepletion * Time.deltaTime;
        } else {
            player.batteryCharge = 0;
        }
    }
}
