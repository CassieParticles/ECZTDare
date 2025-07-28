using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    DeathWall deathWallScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Find the death wall, and destroy it
        DeathWall wall = FindAnyObjectByType<DeathWall>();
        if (!wall){ return; }

        ////NEEDS FIXED////
        //Sets the "Music" State Group's active State to "Hidden"
        //AkSoundEngine.SetState("Music", "Hidden");
        //Stops the audio
        ///deathWallScript.deathWall.Stop(gameObject);

        Destroy(wall.gameObject);

        FindAnyObjectByType<DeathwallRespawn>().DeathWallStop();
    }
}
