using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{

    private Rigidbody2D rb;
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

    [SerializeField] private float runSpeed = 8;
    [SerializeField] private float acceleration = 20;
    [SerializeField] private float deceleration = 15;
    [SerializeField] private float jumpStrength = 5;
    [SerializeField] private float fallActivationSpeed = 1; //At what vertical speed the higher gravity kicks in at
    [SerializeField] private float fallMult = 2;
    [SerializeField] private float maxFallSpeed = 5;

    private bool grounded; //Grounded can be either on the floor or on the wall in theory

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        JumpAndFall();
        WalkRun();
    }

    void CheckGrounded() {
        
    }

    void JumpAndFall() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.velocityY = jumpStrength;
        }
        if (rb.velocityY < fallActivationSpeed) {
            rb.velocityY += (fallMult - 1) * Physics2D.gravity.y * Time.deltaTime; //fallmult - 1 since gravity gets applied by default
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
            rb.velocityX += -acceleration * Time.deltaTime;
            if (Mathf.Sign(rb.velocityX) == 1) {
                rb.velocityX += deceleration * -rb.velocityX * Time.deltaTime;
            }
        } else if (Input.GetKey(KeyCode.D)) {
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
        if (Mathf.Abs(rb.velocityX) > runSpeed) {
            rb.velocityX = runSpeed * Mathf.Sign(rb.velocityX); //Sets the speed to either runSpeed or -runSpeed
        }
    }
}
