using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{

    [NonSerialized] public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float maxRunSpeed = 8;
    [SerializeField] private float acceleration = 20; //Speeding up when running
    [SerializeField] private float deceleration = 15; //Slowing down when no longer running / running in opposite direction
    [SerializeField] private float jumpStrength = 5; //Initial vertical velocity when jumping
    [SerializeField] private float coyoteTime = 0.12f; //Time in seconds that the player can jump after walking off a ledge
    [SerializeField] private float fastFallActivationSpeed = 1; //At what vertical speed the fast fall kicks in at
    [SerializeField] private float fastFallMult = 2; //Fast fall multiplier
    [SerializeField] private float maxFallSpeed = 5; //Needs to be higher if fastfallmult is higher also

    [NonSerialized] public bool grounded; //Grounded is only for the ground, a seperate one will be used for walls
    [NonSerialized] public bool facingRight;

    private LayerMask layers;

    //PlayerControls controls;

    //public void OnEnable() {
    //    if (controls == null) {
    //        controls = new PlayerControls();
    //        // Tell the "gameplay" action map that we want to get told about
    //        // when actions get triggered.
    //        controls.Gameplay.SetCallbacks();
    //    }
    //    controls.Gameplay.Enable();
    //}

    private void Awake() {
        layers = new LayerMask();
        layers = 0b0110011;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        CheckGrounded();
        JumpAndFall();
        WalkRun();
    }

    void CheckGrounded() {
        Vector2 rightRayStart = transform.position + new Vector3(transform.localScale.x * 0.99f / 2f,
                                                                -transform.localScale.y * 0.99f / 2f);
        Vector2 leftRayStart = transform.position + new Vector3(-transform.localScale.x * 0.99f / 2f,
                                                                -transform.localScale.y * 0.99f / 2f);
        if (Physics2D.Raycast(rightRayStart, Vector2.down, 0.1f, layers) ||
        Physics2D.Raycast(leftRayStart, Vector2.down, 0.1f, layers)) {
            grounded = true;
        } else {
            grounded = false;
        }
    }

    void JumpAndFall() {
        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            rb.velocityY = jumpStrength;
        }
        if (rb.velocityY < fastFallActivationSpeed) {
            rb.velocityY += (fastFallMult - 1) * Physics2D.gravity.y * Time.deltaTime; //fallmult - 1 since gravity gets applied by default
            if (rb.velocityY < -maxFallSpeed) { //Less than because its negative
                rb.velocityY = -maxFallSpeed;
            }
        } 
        //else if (rb.velocityY > 0 && !Input.GetKey(KeyCode.Space)) {
        //    rb.velocityY += (fallMult - 1) * Physics2D.gravity.y * Time.deltaTime; //fallmult - 1 since gravity gets applied by default
        //}
    }

    void WalkRun() {
        if (Input.GetKey(KeyCode.A)) {
            facingRight = false;
            rb.velocityX += -acceleration * Time.deltaTime;
            if (Mathf.Sign(rb.velocityX) == 1) {
                rb.velocityX += deceleration * -rb.velocityX * Time.deltaTime;
            }
        } else if (Input.GetKey(KeyCode.D)) {
            facingRight = true;
            rb.velocityX += acceleration * Time.deltaTime;
            if (Mathf.Sign(rb.velocityX) == -1) {
                rb.velocityX += deceleration * -rb.velocityX * Time.deltaTime;
            }
        } else if (rb.velocityX != 0){
            rb.velocityX += deceleration * -rb.velocityX * Time.deltaTime; //Decelerate when not holding left or right
            if (Mathf.Abs(rb.velocityX) < 0.1) {
                rb.velocityX = 0;
            }
        }
        if (Mathf.Abs(rb.velocityX) > maxRunSpeed) {
            rb.velocityX = maxRunSpeed * Mathf.Sign(rb.velocityX); //Sets the speed to either runSpeed or -runSpeed
        }
        spriteRenderer.flipX = !facingRight;
    }
}
