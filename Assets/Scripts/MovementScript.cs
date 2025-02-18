using System;
using UnityEngine;


public class MovementScript : MonoBehaviour
{
    public AK.Wwise.Event playerFootstep;

    //The speed at which footstep sounds are triggered. Whenever footstepRate is 1 a footstep is played
	[SerializeField][Range(0.01f, 3.0f)] private float footstepRate = 1f;

    //How much the velocity of the player affects the footstep frequency
	[SerializeField][Range(0.01f, 3.0f)] private float footstepRateScaler = 1f;

    //Used to determine when to trigger footstep sounds.
    private float footstepCount = 0.0f;

    //Cooldown for playing the landing sound effect in seconds
    private float landingCooldown = 0.5f;

    //The highest velocity the player can reach, affected by the serialized value as well as other factors
    private float dynamicMaxRunSpeed = 0;

    [NonSerialized] public Rigidbody2D rb;
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float maxRunSpeed = 8; //The fastest the player can go horizontally
    [SerializeField] private float acceleration = 20; //Speeding up when running
    [SerializeField] private float deceleration = 15; //Slowing down when no longer running / running in opposite direction
    [SerializeField] private float snapToMaxRunSpeedMult = 1f; //How quickly the player snaps back to max running speed when running faster than it
    [SerializeField] private float jumpStrength = 5; //Initial vertical velocity when jumping
    [SerializeField] private float coyoteTime = 0.12f; //Time in seconds that the player can jump after walking off a ledge
    [SerializeField] private float gravityMult = 1; //Gravity multiplier when not fastfalling
    [SerializeField] private float fastFallActivationSpeed = 1; //At what vertical speed the fast fall kicks in at
    [SerializeField] private float fastFallMult = 2; //Fast fall multiplier
    [SerializeField] private float maxFallSpeed = 5; //Needs to be higher if fastfallmult is higher also
    [SerializeField][Range(0.01f, 1f)] private float fallSlowsRunMult = 1; //Multiplier for how much falling speed slows down horizontal speed.

    [NonSerialized] public bool grounded; //Grounded is only for the ground, a seperate one will be used for walls
    [NonSerialized] public bool facingRight;

    private LayerMask layers;

    AlarmSystem alarm;

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
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        alarm = GameObject.Find("AlarmObject").GetComponent<AlarmSystem>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        CheckGrounded();
        JumpAndFall();
        WalkRun();

        
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocityX));
        animator.SetFloat("yVelocity", rb.velocityY);

        if (Input.GetKey(KeyCode.L))
        {
            alarm.StopAlarm();
        }
    }

    void CheckGrounded() {
        if (landingCooldown > 0) {
            landingCooldown -= Time.deltaTime;
        }

        Vector2 rightRayStart = rb.position + collider.offset + new Vector2(collider.size.x * 0.99f / 2f,
                                                                           -collider.size.y * 0.99f / 2f);
        Vector2 leftRayStart = rb.position + collider.offset + new Vector2(-collider.size.x * 0.99f / 2f,
                                                                           -collider.size.y * 0.99f / 2f);

        if (Physics2D.Raycast(rightRayStart, Vector2.down, 0.1f, layers) ||
        Physics2D.Raycast(leftRayStart, Vector2.down, 0.1f, layers)) {
            if (!grounded && landingCooldown <= 0) {
                //Plays the Player_Land sound if the player was not grounded last frame and it isnt on cooldown
                landingCooldown = 0.1f;
                
                AkSoundEngine.PostEvent("Player_Land", this.gameObject);
            }
            grounded = true;
        } else {
            grounded = false;
        }
        animator.SetBool("Grounded", grounded);
    }

    void JumpAndFall() {
        if (Input.GetKey(KeyCode.Space) && grounded) {
            rb.velocityY = jumpStrength;
            //Plays the Player_Jump sound
            AkSoundEngine.PostEvent("Player_Jump", this.gameObject);
            animator.SetBool("Grounded", false);
        }
        if (rb.velocityY < fastFallActivationSpeed || !Input.GetKey(KeyCode.Space)) {
            rb.velocityY += (fastFallMult - 1) * Physics2D.gravity.y * Time.deltaTime; //fallmult - 1 since gravity gets applied by default
            if (rb.velocityY < -maxFallSpeed) { //Less than because its negative
                rb.velocityY = -maxFallSpeed;
            }
        } else {
            rb.velocityY += (gravityMult - 1) * Physics2D.gravity.y * Time.deltaTime;
        }
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
        if (rb.velocityY < 0) {
            dynamicMaxRunSpeed = maxRunSpeed *
                                 (1 - (fallSlowsRunMult * -rb.velocityY / maxFallSpeed)); //Falling slows down the horizontal speed
        } else {
            dynamicMaxRunSpeed = maxRunSpeed;
        }
        if (Mathf.Abs(rb.velocityX) > dynamicMaxRunSpeed) {
            rb.velocityX -= (rb.velocityX - (dynamicMaxRunSpeed * Mathf.Sign(rb.velocityX))) * snapToMaxRunSpeedMult / 10; //Sets the speed to maxRunSpeed
        }

        //Footstep sound effect
        if (Mathf.Abs(rb.velocityX) > 0.1 && grounded) {
            footstepCount += (Mathf.Abs(rb.velocityX) * footstepRateScaler) * footstepRate * Time.deltaTime;
            if (footstepCount > 1) {
                playerFootstep.Post(gameObject);
                footstepCount--;
            }           
        }
        spriteRenderer.flipX = !facingRight;
    }
}
