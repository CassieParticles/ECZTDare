using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static PlayerMovement;

public class MovementScript : MonoBehaviour, IKeyboardWASDActions {
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

    //How fast the player is currently sliding down the wall
    private float wallClingVelocity;

    //Effective deceleration for when sliding
    private float effectiveDeceleration;

    //Simple short timer so that the player doesnt stop being grounded when crouching
    private float slideGroundedTimer;
    
    [NonSerialized] public Rigidbody2D rb;
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float maxRunSpeed = 8; //The fastest the player can go horizontally
    [SerializeField] private float acceleration = 20; //Speeding up when running
    [SerializeField] private float deceleration = 15; //Slowing down when no longer running / running in opposite direction
    [SerializeField] private float snapToMaxRunSpeedMult = 1f; //How quickly the player snaps back to max running speed when running faster than it
    [SerializeField] private float slideDeceleration = 1; //Slowing down when no longer running / running in opposite direction
    [SerializeField] private float velocityToSlide = 12; //Velocity the player needs to be to be able to slide
    [SerializeField] private float velocityEndSlide = 5; //Velocity the player needs to be to be able to slide

    [SerializeField] private float jumpStrength = 5; //Initial vertical velocity when jumping
    [SerializeField][Range(0f, 0.5f)] private float minJumpTime = 0.1f; //Time in seconds that the player must jump for before fastfalling
    [SerializeField] private float gravityMult = 1; //Gravity multiplier when not fastfalling
    [SerializeField] private float fastFallActivationSpeed = 1; //At what vertical speed the fast fall kicks in at
    [SerializeField] private float fastFallMult = 2; //Fast fall multiplier
    [SerializeField] private float maxFallSpeed = 5; //Needs to be higher if fastfallmult is higher also
    [SerializeField][Range(0.01f, 1f)] private float fallSlowsRunMult = 1; //Multiplier for how much falling speed slows down horizontal speed.

    [SerializeField] private float wallClingSpeed; //How quickly the player falls when clinging to a wall
    [SerializeField][Range(0f, 1f)] private float walljumpRayGap = 0.8f; //Position of rays, smaller gaps mean smaller range the player can walljump from
    [SerializeField] private float horizontalWalljumpStrength = 8f; //How much horizontal speed a walljump gives
    [SerializeField] private float verticalWalljumpStrength = 8f; //How much vertical speed a walljump gives
    [SerializeField][Range(0.01f, 1f)] private float walljumpInputDelay = 0.5f; //Delay for moving the opposite direction after a walljump

    [NonSerialized] public bool grounded; //Grounded is only for the ground, a seperate one will be used for walls
    [NonSerialized] public bool minJumpActive; //If the player is in the first part of a jump where they cant fastfall
    [NonSerialized] public bool onWall; //If the player is on a wall
    [NonSerialized] public bool onRightWall; //If the wall the player is on is to the right
    [NonSerialized] public int jumpedOffWall; //-1 for left wall, 0 for neither, and 1 for right wall
    [NonSerialized] public int postWalljumpInputs; //If inputs are taken in for the opposite direction for the duration after a walljump
    [NonSerialized] public bool facingRight; //Is facing to the right
    [NonSerialized] public bool sliding; //If the player is currently sliding

    //All raycasts that get used
    Vector2 rightGroundRayStart;
    Vector2 leftGroundRayStart;
    Vector2 topRightWallRayStart;
    Vector2 bottomRightWallRayStart;
    Vector2 topLeftWallRayStart;
    Vector2 bottomLeftWallRayStart;

    //All inputs that are used
    PlayerMovement controls;
    InputAction runAction;
    InputAction jumpAction;
    InputAction slideAction;
    InputAction boostAction;

    int movementDir;
    bool jumpInput;
    bool hasJumped; //If the player has jumped while holding the jump key
    bool slideInput;
    bool hasSlid; //If the player has slid while holding the slide key
    bool boostInput;


    private LayerMask layers;

    Vector2 colliderSize;

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

        colliderSize = collider.size;
        effectiveDeceleration = deceleration;

        if (controls == null) {
            controls = new PlayerMovement();
            controls.KeyboardWASD.SetCallbacks(this);
        }
        controls.KeyboardWASD.Enable();
        runAction = controls.FindAction("Running");
        jumpAction = controls.FindAction("Jumping");
        slideAction = controls.FindAction("Sliding");
        boostAction = controls.FindAction("Boosting");
    }

    // Update is called once per frame
    void FixedUpdate() {

        //Checks inputs
        HandleInputs();
        //Checks if grounded, on wall, and other raycasts
        CheckGrounded();
        //Calculates jumping and falling, all vertical velocity
        JumpAndFall();
        //Running and Sliding, all horizontal velocity
        RunAndSlide();

        
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocityX));
        animator.SetFloat("yVelocity", rb.velocityY);

        if (Input.GetKey(KeyCode.L))
        {
            alarm.StopAlarm();
        }
    }

    void HandleInputs() {
        movementDir = Mathf.RoundToInt(runAction.ReadValue<float>());

        jumpInput = jumpAction.ReadValue<float>() > 0;
        if (!jumpInput) {
            hasJumped = false;
        }

        slideInput = slideAction.ReadValue<float>() > 0;
        if (!slideInput) {
            hasSlid = false;
        }

        boostInput = boostAction.ReadValue<float>() > 0;
    }

    void CheckGrounded() {
        if (landingCooldown > 0) {
            landingCooldown -= Time.deltaTime;
        }

        doRayCasts();

        if (Physics2D.Raycast(rightGroundRayStart, Vector2.down, 0.1f, layers) ||
            Physics2D.Raycast(leftGroundRayStart, Vector2.down, 0.1f, layers)) {
            if (!grounded && landingCooldown <= 0) {
                //Plays the Player_Land sound if the player was not grounded last frame and it isnt on cooldown
                landingCooldown = 0.1f;
                
                AkSoundEngine.PostEvent("Player_Land", this.gameObject);
            }
            grounded = true;
            jumpedOffWall = 0;
        } else {
            grounded = false;
        }

        if (slideGroundedTimer > 0) {
            slideGroundedTimer -= Time.deltaTime;
            grounded = true;
        }

        animator.SetBool("Grounded", grounded);


        if (!grounded) 
        {   
            if (Physics2D.Raycast(topRightWallRayStart, Vector2.right, 0.1f, layers) || 
                Physics2D.Raycast(bottomRightWallRayStart, Vector2.right, 0.1f, layers)) 
            { //If the player is on a wall to their right
                onWall = true;
                onRightWall = true;
            } else if (Physics2D.Raycast(topLeftWallRayStart, Vector2.left, 0.1f, layers) ||
                       Physics2D.Raycast(bottomLeftWallRayStart, Vector2.left, 0.1f, layers))
            { //If the player is on a wall to their left
                onWall = true;
                onRightWall = false;
            } else 
            { //Player is not on a wall
                onWall = false;
            }
        }
        //animator set bool onWall


    }

    void doRayCasts() {
        //Rays for checking if the player is on the ground
        rightGroundRayStart = rb.position + collider.offset + new Vector2(collider.size.x * 0.99f / 2f,
                                                                         -collider.size.y * 0.99f / 2f);
        leftGroundRayStart = rb.position + collider.offset + new Vector2(-collider.size.x * 0.99f / 2f,
                                                                         -collider.size.y * 0.99f / 2f);

        //Rays for checking if the player is on a wall
        topRightWallRayStart = rb.position + collider.offset + new Vector2(collider.size.x * 0.99f / 2f,
                                                                          collider.size.y * walljumpRayGap / 2f);
        bottomRightWallRayStart = rb.position + collider.offset + new Vector2(collider.size.x * 0.99f / 2f,
                                                                             -collider.size.y * walljumpRayGap / 2f);
        topLeftWallRayStart = rb.position + collider.offset + new Vector2(-collider.size.x * 0.99f / 2f,
                                                                         collider.size.y * walljumpRayGap / 2f);
        bottomLeftWallRayStart = rb.position + collider.offset + new Vector2(-collider.size.x * 0.99f / 2f,
                                                                            -collider.size.y * walljumpRayGap / 2f);
    }
    void JumpAndFall() {
        if (jumpInput && grounded && !hasJumped) { //Normal Jumping
            rb.velocityY = jumpStrength;
            //Plays the Player_Jump sound
            AkSoundEngine.PostEvent("Player_Jump", this.gameObject);
            animator.SetBool("Grounded", false);
            hasJumped = true;
            StartCoroutine(MinJumpDuration());
        } else if (jumpInput && onWall && !hasJumped) { //Walljumping
            int whichWallJump = Convert.ToInt32(onRightWall) * 2 - 1;
            hasJumped = true;
            if (whichWallJump == -1 && jumpedOffWall != -1) { //Jumping off a left wall
                rb.velocityX = horizontalWalljumpStrength;
                rb.velocityY = verticalWalljumpStrength;
                jumpedOffWall = -1;
                //Plays the Player_Jump sound
                AkSoundEngine.PostEvent("Player_Jump", this.gameObject);
                StartCoroutine("WalljumpInputDelay", -1);
            } else if (whichWallJump == 1 && jumpedOffWall != 1) { //Jumping off a right wall
                rb.velocityX = -horizontalWalljumpStrength;
                rb.velocityY = verticalWalljumpStrength;
                jumpedOffWall = 1;
                //Plays the Player_Jump sound
                AkSoundEngine.PostEvent("Player_Jump", this.gameObject);
                StartCoroutine("WalljumpInputDelay", 1);
            }

        }
        bool cantSlideOnRightWall = false;
        if (jumpedOffWall != 0) {
            cantSlideOnRightWall = Convert.ToBoolean((jumpedOffWall + 1) / 2);
        } 
        //If you arent on a wall or you are against a wall you jumped off of, or you are moving upwards
        if (!onWall || (jumpedOffWall != 0 && cantSlideOnRightWall == onRightWall) || rb.velocityY > 0) {
            if (rb.velocityY < fastFallActivationSpeed || (!Input.GetKey(KeyCode.Space) && !minJumpActive)) {
                rb.velocityY += (fastFallMult - 1) * Physics2D.gravity.y * Time.deltaTime; //fallmult - 1 since gravity gets applied by default
                if (rb.velocityY < -maxFallSpeed) { //Less than because its negative
                    rb.velocityY = -maxFallSpeed;
                }
            } else {
                rb.velocityY += (gravityMult - 1) * Physics2D.gravity.y * Time.deltaTime;
            }   
        } else { //If you are sliding down a wall
            wallClingVelocity += wallClingSpeed * 0.01f;
            rb.velocityY += wallClingVelocity * (fastFallMult - 1) * Physics2D.gravity.y * Time.deltaTime;
            spriteRenderer.flipX = !onRightWall;
        }
    }
    IEnumerator MinJumpDuration() {
        minJumpActive = true;
        yield return new WaitForSeconds(minJumpTime);
        minJumpActive = false;
    }

    IEnumerator WalljumpInputDelay(int direction) {
        postWalljumpInputs = direction;
        spriteRenderer.flipX = Convert.ToBoolean((direction + 1) / 2);
        yield return new WaitForSeconds(walljumpInputDelay);
        postWalljumpInputs = 0;
    }

    void RunAndSlide() {
        if (Input.GetKey(KeyCode.S) && grounded && !sliding && Mathf.Abs(rb.velocityX) >= velocityToSlide) {
            sliding = true;
            collider.size = new Vector2(colliderSize.x * 1.5f, colliderSize.y * 0.3f);
            transform.position = new Vector2(transform.position.x, transform.position.y - colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
            effectiveDeceleration = slideDeceleration;
            slideGroundedTimer = 0.02f;
        } else if ((!Input.GetKey(KeyCode.S)  || Mathf.Abs(rb.velocityX) < velocityEndSlide || !grounded) && sliding) {
            sliding = false;
            transform.position = new Vector2(transform.position.x, transform.position.y + colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
            collider.size = colliderSize;
            effectiveDeceleration = deceleration;
        }

        if (movementDir == -1 && postWalljumpInputs != -1 && !sliding) { //If not recently jumped off a left wall
            facingRight = false;
            rb.velocityX += -acceleration * Time.deltaTime;
            if (Mathf.Sign(rb.velocityX) == 1) {
                rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime;
                
            }
        } else if (movementDir == 1 && postWalljumpInputs != 1 && !sliding) { //If not recently jumped off a right wall
            facingRight = true;
            rb.velocityX += acceleration * Time.deltaTime;
            if (Mathf.Sign(rb.velocityX) == -1) {
                rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime;
            }
        } else if (rb.velocityX != 0 && postWalljumpInputs == 0){
            rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime; //Decelerate when not holding left or right
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
        if (Mathf.Abs(rb.velocityX) > dynamicMaxRunSpeed && !sliding) {
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
        if (postWalljumpInputs == 0) {
            spriteRenderer.flipX = !facingRight;
        }
    }

    public void OnRunning(InputAction.CallbackContext context) {
        
    }

    public void OnJumping(InputAction.CallbackContext context) {
        
    }

    public void OnSliding(InputAction.CallbackContext context) {
        
    }

    public void OnBoosting(InputAction.CallbackContext context) {
        
    }
}
