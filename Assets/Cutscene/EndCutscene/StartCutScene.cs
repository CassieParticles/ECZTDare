using UnityEngine;
using UnityEngine.Playables;


public class StartCutScene : MonoBehaviour
{
    [SerializeField]PlayableDirector director;
    MovementScript player;
    bool waitForGrounded = false;

    private void Awake()
    {
        director = GameObject.Find("Director").GetComponent<PlayableDirector>();
    }

    private void FixedUpdate()
    {
        if(waitForGrounded && player.grounded)
        {
            //Start cutscene

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MovementScript playerScript = collision.GetComponent<MovementScript>();
        if(playerScript)
        {
            playerScript.inputLocked=true;
            playerScript.GetComponent<Rigidbody2D>().velocityX = 0;
            //Wait for player to be grounded
            waitForGrounded = true;
            player = playerScript;
        }
    }
}
