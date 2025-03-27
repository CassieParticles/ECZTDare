using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;


public class StartCutScene : MonoBehaviour
{
    PlayableDirector director;
    MovementScript player;
    private AlarmMusicHandler gameMusicScript;
    bool waitForGrounded = false;
    bool cutscenePlayed = false;

    private void Awake()
    {
        director = GameObject.Find("Director").GetComponent<PlayableDirector>();
    }

    private void FixedUpdate()
    {
        if (waitForGrounded && player.grounded && !cutscenePlayed)
        {
            //Start cutscene
            //Sets the "Music" State Group's active State to "Menu"
            AkSoundEngine.SetState("Music", "Menu");
            director.Play();
            cutscenePlayed = true;
        }
    }

    public void DoStuff()
    {
        director.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cutscenePlayed){ return; }
        MovementScript playerScript = collision.GetComponent<MovementScript>();
        
        if(playerScript)
        {
            Debug.Log("Collider hit");
            playerScript.InputLocked=true;
            playerScript.GetComponent<Rigidbody2D>().velocityX = 0;
            //Wait for player to be grounded
            waitForGrounded = true;
            player = playerScript;
            player.transform.parent = transform.parent;

            //Make player face right
            player.facingRight= true;
        }
    }
}
