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
    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float acceleration = 2;
    [SerializeField] private float deceleration = 2;
    [SerializeField] private float jumpStrength = 5;
    [SerializeField] private float fallMult = 2;

    private bool grounded; //Grounded can be either on the floor or on the wall in theory

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        JumpAndFall();
        WalkRun();
    }

    void JumpAndFall() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.velocityY = jumpStrength;
        }
        if (rb.velocityY < 0) {
            rb.velocityY += (fallMult - 1) * Physics2D.gravity.y * Time.deltaTime; //fallmult - 1 since gravity gets applied by default
        } else if (rb.velocityY > 0 && !Input.GetKey(KeyCode.Space)) {
            rb.velocityY += (fallMult - 1) * Physics2D.gravity.y * Time.deltaTime; //fallmult - 1 since gravity gets applied by default
        }
    }

    void WalkRun() {
        if (Input.GetKey(KeyCode.A)) {
            rb.velocityX += -acceleration; 
        } else if (Input.GetKey(KeyCode.D)) {
            rb.velocityX += acceleration;
        }
        if (Mathf.Abs(rb.velocityX) > runSpeed) {
            rb.velocityX = runSpeed * rb.velocityX / Mathf.Abs(rb.velocityX); //Sets the speed to either runSpeed or -runSpeed
        }
    }
}
