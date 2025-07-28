using Cinemachine;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;
using RangeAttribute = UnityEngine.RangeAttribute;

public class MovementScript : MonoBehaviour, IGameplayControlsActions {
    [Header("SFX")]
    public AK.Wwise.Event playerFootstep;
    public AK.Wwise.Event playerSlide;
    public AK.Wwise.Event playerDash;
    public AK.Wwise.Event boostStart;
    public AK.Wwise.Event boostRush;
    public AK.Wwise.Event boostStop;
    public AK.Wwise.Event cloakStart;
    public AK.Wwise.Event cloakStop;

    //The speed at which footstep sounds are triggered. Whenever footstepRate is 1 a footstep is played
    [SerializeField][Range(0.01f, 3.0f)] public float footstepRate = 1f;

    //How much the velocity of the player affects the footstep frequency
    [SerializeField][Range(0.01f, 3.0f)] public float footstepRateScaler = 1f;

    //Used to determine when to trigger footstep sounds.
    [NonSerialized] public float footstepCount = 0.0f;

    //Cooldown for playing the landing sound effect in seconds
    private float landingCooldown = 0.5f;

    //The highest velocity the player can reach, affected by the serialized value as well as other factors
    [NonSerialized] public float dynamicMaxRunSpeed = 0;

    //How fast the player is currently sliding down the wall
    [NonSerialized] public float wallClingVelocity;

    //Effective variables for when there are multiple values they can have depending on situation
    [NonSerialized] public float effectiveMaxRunSpeed;
    [NonSerialized] public float effectiveAcceleration;
    [NonSerialized] public float effectiveDeceleration;


    //Simple short timer so that the player doesnt stop being grounded when crouching
    [NonSerialized] public float tempGroundedTimer;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D collider;
    [NonSerialized] public SpriteRenderer spriteRenderer;
    [NonSerialized] public Animator animator;
    [NonSerialized] public Animator modeHexAnimator;
    [NonSerialized] public Subtitle modeHexSubtitle;
    [NonSerialized] public ParticleManager particleManager;

    [Header("RUNNING")]
    [SerializeField] public float maxRunSpeed = 8; //The fastest the player can go horizontally
    [SerializeField] public float acceleration = 20; //Speeding up when running
    [SerializeField] public float deceleration = 15; //Slowing down when no longer running / running in opposite direction
    [SerializeField] public float startingSpeed = 2; //Slowing down when no longer running / running in opposite direction
    [SerializeField] public float snapToMaxRunSpeedMult = 1f; //How quickly the player snaps back to max running speed when running faster than it

    [SerializeField][Range(0f, 1f)] private float snapToLedgeTopRayHeight = 0.22f; //Height of the ray that needs to be not hitting something to snap to a ledge
    [SerializeField][Range(0f, 1f)] private float snapToLedgeBottomRayHeight = 0.05f; //Height of the ray that needs to be hitting something to snap to a ledge

    [Header("JUMPING")]
    [SerializeField] public float jumpStrength = 5; //Initial vertical velocity when jumping
    [SerializeField][Range(0f, 0.5f)] public float minJumpTime = 0.1f; //Time in seconds that the player must jump for before fastfalling
    [SerializeField] public float gravityMult = 1; //Gravity multiplier when not fastfalling
    [SerializeField] public float fastFallActivationSpeed = 1; //At what vertical speed the fast fall kicks in at
    [SerializeField] public float fastFallMult = 2; //Fast fall multiplier
    [SerializeField] public float maxFallSpeed = 5; //Needs to be higher if fastfallmult is higher also
    [SerializeField][Range(0.01f, 1f)] public float fallSlowsRunMult = 1; //Multiplier for how much falling speed slows down horizontal speed.
    [SerializeField][Range(0.01f, 0.5f)] public float coyoteTime = 0.05f;

    [Header("WALLJUMPING")]
    [SerializeField] public float wallClingSpeed = 1; //How quickly the player falls when clinging to a wall
    [SerializeField][Range(0f, 1f)] private float walljumpRayGap = 0.8f; //Position of rays, smaller gaps mean smaller range the player can walljump from
    [SerializeField] public float horizontalWalljumpStrength = 8f; //How much horizontal speed a walljump gives
    [SerializeField] public float verticalWalljumpStrength = 8f; //How much vertical speed a walljump gives
    [SerializeField][Range(0.01f, 1f)] private float walljumpInputDelay = 0.5f; //Delay for moving the opposite direction after a walljump
    
    [Header("SLIDING AND CROUCHING")]
    [SerializeField] public float slideDeceleration = 1; //Slowing down sliding
    [SerializeField] private float velocityToSlide = 12; //Velocity the player needs to be to be able to slide
    [SerializeField] private float velocityEndSlide = 5; //Velocity the player needs to be to be able to continue a slide
    [SerializeField] public float maxCrouchSpeed = 8; //Max speed while crouching
    [SerializeField] public float crouchAcceleration = 10; //Acceleration while crouching
    [SerializeField] public float crouchDeceleration = 3; //Deceleration while crouching

    [Header("CLOAK AND DASH")]
    [SerializeField] public float batteryRecharge = 10f; //Boost recharge rate
    [SerializeField] private float minimumBatteryToCloak = 5; //The minimum boost required to start cloaking
    [SerializeField] public float cloakDepletion = 70f;
    public float dashSpeed = 29.5f;
    public float dashDuration = 0.2f;
    public float dashBatteryCost = 25f;
    public float dashCooldown = 0.2f;
    public int dashChargesPerJump = 1;
    [NonSerialized] public int dashChargesRemaining = 1;
    [NonSerialized] public bool dashCooldownActive = false;


    [NonSerialized] public bool grounded; //Grounded is only for the ground, a seperate one will be used for walls
    [NonSerialized] public bool minJumpActive; //If the player is in the first part of a jump where they cant fastfall
    [NonSerialized] public bool onWall; //If the player is on a wall
    [NonSerialized] public bool onRightWall; //If the wall the player is on is to the right
    [NonSerialized] public int postWalljumpInputs; //If inputs are taken in for the opposite direction for the duration after a walljump
    [NonSerialized] public bool facingRight = true; //Is facing to the right
    [Header("DEBUG AND OTHER")]
    public bool sliding; //If the player is currently sliding
    public bool crouching; //If the player is currently crouching
    //[NonSerialized] public bool boosting; //If the player is currently boosting
    [NonSerialized] public bool dashing; //If the player is currently boosting
    [NonSerialized] public bool cloaking;

    public float batteryCharge = 100; //The current boosting charge the player has
    public bool cloakUnlocked = false;


    [NonSerialized] public float conveyorSpeed = 0f;
    [NonSerialized] public float jumpingFromConveyorSpeed = 0f;

    //Disables user input, if set to true, also sets all movement to 0 (prevent directions being "held down")
    private bool inputLocked;

        public bool InputLocked
        {
        get => inputLocked;
        set
        {
            inputLocked = value;
            //Set input locked to be value, and if value is true, clear inputs
            if (value == true)
            {
                runInput = 0;
                jumpInput = false;
                slideInput = false;
                dashInput = false;
            }
        }
    }


    //All raycasts that get used
    //Grounded checks
    Vector2 rightGroundRayStart;
    Vector2 leftGroundRayStart;
    //OnWall checks
    Vector2 topRightWallRayStart;
    Vector2 bottomRightWallRayStart;
    Vector2 topLeftWallRayStart;
    Vector2 bottomLeftWallRayStart;
    //Snap to ledges checks
    Vector2 topRightSnapRayStart;
    Vector2 bottomRightSnapRayStart;
    Vector2 topLeftSnapRayStart;
    Vector2 bottomLeftSnapRayStart;
    //Make sure the player can stop sliding checks
    Vector2 rightSlideRayStart;
    Vector2 leftSlideRayStart;

    //All inputs that are used
    PlayerControls controls;
    InputAction runAction;
    InputAction jumpAction;
    InputAction slideAction;
    InputAction dashAction;
    InputAction cloakAction;
    ControlsScript controlsScript;

    //The hasActioned variables are so that the player cannot hold in the key to keep jumping forever, or slide many times in a row by just holding in the key
    [NonSerialized] public int runInput;
    [NonSerialized] public bool jumpInput;
    [NonSerialized] public bool hasJumped; //If the player has jumped while holding the jump key
    [NonSerialized] public bool slideInput;
    [NonSerialized] public bool hasSlid; //If the player has slid while holding the slide key
    [NonSerialized] public bool dashInput;
    [NonSerialized] public bool hasDashed; //If the player has dashed while holding the dash key
    [NonSerialized] public bool cloakInput;
    [NonSerialized] public bool hasCloaked; //If the player has dashed while holding the dash key
    [NonSerialized] public bool canStandUp; //If the player can end their slide
    

    //RB velocityX absolute value
    [NonSerialized] public float horizontalVelocity;

    private LayerMask layers;

    [NonSerialized] public Vector2 colliderSize;

    CinemachineVirtualCamera movementCamera;

    Running runningScript;
    Jumping jumpScript;
    public Sliding slideScript;
    Dash dashScript;
    Cloak cloakScript;

    private float distanceSnap = 0.2f;
    private float predictionSnap = 1.15f;
    private float offsetSnap = 0.03f;

    private float animationCoyoteTime = 0.167f;
    private float animationGroundedTimer = -1;

    public List<Vector2> velocityBuffer;
    public int velocityBufferSize = 20;
    public float velocityBufferRayScaler = 1;
    public float velocityBufferStoppedSpeedThreshold = 1;
    public float velocityBufferRecoveredSpeedThreshold = 1;

    private void Start() {
        layers = new LayerMask();
        layers = 0b0110011;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        movementCamera = GameObject.Find("MovementFollowerCamera").GetComponent<CinemachineVirtualCamera>();

        runningScript = new Running();
        jumpScript = new Jumping();
        slideScript = new Sliding();
        dashScript = new Dash();
        cloakScript = new Cloak();
        particleManager = new ParticleManager();

        colliderSize = collider.size;
        effectiveMaxRunSpeed = maxRunSpeed;
        effectiveAcceleration = acceleration;
        effectiveDeceleration = deceleration;

        //Setup inputs
        controlsScript = GameObject.Find("Menu Canvas").GetComponent<ControlsScript>();
        if (controlsScript != null) {
            controls = controlsScript.controls;
            controls.GameplayControls.SetCallbacks(this);
        } else {
            controls = new PlayerControls();
            controls.GameplayControls.SetCallbacks(this);
        }
        controls.GameplayControls.Enable();
        runAction = controls.FindAction("Running");
        jumpAction = controls.FindAction("Jumping");
        slideAction = controls.FindAction("Sliding");
        cloakAction = controls.FindAction("Cloaking");
        dashAction = controls.FindAction("Dashing");

        velocityBuffer = new List<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate() {

        //Checks inputs
        HandleInputs();
        //Checks if grounded, on wall, and other raycasts
        CheckGrounded();
        //Calculates jumping and falling, all vertical velocity
        JumpAndFall();
        //if (boostCloakUnlocked) {
            //Boosting and Cloaking, the ability that switches between modes
        DashCloak();
        //}
        //Running and Sliding, all horizontal velocity
        RunSlide();

        //Set up variables for animation and audio
        if (sliding) {
            if (canStandUp) {
                animator.SetFloat("xVelocity", Mathf.Abs(rb.velocityX));
            } else {
                animator.SetFloat("xVelocity", 6.1f);
            }
        } else {
            animator.SetFloat("xVelocity", Mathf.Abs(rb.velocityX));
        }
        animator.SetFloat("yVelocity", rb.velocityY);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("OnWall", onWall);
        animator.SetBool("Sliding", sliding); //MARK replaced sliding || crouching with just sliding
        animator.SetFloat("CoyoteTime", animationGroundedTimer);
        animator.SetBool("Crouching", crouching);

        horizontalVelocity = Mathf.Abs(rb.velocityX);

        VelocityBufferCheck();
        //changeModeToStealth(inStealthMode);
    }

    void HandleInputs() {
        if(inputLocked)
        {
            return;
        }

        runInput = Mathf.RoundToInt(runAction.ReadValue<float>());

        jumpInput = jumpAction.ReadValue<float>() > 0;
        if (!jumpInput) {
            hasJumped = false;
        }

        slideInput = slideAction.ReadValue<float>() > 0;
        if (!slideInput) {
            hasSlid = false;
        }

        dashInput = dashAction.ReadValue<float>() > 0;
        if (!dashInput) {
            hasDashed = false;
        }

        cloakInput = cloakAction.ReadValue<float>() > 0;
        if (!cloakInput) {
            hasCloaked = false;
        }
    }

    void CheckGrounded() {
        if (landingCooldown > 0) {
            landingCooldown -= Time.fixedDeltaTime;
        }

        doRayCasts();

        //If the player is grounded
        RaycastHit2D rightGroundRay = Physics2D.Raycast(rightGroundRayStart, Vector2.down, 0.1f, layers);
        RaycastHit2D leftGroundRay = Physics2D.Raycast(leftGroundRayStart, Vector2.down, 0.1f, layers);
        if (rightGroundRay || leftGroundRay) {
            if (!grounded && landingCooldown <= 0) {
                //Plays the Player_Land sound if the player was not grounded last frame and it isnt on cooldown
                landingCooldown = 0.1f;
                particleManager.Dust();
                AkSoundEngine.PostEvent("Player_Land", this.gameObject);
            }
            grounded = true;
            tempGroundedTimer = coyoteTime;
            animationGroundedTimer = animationCoyoteTime;
            dashChargesRemaining = dashChargesPerJump;
            onWall = false;
            //Conveyor belts
            jumpingFromConveyorSpeed = 0f;
            if (rightGroundRay) {
                ConveyorBeltScript rightConveyor = rightGroundRay.transform.gameObject.GetComponent<ConveyorBeltScript>();
                if (rightConveyor != null) {
                    if (conveyorSpeed == 0) { //Reduce speed when landing on a conveyor
                        rb.velocityX += -conveyorSpeed;
                    }
                    conveyorSpeed = rightConveyor.currentSpeed;
                } else {
                    conveyorSpeed = 0;
                    rb.velocityX += conveyorSpeed;
                }
            } else {
                ConveyorBeltScript leftConveyor = leftGroundRay.transform.gameObject.GetComponent<ConveyorBeltScript>();
                if (leftConveyor != null) {
                    if (conveyorSpeed == 0) { //Reduce speed when landing on a conveyor
                        rb.velocityX += -conveyorSpeed;
                    }
                    conveyorSpeed = leftConveyor.currentSpeed;
                } else {
                    conveyorSpeed = 0;
                    rb.velocityX += conveyorSpeed;
                }    
            }
        } else {
            grounded = false;
            animationGroundedTimer -= Time.fixedDeltaTime;
        }

        //Grounds the player temporarily, currently is being used if the player starts sliding, and when they fall off a ledge (coyote time)
        if (tempGroundedTimer > 0) {
            tempGroundedTimer -= Time.fixedDeltaTime;
            
            grounded = true;
        }

        

        //If the player is on a wall
        if (!grounded) 
        {   
            if ((Physics2D.Raycast(topRightWallRayStart, Vector2.right, 0.1f, layers) || 
                Physics2D.Raycast(bottomRightWallRayStart, Vector2.right, 0.1f, layers))) 
            { //If the player is on a wall to their right
                if (!onWall)
                {
                    AkSoundEngine.PostEvent("Player_Land", this.gameObject);
                }
                rb.velocityX = 0;
                onWall = true;
                onRightWall = true;
                animator.SetFloat("CoyoteTime", animationGroundedTimer);
            } else if (Physics2D.Raycast(topLeftWallRayStart, Vector2.left, 0.1f, layers) ||
                       Physics2D.Raycast(bottomLeftWallRayStart, Vector2.left, 0.1f, layers))
            { //If the player is on a wall to their left
                if (!onWall)
                {
                    AkSoundEngine.PostEvent("Player_Land", this.gameObject);
                }
                rb.velocityX = 0;
                onWall = true;
                onRightWall = false;
            } else 
            { //Player is not on a wall
                onWall = false;
            }
            if (onWall && onRightWall == !facingRight) {
                facingRight = !facingRight;
            }
        }


        //If the player can snap to a ledge
        if (Mathf.Abs(rb.velocityX) >= 0.099 || runInput != 0) {
            RaycastHit2D topRightSnap = Physics2D.Raycast(topRightSnapRayStart, Vector2.right, horizontalVelocity * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            RaycastHit2D bottomRightSnap = Physics2D.Raycast(bottomRightSnapRayStart, Vector2.right, horizontalVelocity * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            RaycastHit2D topLeftSnap = Physics2D.Raycast(topLeftSnapRayStart, Vector2.left, horizontalVelocity * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            RaycastHit2D bottomLeftSnap = Physics2D.Raycast(bottomLeftSnapRayStart, Vector2.left, horizontalVelocity * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            //Check if top ray isnt hitting anything and bottom ray is
            if ((!topRightSnap && bottomRightSnap) || (!topLeftSnap && bottomLeftSnap)) {
                //dif ((bottomRightSnap && bottomRightSnap.distance < ) || (bottomLeftSnap && bottomLeftSnap.distance > rb.velocityX * Time.fixedDeltaTime * pred))
                rb.position += new Vector2(distanceSnap * runInput, snapToLedgeTopRayHeight * collider.size.y);
                tempGroundedTimer = 0.05f;
            }
        }

        //If the player can stop sliding
        if (sliding || crouching) {
            RaycastHit2D rightSlide = Physics2D.Raycast(rightSlideRayStart, Vector2.up, colliderSize.y * 0.98f, layers);
            RaycastHit2D leftSlide = Physics2D.Raycast(leftSlideRayStart, Vector2.up, colliderSize.y * 0.98f, layers);
            if (rightSlide || leftSlide) {
                canStandUp = false;
            } else {
                canStandUp = true;
            }
        }
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

        //Rays for snapping up ledges
        topRightSnapRayStart = rb.position + collider.offset + new Vector2(collider.size.x * 0.99f / 2f,
                                                                          collider.size.y * snapToLedgeTopRayHeight - collider.size.y / 2f);
        bottomRightSnapRayStart = rb.position + collider.offset + new Vector2(collider.size.x * 0.99f / 2f,
                                                                             collider.size.y * snapToLedgeBottomRayHeight - collider.size.y / 2f);
        topLeftSnapRayStart = rb.position + collider.offset + new Vector2(-collider.size.x * 0.99f / 2f,
                                                                         collider.size.y * snapToLedgeTopRayHeight - collider.size.y / 2f);
        bottomLeftSnapRayStart = rb.position + collider.offset + new Vector2(-collider.size.x * 0.99f / 2f,
                                                                            collider.size.y * snapToLedgeBottomRayHeight - collider.size.y / 2f);

        //Rays for checking if the player can stop sliding
        rightSlideRayStart = rb.position + collider.offset + new Vector2(colliderSize.x / 3f,
                                                                        -collider.size.y * 0.98f / 2f);
        leftSlideRayStart = rb.position + collider.offset + new Vector2(-colliderSize.x / 3f,
                                                                       -collider.size.y  * 0.98f / 2f);

    }

    void JumpAndFall() {
        if (jumpInput && grounded && !hasJumped) { //Normal Jumping

            jumpScript.BasicJump();
            animationGroundedTimer = 0;
            particleManager.Dust();

        } else if (jumpInput && onWall && !hasJumped) { //Walljumping

            jumpScript.WallJump();

        }

        //If you arent on a wall or you are moving upwards, you wont slide down a wall
        if (!grounded && (!onWall || rb.velocityY > 0)) {
            if (rb.gravityScale != 0) {
                if (rb.velocityY < fastFallActivationSpeed || (!jumpInput && !minJumpActive)) {

                    jumpScript.Falling();

                } else {

                    jumpScript.FastFalling();

                }
            }
            //Specifically if you fall off a conveyor, this adds the speed of the conveyor to the player speed, otherwise this does nothing
            rb.velocityX += conveyorSpeed;
            jumpingFromConveyorSpeed = conveyorSpeed;
            conveyorSpeed = 0;
        } else { //If you are sliding down a wall
            if (rb.gravityScale != 0) {
                jumpScript.SlidingDownWall();
            }
        }
    }

    public IEnumerator MinJumpDuration() {
        minJumpActive = true;
        yield return new WaitForSeconds(minJumpTime);
        minJumpActive = false;
    }

    public IEnumerator WalljumpInputDelay(int direction) {
        postWalljumpInputs = direction;
        spriteRenderer.flipX = Convert.ToBoolean((direction + 1) / 2);
        yield return new WaitForSeconds(walljumpInputDelay);
        postWalljumpInputs = 0;
    }

    void DashCloak() {
        //Dashing
        if (dashInput) {
            if (!dashing && !hasDashed && batteryCharge > 20 && dashChargesRemaining > 0 && !dashCooldownActive) {
                //Start a dash and save the direction
                int dashDirection = dashScript.StartDashing();
                particleManager.Afterimages(dashDuration, dashDirection);
            }
        }
        
        //Cloaking
        if (cloakUnlocked && cloakInput) {
            if (!cloaking) {
                if (batteryCharge > minimumBatteryToCloak && !hasCloaked) {
                    cloakScript.Enable();
                    particleManager.CloakOn();
                } else {
                    hasCloaked = true;
                }
            } else {
                if (batteryCharge > minimumBatteryToCloak) {
                    cloakScript.OnTick();
                } else {
                    cloakScript.Disable();
                    particleManager.CloakOff();
                }
            }
        }  else {
            if (cloaking) {
                cloakScript.Disable();
                particleManager.CloakOff();
            }
        }

        //Recharge Battery
        if (!cloaking) {
            if (batteryCharge + batteryRecharge * Time.deltaTime < 100f) {
                batteryCharge += batteryRecharge * Time.deltaTime;
            } else {
                batteryCharge = 100f;
            }
        }
    }

    void RunSlide() {

        //Handle Sliding and Crouching
        if (slideInput && grounded && !hasSlid && !sliding && !crouching) { //Conditions to either crouch or slide
            if (Mathf.Abs(rb.velocityX) >= velocityToSlide) { 
                slideScript.StartSliding();
            } else { //Depending on speed, crouch or slide will begin
                slideScript.Crouch();
            }
        }

        if (sliding || crouching) { //Handles transitions into standing or crouching as well as per frame updates during a slide
            if (sliding) {
                slideScript.WhileSliding();
            }
            if (canStandUp && (!slideInput || !grounded)) { //Conditions to make the player stand up again
                slideScript.StandUp();
            } else if (sliding && (slideInput || !canStandUp) && Mathf.Abs(rb.velocityX) < velocityEndSlide) { //Alternate conditions to instead transition into a crouch
                slideScript.SlideToCrouch();
            }
        }

        //Handle Running
        if (runInput != 0 && postWalljumpInputs != runInput && !sliding && (grounded || horizontalVelocity < dynamicMaxRunSpeed)) {

            runningScript.Accelerate(runInput);

        } else if (rb.velocityX != 0 && grounded && !dashing) {

            runningScript.Decelerate();

        }
        if (!grounded && horizontalVelocity > dynamicMaxRunSpeed) {
            if (MathF.Sign(rb.velocityX) == -runInput) {
                runningScript.Accelerate(runInput);
            }
        }

        transform.position += new Vector3(conveyorSpeed * Time.fixedDeltaTime, 0, 0);

        //Determine what the max velocity is and cap the player if necessary
        if (!dashing) {
            runningScript.CapRunningSpeed();
        }

        //Do sound effects for footsteps
        runningScript.FootstepSounds();

        //Update the sprite to flip it to the right direction
        if (rb.bodyType == RigidbodyType2D.Static) {
            spriteRenderer.flipX = false;
        } else {
            spriteRenderer.flipX = !facingRight;
        }
    }
    /*
    public void changeModeToStealth(bool mode) {
        if (inStealthMode != mode) {
            float animationTime = 1;
            if (!modeHexAnimator.GetCurrentAnimatorStateInfo(0).IsName("Empty")) {
                animationTime = modeHexAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            } else {

            }
            if (mode) {
                modeHexAnimator.Play("MovementToStealth");
                if (!modeHexSubtitle.writing) {
                    modeHexSubtitle.StartSubtitle("Stealth Mode Activated");                
                }
            } else {
                modeHexAnimator.Play("StealthToMovement");
                if (!modeHexSubtitle.writing) {
                    modeHexSubtitle.StartSubtitle("Speed Mode Activated");
                }
            }
        }
        inStealthMode = mode;
        if (inStealthMode) {
            boostScript.StopBoosting();
            particleManager.BoostOff();
            stealthCamera.Priority = 10;
            movementCamera.Priority = 0;
            uiModeChange.StealthMode();

            effectiveMaxRunSpeed = stealthMaxRunSpeed;
            effectiveAcceleration = stealthAcceleration;
            if (!sliding) {
                effectiveDeceleration = stealthDeceleration;
            }
            effectiveVelocityToSlide = stealthVelocityToSlide;
            effectiveVelocityEndSlide = stealthVelocityEndSlide;
            effectiveJumpStrength = stealthJumpStrength;
            effectiveMinJumpTime = stealthMinJumpTime;
            effectiveHorizontalWalljumpStrength = stealthHorizontalWalljumpStrength;
            effectiveVerticalWalljumpStrength = stealthVerticalWalljumpStrength;
            

        } else {
            if (cloaking) {
                cloakScript.Disable();
                particleManager.CloakOff();
            }
            movementCamera.Priority = 10;
            stealthCamera.Priority = 0;
            uiModeChange.MovementMode();

            effectiveMaxRunSpeed = maxRunSpeed;
            effectiveAcceleration = acceleration;
            if (!sliding) {
                effectiveDeceleration = deceleration;
            }
            effectiveVelocityToSlide = velocityToSlide;
            effectiveVelocityEndSlide = velocityEndSlide;
            effectiveJumpStrength = jumpStrength;
            effectiveMinJumpTime = minJumpTime;
            effectiveHorizontalWalljumpStrength = horizontalWalljumpStrength;
            effectiveVerticalWalljumpStrength = verticalWalljumpStrength;
            
        }
    }
    */

    private void VelocityBufferCheck() {
        velocityBuffer.Add(rb.velocity);
        if (velocityBuffer.Count > velocityBufferSize) {
            velocityBuffer.RemoveAt(0);    
        }

        Vector2 recoveredVelocity = Vector2.zero;

        if (MathF.Abs(rb.velocityX) < velocityBufferStoppedSpeedThreshold && !onWall) {
            foreach (var bufferedvelocity in velocityBuffer) {
                if (MathF.Abs(bufferedvelocity.x) > velocityBufferRecoveredSpeedThreshold) {
                    if (MathF.Abs(bufferedvelocity.x) > MathF.Abs(recoveredVelocity.x) && MathF.Sign(bufferedvelocity.x) == MathF.Sign(rb.velocityX) && MathF.Sign(bufferedvelocity.x) == runInput) {
                        recoveredVelocity.x = bufferedvelocity.x;
                        Debug.Log("found recovery speed x");
                    }
                }
            }
        }

        if (recoveredVelocity != Vector2.zero) {
            int recoverDirection = VelocityBufferRaycasts(recoveredVelocity);
            if (recoverDirection  != 0) {
                if (crouching) {
                    slideScript.StandUp();
                    slideScript.StartSliding();
                }
                rb.velocityX = recoveredVelocity.x;
                Debug.Log("buffer x velocity");

            }

        }

    }

    private int VelocityBufferRaycasts(Vector2 recoveredVelocity) {

        int velocityOverride = 0;


        if (recoveredVelocity.x != 0) {
            int dir = (int)recoveredVelocity.normalized.x;

            Vector2 horizontalTopRayStart = rb.position + collider.offset + new Vector2(dir * collider.size.x * 0.99f / 2f,
                                                                                        collider.size.y * 0.99f / 2f);
            Vector2 horizontalBottomRayStart = rb.position + collider.offset + new Vector2(dir * collider.size.x * 0.99f / 2f,
                                                                                           -collider.size.y * 0.99f / 2f);
            float rayXScaler = rb.velocityX * velocityBufferRayScaler * 0.1f;

            RaycastHit2D horizontalTopRay = Physics2D.Raycast(horizontalTopRayStart, Vector2.right * dir, rayXScaler, layers);
            RaycastHit2D horizontalBottomRay = Physics2D.Raycast(horizontalBottomRayStart, Vector2.right * dir, rayXScaler, layers);

            if (!sliding && !crouching) { //Dont do middle raycast if crouching or sliding as height is lower
                Vector2 horizontalMiddleRayStart = rb.position + collider.offset + new Vector2(dir * collider.size.x * 0.99f / 2f,
                                                                                               0);

                RaycastHit2D horizontalMiddleRay = Physics2D.Raycast(horizontalMiddleRayStart, Vector2.right * dir, rayXScaler, layers);

                if (!horizontalTopRay && !horizontalBottomRay && !horizontalMiddleRay) {
                    velocityOverride = 1;
                }

            } else {
                if (!horizontalTopRay && !horizontalBottomRay) {
                    velocityOverride = 1;
                }
            }
        }
        Debug.Log("override velocity");
        return velocityOverride;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bottomLeftWallRayStart, bottomLeftWallRayStart + new Vector2(-0.1f, 0));
        Gizmos.DrawLine(topLeftWallRayStart, topLeftWallRayStart + new Vector2(-0.1f, 0));
        Gizmos.DrawLine(bottomRightWallRayStart, bottomRightWallRayStart + new Vector2(0.1f, 0));
        Gizmos.DrawLine(topRightWallRayStart, topRightWallRayStart + new Vector2(0.1f, 0));
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(topLeftSnapRayStart, topLeftSnapRayStart + new Vector2(-rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
        //Gizmos.DrawLine(bottomLeftSnapRayStart, bottomLeftSnapRayStart + new Vector2(-rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
        //Gizmos.DrawLine(topRightSnapRayStart, topRightSnapRayStart + new Vector2(rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
        //Gizmos.DrawLine(bottomRightSnapRayStart, bottomRightSnapRayStart + new Vector2(rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
    }

    public void OnRunning(InputAction.CallbackContext context) {
        
    }

    public void OnJumping(InputAction.CallbackContext context) {
        
    }

    public void OnSliding(InputAction.CallbackContext context) {
        
    }

    public void OnCloaking(InputAction.CallbackContext context) {
        
    }

    public void OnDashing(InputAction.CallbackContext context) {

    }

    public void OnHacking(InputAction.CallbackContext context) {
        
    }

    public void OnAimHack(InputAction.CallbackContext context) {

    }

    public void OnPause(InputAction.CallbackContext context) {

    }

    public void OnReset(InputAction.CallbackContext context) {

    }
}
