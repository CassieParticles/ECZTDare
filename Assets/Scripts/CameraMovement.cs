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
    [SerializeField] float verticalOffset = 2f; //How far above the player the camera settles at

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
        targetPos = player.rb.position + (Vector2.right * smoothTurning * directionalOffset) + Vector2.up * verticalOffset;

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
