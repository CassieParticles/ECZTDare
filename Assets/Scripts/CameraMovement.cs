using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector2 targetPos;
    [NonSerialized] public bool isBeingPulled;
    [NonSerialized] public Vector2 pulledTargetPos;
    [NonSerialized] public float pulledExponentX;
    [NonSerialized] public float pulledExponentY;

    private float smoothTurning = 0f;
    private MovementScript player;
    private Rigidbody2D rb;

    [SerializeField] private float turningMult = 2f; //Multiplier for how quick the camera moves between facing left and right
    [SerializeField] private float exponentX = 2f; //How quickly the camera speeds up with distance horizontally
    [SerializeField] private float exponentY = 2f; //How quickly the camera speeds up with distance vertically
    [SerializeField] private float directionalOffset = 2f; //How far in front of the player the camera settles at
    [SerializeField] private float runningOffsetMult = 1f; //Multiplies how much the player's velocity affects the cameras position, letting it settle in front of the player
    [SerializeField] private float verticalOffset = 2f; //How far above the player the camera settles at
    [SerializeField] private float fallingOffsetMult = 0.1f; //Multiplies how much of the player's falling velocity affects camera position, allowing the player to see below them when falling.
    [SerializeField] private float fallingOffsetThreshold = -2f; //The vertical velocity the player needs to have for the camera to start taking it into the calculations
    

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the camera isnt being pulled calculate the velocity as normal
        if (!isBeingPulled) { 
            //Changes the variable that controls the target position when the player is turning around
            int playerDir = Convert.ToInt32(player.facingRight) * 2 - 1;
            if (MathF.Abs(smoothTurning + playerDir * 0.1f) <= 1f) {
                smoothTurning += playerDir * 0.1f * turningMult;
            } 

            //Sets the variable for checking if the player is falling
            int isPlayerFalling = 0;
            if (player.rb.velocity.y < fallingOffsetThreshold) {
                isPlayerFalling = 1;
            }

            //Calculates the target position
            targetPos = player.rb.position //Players position
                        + Vector2.right * smoothTurning * directionalOffset //Directional offset in the direction the player is facing
                        + Vector2.up * verticalOffset //Vertical offset so that the player is higher or lower on the screen
                        + Vector2.right * player.rb.velocityX * runningOffsetMult * 0.1f //Running multiplier to offset so that the camera keeps up
                        + Vector2.up * isPlayerFalling * (player.rb.velocityY - fallingOffsetThreshold) * fallingOffsetMult; //Falling multiplier to offset so that the camera keeps up

            //Snaps the position of the camera to the target position if close enough
            if (Vector2.Distance(rb.position, targetPos) < 0.1f) {
                rb.position = targetPos;
            } else { //Otherwise calculate the velocity of the camera
                Vector2 velocityVector = targetPos - rb.position;
                rb.velocity = new Vector2(Mathf.Sign(velocityVector.x) * Mathf.Pow(Mathf.Abs(velocityVector.x), exponentX),
                                          Mathf.Sign(velocityVector.y) * Mathf.Pow(Mathf.Abs(velocityVector.y), exponentY));
            }
        } else { //If the camera is being pulled
            //Snaps the position of the camera to the target position if close enough
            if (Vector2.Distance(rb.position, targetPos) < 0.1f) {
                rb.position = pulledTargetPos;
            } else { //Otherwise calculate the velocity of the camera
                Vector2 velocityVector = pulledTargetPos - rb.position;
                rb.velocity = new Vector2(Mathf.Sign(velocityVector.x) * Mathf.Pow(Mathf.Abs(velocityVector.x), exponentX),
                                          Mathf.Sign(velocityVector.y) * Mathf.Pow(Mathf.Abs(velocityVector.y), exponentY));
            }
        }
    }
}
