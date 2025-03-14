using Cinemachine;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

public class MovementScript : MonoBehaviour, IGameplayControlsActions {
    public AK.Wwise.Event playerFootstep;
    public AK.Wwise.Event playerSlide;
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

     public bool inStealthMode;

    //Effective variables for when there are multiple values they can have depending on situation
    [NonSerialized] public float effectiveMaxRunSpeed;
    [NonSerialized] public float effectiveAcceleration;
    [NonSerialized] public float effectiveDeceleration;
    [NonSerialized] public float effectiveVelocityToSlide;
    [NonSerialized] public float effectiveVelocityEndSlide;
    [NonSerialized] public float effectiveJumpStrength;
    [NonSerialized] public float effectiveMinJumpTime;
    [NonSerialized] public float effectiveHorizontalWalljumpStrength;
    [NonSerialized] public float effectiveVerticalWalljumpStrength;


    //Simple short timer so that the player doesnt stop being grounded when crouching
    [NonSerialized] public float tempGroundedTimer;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D collider;
    [NonSerialized] public SpriteRenderer spriteRenderer;
    [NonSerialized] public Animator animator;

    [SerializeField] public float maxRunSpeed = 8; //The fastest the player can go horizontally
    [SerializeField] public float acceleration = 20; //Speeding up when running
    [SerializeField] public float deceleration = 15; //Slowing down when no longer running / running in opposite direction
    [SerializeField] public float snapToMaxRunSpeedMult = 1f; //How quickly the player snaps back to max running speed when running faster than it

    [SerializeField][Range(0f, 1f)] private float snapToLedgeTopRayHeight = 0.22f; //Height of the ray that needs to be not hitting something to snap to a ledge
    [SerializeField][Range(0f, 1f)] private float snapToLedgeBottomRayHeight = 0.05f; //Height of the ray that needs to be hitting something to snap to a ledge

    [SerializeField] public float slideDeceleration = 1; //Slowing down sliding
    [SerializeField] private float velocityToSlide = 12; //Velocity the player needs to be to be able to slide
    [SerializeField] private float velocityEndSlide = 5; //Velocity the player needs to be to be able to slide

    [SerializeField] public float boostMaxRunSpeedMultiplier = 1.5f; //Multiplier for the max run speed when boosting
    [SerializeField] public float boostAcceleration = 25; //New acceleration when boosting
    [SerializeField] public float boostRecharge = 10f; //Boost recharge rate
    [SerializeField] public float boostDepletion = 50f; //Boost depletion rate
    [SerializeField] private float minimumBoostCharge = 5; //The minimum boost required to start boosting

    [SerializeField] public float jumpStrength = 5; //Initial vertical velocity when jumping
    [SerializeField][Range(0f, 0.5f)] public float minJumpTime = 0.1f; //Time in seconds that the player must jump for before fastfalling
    [SerializeField] public float gravityMult = 1; //Gravity multiplier when not fastfalling
    [SerializeField] public float fastFallActivationSpeed = 1; //At what vertical speed the fast fall kicks in at
    [SerializeField] public float fastFallMult = 2; //Fast fall multiplier
    [SerializeField] public float maxFallSpeed = 5; //Needs to be higher if fastfallmult is higher also
    [SerializeField][Range(0.01f, 1f)] public float fallSlowsRunMult = 1; //Multiplier for how much falling speed slows down horizontal speed.
    [SerializeField][Range(0.01f, 0.5f)] public float coyoteTime = 0.05f;

    [SerializeField] public float wallClingSpeed = 1; //How quickly the player falls when clinging to a wall
    [SerializeField][Range(0f, 1f)] private float walljumpRayGap = 0.8f; //Position of rays, smaller gaps mean smaller range the player can walljump from
    [SerializeField] public float horizontalWalljumpStrength = 8f; //How much horizontal speed a walljump gives
    [SerializeField] public float verticalWalljumpStrength = 8f; //How much vertical speed a walljump gives
    [SerializeField][Range(0.01f, 1f)] private float walljumpInputDelay = 0.5f; //Delay for moving the opposite direction after a walljump

    [SerializeField] public float stealthMaxRunSpeed = 8; //The fastest the player can go horizontally
    [SerializeField] public float stealthAcceleration = 20; //Speeding up when running
    [SerializeField] public float stealthDeceleration = 15; //Slowing down when no longer running / running in opposite direction
    [SerializeField] private float stealthVelocityToSlide = 12; //Velocity the player needs to be to be able to slide
    [SerializeField] private float stealthVelocityEndSlide = 5; //Velocity the player needs to be to be able to slide
    [SerializeField] public float stealthJumpStrength = 5; //Initial vertical velocity when jumping
    [SerializeField][Range(0f, 0.5f)] public float stealthMinJumpTime = 0.1f; //Time in seconds that the player must jump for before fastfalling
    [SerializeField] public float stealthHorizontalWalljumpStrength = 8f; //How much horizontal speed a walljump gives
    [SerializeField] public float stealthVerticalWalljumpStrength = 8f; //How much vertical speed a walljump gives

    [SerializeField] public float cloakRecharge = 10f;
    [SerializeField] public float cloakDepletion = 70f;

    [SerializeField] public float boostFootStepSoundRange = 10f;
    [SerializeField] public float boostFootStepSoundSuspicionIncrease = 15f;

    [SerializeField] public float boostJumpSoundRange =25f;
    [SerializeField] public float boostJumpSoundSuspicionIncrease = 35f;

    [SerializeField] public float boostSlideSoundRange = 15f;
    [SerializeField] public float boostSlideSoundSuspicionIncrease = 20f;

    [NonSerialized] public bool grounded; //Grounded is only for the ground, a seperate one will be used for walls
    [NonSerialized] public bool minJumpActive; //If the player is in the first part of a jump where they cant fastfall
    [NonSerialized] public bool onWall; //If the player is on a wall
    [NonSerialized] public bool onRightWall; //If the wall the player is on is to the right
    [NonSerialized] public int postWalljumpInputs; //If inputs are taken in for the opposite direction for the duration after a walljump
    [NonSerialized] public bool facingRight; //Is facing to the right
    [NonSerialized] public bool sliding; //If the player is currently sliding
    [NonSerialized] public bool boosting; //If the player is currently boosting
    [NonSerialized] public float boostingMaxRunSpeedMultiplier = 1; //If the player is currently boosting
    [NonSerialized] public bool cloaking;
    public float batteryCharge; //The current boosting charge the player has

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
    InputAction boostCloakAction;

    //The hasActioned variables are so that the player cannot hold in the key to keep jumping forever, or slide many times in a row by just holding in the key
    [NonSerialized] public int runInput;
    [NonSerialized] public bool jumpInput;
    [NonSerialized] public bool hasJumped; //If the player has jumped while holding the jump key
    [NonSerialized] public bool slideInput;
    [NonSerialized] public bool hasSlid; //If the player has slid while holding the slide key
    [NonSerialized] public bool boostCloakInput;
    [NonSerialized] public bool hasBoostCloaked; //If the player has boosted while holding the boost key
    [NonSerialized] public bool canEndSlide; //If the player can end their slide

    //RB velocityX absolute value
    [NonSerialized] public float horizontalVelocity;

    private LayerMask layers;

    [NonSerialized] public Vector2 colliderSize;

    AlarmSystem alarm;
    CinemachineVirtualCamera movementCamera;
    CinemachineVirtualCamera stealthCamera;

    Running runningScript;
    Jumping jumpScript;
    Sliding slideScript;
    Boost boostScript;
    Cloak cloakScript;

    private float distanceSnap = 0.2f;
    private float predictionSnap = 1.15f;
    private float offsetSnap = 0.03f;

    private float animationCoyoteTime = 0.167f;
    private float animationGroundedTimer = -1;

    //reference to the ui mode change script
    private UIModeChange uiModeChange;
    private void Awake() {
        layers = new LayerMask();
        layers = 0b0110011;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        alarm = GameObject.Find("AlarmObject").GetComponent<AlarmSystem>();

        movementCamera = GameObject.Find("MovementFollowerCamera").GetComponent<CinemachineVirtualCamera>();
        stealthCamera = GameObject.Find("StealthFollowerCamera").GetComponent<CinemachineVirtualCamera>();
        uiModeChange = GameObject.Find("GameController").GetComponent<UIModeChange>();

        boostScript = new Boost();
        jumpScript = new Jumping();
        runningScript = new Running();
        cloakScript = new Cloak();
        slideScript = new Sliding();

        colliderSize = collider.size;
        //inStealthMode = false;
        effectiveMaxRunSpeed = maxRunSpeed;
        effectiveAcceleration = acceleration;
        effectiveDeceleration = deceleration;
        effectiveVelocityToSlide = velocityToSlide;
        effectiveVelocityEndSlide = velocityEndSlide;
        effectiveJumpStrength = jumpStrength;
        effectiveMinJumpTime = minJumpTime;
        effectiveHorizontalWalljumpStrength = horizontalWalljumpStrength;
        effectiveVerticalWalljumpStrength = verticalWalljumpStrength;
        batteryCharge = 100;

        //Setup inputs
        if (controls == null) {
            controls = new PlayerControls();
            controls.GameplayControls.SetCallbacks(this);
        }
        controls.GameplayControls.Enable();
        runAction = controls.FindAction("Running");
        jumpAction = controls.FindAction("Jumping");
        slideAction = controls.FindAction("Sliding");
        boostCloakAction = controls.FindAction("BoostCloak");
    }

    // Update is called once per frame
    void FixedUpdate() {

        //Checks inputs
        HandleInputs();
        //Checks if grounded, on wall, and other raycasts
        CheckGrounded();
        //Calculates jumping and falling, all vertical velocity
        JumpAndFall();
        //Boosting and Cloaking, the action that switches between modes
        BoostCloak();
        //Running and Sliding, all horizontal velocity
        RunSlide();

        //Set up variables for animation and audio
        if (sliding) {
            if (canEndSlide) {
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
        animator.SetBool("Sliding", sliding);
        animator.SetFloat("CoyoteTime", animationGroundedTimer);

        horizontalVelocity = Mathf.Abs(rb.velocityX);

        changeModeToStealth(inStealthMode);

        if (Input.GetKey(KeyCode.L))
        {
            alarm.StopAlarm();
        }
    }

    void HandleInputs() {
        runInput = Mathf.RoundToInt(runAction.ReadValue<float>());

        jumpInput = jumpAction.ReadValue<float>() > 0;
        if (!jumpInput) {
            hasJumped = false;
        }

        slideInput = slideAction.ReadValue<float>() > 0;
        if (!slideInput) {
            hasSlid = false;
        }

        boostCloakInput = boostCloakAction.ReadValue<float>() > 0;
        if (!boostCloakInput) {
            hasBoostCloaked = false;
            
            if (cloaking) {
                cloakScript.Disable();
            }
        }
    }

    void CheckGrounded() {
        if (landingCooldown > 0) {
            landingCooldown -= Time.fixedDeltaTime;
        }

        doRayCasts();

        //If the player is grounded
        if (Physics2D.Raycast(rightGroundRayStart, Vector2.down, 0.1f, layers) ||
            Physics2D.Raycast(leftGroundRayStart, Vector2.down, 0.1f, layers)) {
            if (!grounded && landingCooldown <= 0) {
                //Plays the Player_Land sound if the player was not grounded last frame and it isnt on cooldown
                landingCooldown = 0.1f;
                
                AkSoundEngine.PostEvent("Player_Land", this.gameObject);
            }
            grounded = true;
            tempGroundedTimer = coyoteTime;
            animationGroundedTimer = animationCoyoteTime;
            onWall = false;
            //animator.SetFloat("CoyoteTime", animationGroundedTimer);
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
            RaycastHit2D topRightSnap = Physics2D.Raycast(topRightSnapRayStart, Vector2.right, rb.velocityX * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            RaycastHit2D bottomRightSnap = Physics2D.Raycast(bottomRightSnapRayStart, Vector2.right, rb.velocityX * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            RaycastHit2D topLeftSnap = Physics2D.Raycast(topLeftSnapRayStart, Vector2.left, rb.velocityX * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            RaycastHit2D bottomLeftSnap = Physics2D.Raycast(bottomLeftSnapRayStart, Vector2.left, rb.velocityX * Time.fixedDeltaTime * predictionSnap + offsetSnap, layers);
            //Check if top ray isnt hitting anything and bottom ray is
            if ((!topRightSnap && bottomRightSnap) || (!topLeftSnap && bottomLeftSnap)) {
                //dif ((bottomRightSnap && bottomRightSnap.distance < ) || (bottomLeftSnap && bottomLeftSnap.distance > rb.velocityX * Time.fixedDeltaTime * pred))
                rb.position += new Vector2(distanceSnap * runInput, snapToLedgeTopRayHeight * collider.size.y);
                tempGroundedTimer = 0.05f;
            }
        }

        //If the player can stop sliding
        if (sliding) {
            RaycastHit2D rightSlide = Physics2D.Raycast(rightSlideRayStart, Vector2.up, colliderSize.y * 0.98f, layers);
            RaycastHit2D leftSlide = Physics2D.Raycast(leftSlideRayStart, Vector2.up, colliderSize.y * 0.98f, layers);
            if (rightSlide || leftSlide) {
                canEndSlide = false;
            } else {
                canEndSlide = true;
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

        } else if (jumpInput && onWall && !hasJumped) { //Walljumping

            jumpScript.WallJump();

        }

        //If you arent on a wall or you are moving upwards, you wont slide down a wall
        if (!onWall || rb.velocityY > 0) {
            if (rb.velocityY < fastFallActivationSpeed || (!Input.GetKey(KeyCode.Space) && !minJumpActive)) {

                jumpScript.Falling();

            } else {

                jumpScript.FastFalling();

            }   
        } else { //If you are sliding down a wall

            jumpScript.SlidingDownWall();
        
        }
    }

    public IEnumerator MinJumpDuration() {
        minJumpActive = true;
        yield return new WaitForSeconds(effectiveMinJumpTime);
        minJumpActive = false;
    }

    public IEnumerator WalljumpInputDelay(int direction) {
        postWalljumpInputs = direction;
        spriteRenderer.flipX = Convert.ToBoolean((direction + 1) / 2);
        yield return new WaitForSeconds(walljumpInputDelay);
        postWalljumpInputs = 0;
    }

    void BoostCloak() {
        //If movement mode active, do boost things

        //They use the same input, and there is a cloakScript already created
        
        if (boostCloakInput) {
            //Boosting
            if (!inStealthMode) {
                if (!hasBoostCloaked && runInput != 0 && batteryCharge > minimumBoostCharge && grounded) { //Can only boost if enough charge and on the ground, as well as holding in the boost button and a direction

                    boostScript.StartBoosting();

                } else if (batteryCharge < minimumBoostCharge || Mathf.Abs(rb.velocityX) < 0.05f) {

                    boostScript.StopBoosting();

                }
                if (boosting) {

                    boostScript.WhileBoosting();

                }
            } else { //Cloaking
                if (!cloaking) {
                    if (batteryCharge > minimumBoostCharge && !hasBoostCloaked) {
                        cloakScript.Enable();
                    } else {
                        hasBoostCloaked = true;
                    }
                } else {
                    if (batteryCharge > minimumBoostCharge) {
                        cloakScript.OnTick();
                    } else {
                        cloakScript.Disable();
                    }
                }
            }
        } else {
            if (inStealthMode) {
                if (cloaking) {
                    cloakScript.Disable();
                }
            } else {
                if ((boosting && grounded) || (boosting && onWall)) {
                    boostScript.StopBoosting();
                } else if (boosting) {
                    boostScript.WhileBoosting();
                }
            }
        }

        if (!cloaking && !boosting) {
            if (inStealthMode) {
                if (batteryCharge + cloakRecharge * Time.fixedDeltaTime < 100f) {
                    batteryCharge += cloakRecharge * Time.fixedDeltaTime;
                } else {
                    batteryCharge = 100f;
                }
            } else {
                boostScript.NotBoosting();
            }
        }

    }

    void RunSlide() {

        //Handle Sliding
        if (slideInput && grounded && !sliding && Mathf.Abs(rb.velocityX) >= effectiveVelocityToSlide && !hasSlid) {

            slideScript.StartSliding();

        } else if ((!slideInput || Mathf.Abs(rb.velocityX) < effectiveVelocityEndSlide || !grounded) && sliding && canEndSlide) {

            slideScript.StopSliding();

        }
        if (sliding) {

            slideScript.WhileSliding();

        }
        
        //Handle Running
        if (runInput != 0 && postWalljumpInputs != runInput && !sliding && (grounded || horizontalVelocity < dynamicMaxRunSpeed)) {

            runningScript.Accelerate(runInput);

        } else if (rb.velocityX != 0 && grounded) {

            runningScript.Decelerate();

        }
        if (!grounded && horizontalVelocity > dynamicMaxRunSpeed) {
            if (MathF.Sign(rb.velocityX) == -runInput) {
                runningScript.Accelerate(runInput);
            }
        }

        //Decide what the max velocity is and cap the player if necessary
        runningScript.CapRunningSpeed();

        //Do sound effects for footsteps
        runningScript.FootstepSounds();

        //Update the sprite to flip it to the right direction
        spriteRenderer.flipX = !facingRight;
    }

    public void changeModeToStealth(bool mode) {
        inStealthMode = mode;
        if (inStealthMode) {
            boostScript.StopBoosting();
            stealthCamera.Priority = 10;
            movementCamera.Priority = 0;
            uiModeChange.stealthMode();

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
            //cloakScript.Disable();
            movementCamera.Priority = 10;
            stealthCamera.Priority = 0;
            uiModeChange.movementMode();

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

    public void OnBoostCloak(InputAction.CallbackContext context) {
        
    }

    public void OnHacking(InputAction.CallbackContext context) {
        
    }
}
