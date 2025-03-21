using UnityEngine;
using UnityEngine.Playables;


public class StartCutScene : MonoBehaviour
{
    PlayableDirector director;
    MovementScript player;
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
            player.transform.parent = GameObject.Find("Cutscene").transform;

            //Make player face right
            player.facingRight= true;
        }
    }
}
