using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Find the death wall, and destroy it
        DeathWall wall = FindAnyObjectByType<DeathWall>();
        if (!wall){ return; }

        ////NEEDS FIXED////
        //Stops the audio
        wall.deathWall.Stop(gameObject);
        //Sets the "Music" State Group's active State to "Hidden"
        //AkSoundEngine.SetState("Music", "Hidden");

        Destroy(wall.gameObject);

        FindAnyObjectByType<DeathwallRespawn>().DeathWallStop();
    }
}
