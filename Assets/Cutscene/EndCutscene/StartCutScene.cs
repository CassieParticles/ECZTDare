using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class StartCutScene : MonoBehaviour
{
    PlayableDirector director;
    MovementScript player;
    private AlarmMusicHandler gameMusicScript;
    bool waitForGrounded = false;
    bool cutscenePlayed = false;

    bool canLeave;

    private void Awake()
    {
        director = GameObject.Find("Director").GetComponent<PlayableDirector>();
        canLeave = false;
    }

    private void FixedUpdate()
    {
        if (waitForGrounded && player.grounded && !cutscenePlayed)
        {
            //Start cutscene
            //Sets the "Music" State Group's active State to "Cutscene"
            AkSoundEngine.SetState("Music", "Cutscene");
            director.Play();
            cutscenePlayed = true;
        }

        if(canLeave && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("I'm outa here!");

            MenuScript menuScript = FindAnyObjectByType<MenuScript>();
            if(menuScript)
            {
                menuScript.ChangeScene("Main Menu");
            }

            canLeave = false;
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

    public void CanLeave()
    {
        canLeave = true;
    }
}
