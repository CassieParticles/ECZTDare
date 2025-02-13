using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector2 targetPos;
    private float smoothTurning = 0f;
    private MovementScript player;
    private Rigidbody2D rb;

    [SerializeField] float turningMult = 2f; //Multiplier for how quick the camera moves between facing left and right
    [SerializeField] float exponentX = 2f; //How quickly the camera speeds up with distance horizontally
    [SerializeField] float exponentY = 2f; //How quickly the camera speeds up with distance vertically
    [SerializeField] float directionalOffset = 2f; //How far in front of the player the camera settles at
    [SerializeField] float runningOffsetMult = 1f; //Multiplies how much the player's velocity affects the cameras position, letting it settle in front of the player
    [SerializeField] float verticalOffset = 2f; //How far above the player the camera settles at
    [SerializeField] float fallingOffsetMult = 0.1f; //Multiplies how much of the player's falling velocity affects camera position, allowing the player to see below them when falling.
    [SerializeField] float fallingOffsetThreshold = -2f; //The vertical velocity the player needs to have for the camera to start taking it into the calculations
    

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int playerDir = Convert.ToInt32(player.facingRight) * 2 - 1;
        if (MathF.Abs(smoothTurning + playerDir * 0.1f) <= 1f) {
            smoothTurning += playerDir * 0.1f * turningMult;

        } 

        int isPlayerFalling = 0;
        if (player.rb.velocity.y < fallingOffsetThreshold) {
            isPlayerFalling = 1;
        }

        targetPos = player.rb.position
                    + Vector2.right * smoothTurning * directionalOffset
                    + Vector2.up * verticalOffset
                    + Vector2.right * player.rb.velocityX * runningOffsetMult * 0.1f
                    + Vector2.up * isPlayerFalling * (player.rb.velocityY - fallingOffsetThreshold) * fallingOffsetMult;


        //rb.velocity += (targetPos - rb.position) * 0.5f;
        if (Vector2.Distance(rb.position, targetPos) < 0.1f) {
            rb.position = targetPos;
        } else {
            Vector2 velocityVector = targetPos - rb.position;
            rb.velocity = new Vector2(Mathf.Sign(velocityVector.x) * Mathf.Pow(Mathf.Abs(velocityVector.x), exponentX),
                                      Mathf.Sign(velocityVector.y) * Mathf.Pow(Mathf.Abs(velocityVector.y), exponentY));
        }
    }
}
