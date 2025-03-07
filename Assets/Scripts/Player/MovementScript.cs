using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerMovement;

public class MovementScript : MonoBehaviour, IKeyboardWASDActions {
    public AK.Wwise.Event playerFootstep;
    public AK.Wwise.Event playerSlide;
    public AK.Wwise.Event boostStart;
    public AK.Wwise.Event boostRush;
    public AK.Wwise.Event boostStop;

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
    public float wallClingVelocity;

    //Effective deceleration for when sliding
    [NonSerialized] public float effectiveDeceleration;

    //Simple short timer so that the player doesnt stop being grounded when crouching
    public float tempGroundedTimer;

    //Effective deceleration for when sliding
    [NonSerialized] public float effectiveAcceleration;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D collider;
    [NonSerialized] public SpriteRenderer spriteRenderer;
    public Animator animator;

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
    [SerializeField] public float boostRecharge = 50f; //Boost recharge rate
    [SerializeField] public float boostDepletion = 10f; //Boost depletion rate
    [SerializeField] private float minimumBoostCharge = 5; //The minimum boost required to start boosting

    [SerializeField] public float jumpStrength = 5; //Initial vertical velocity when jumping
    [SerializeField][Range(0f, 0.5f)] public float minJumpTime = 0.1f; //Time in seconds that the player must jump for before fastfalling
    [SerializeField] public float gravityMult = 1; //Gravity multiplier when not fastfalling
    [SerializeField] public float fastFallActivationSpeed = 1; //At what vertical speed the fast fall kicks in at
    [SerializeField] public float fastFallMult = 2; //Fast fall multiplier
    [SerializeField] public float maxFallSpeed = 5; //Needs to be higher if fastfallmult is higher also
    [SerializeField][Range(0.01f, 1f)] public float fallSlowsRunMult = 1; //Multiplier for how much falling speed slows down horizontal speed.

    [SerializeField] public float wallClingSpeed; //How quickly the player falls when clinging to a wall
    [SerializeField][Range(0f, 1f)] private float walljumpRayGap = 0.8f; //Position of rays, smaller gaps mean smaller range the player can walljump from
    [SerializeField] public float horizontalWalljumpStrength = 8f; //How much horizontal speed a walljump gives
    [SerializeField] public float verticalWalljumpStrength = 8f; //How much vertical speed a walljump gives
    [SerializeField][Range(0.01f, 1f)] private float walljumpInputDelay = 0.5f; //Delay for moving the opposite direction after a walljump

    [SerializeField] private float boostFootStepSoundRange = 10f;
    [SerializeField] private float boostFootStepSoundSuspicionIncrease = 15f;

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
    [NonSerialized] public float boostingMaxRunSpeedMultiplier; //If the player is currently boosting
    public float boostCharge; //The current boosting charge the player has

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

    //All inputs that are used
    PlayerMovement controls;
    InputAction runAction;
    InputAction jumpAction;
    InputAction slideAction;
    InputAction boostCloakAction;


    [NonSerialized] public int runInput;
    [NonSerialized] public bool jumpInput;
    [NonSerialized] public bool hasJumped; //If the player has jumped while holding the jump key
    [NonSerialized] public bool slideInput;
    [NonSerialized] public bool hasSlid; //If the player has slid while holding the slide key
    [NonSerialized] public bool boostCloakInput;
    [NonSerialized] public bool hasBoostCloaked; //If the player has boosted while holding the boost key

    [NonSerialized] public float horizontalVelocity;

    private LayerMask layers;

    [NonSerialized] public Vector2 colliderSize;

    AlarmSystem alarm;

    Running runningScript;
    CappingRunSpeed MaxSpeedScript;
    Jumping jumpScript;
    Sliding slideScript;
    Boost boostScript;
    Cloak cloakingScript;

    private float distanceSnap = 0.2f;
    private float predictionSnap = 1.15f;
    private float offsetSnap = 0.04f;

    private void Awake() {
        layers = new LayerMask();
        layers = 0b0110011;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        alarm = GameObject.Find("AlarmObject").GetComponent<AlarmSystem>();

        boostScript = new Boost();
        jumpScript = new Jumping();
        runningScript = new Running();
        MaxSpeedScript = new CappingRunSpeed();
        cloakingScript = new Cloak();
        slideScript = new Sliding();

        colliderSize = collider.size;
        effectiveDeceleration = deceleration;
        effectiveAcceleration = acceleration;
        boostCharge = 100;

        //Setup inputs
        if (controls == null) {
            controls = new PlayerMovement();
            controls.KeyboardWASD.SetCallbacks(this);
        }
        controls.KeyboardWASD.Enable();
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
        //Running and Sliding, all horizontal velocity
        RunBoostSlide();

        //Set up variables for animation and audio
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocityX));
        animator.SetFloat("yVelocity", rb.velocityY);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("OnWall", onWall);
        animator.SetBool("Sliding", sliding);

        horizontalVelocity = Mathf.Abs(rb.velocityX);

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
        }
    }

    void CheckGrounded() {
        if (landingCooldown > 0) {
            landingCooldown -= Time.deltaTime;
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
        } else {
            grounded = false;
        }

        //Grounds the player if they start sliding
        if (tempGroundedTimer > 0) {
            tempGroundedTimer -= Time.deltaTime;
            grounded = true;
        }

        

        //If the player is on a wall
        if (!grounded) 
        {   
            if (Physics2D.Raycast(topRightWallRayStart, Vector2.right, 0.1f, layers) || 
                Physics2D.Raycast(bottomRightWallRayStart, Vector2.right, 0.1f, layers)) 
            { //If the player is on a wall to their right
                if (!onWall)
                {
                    AkSoundEngine.PostEvent("Player_Land", this.gameObject);
                }
                onWall = true;
                onRightWall = true;
            } else if (Physics2D.Raycast(topLeftWallRayStart, Vector2.left, 0.1f, layers) ||
                       Physics2D.Raycast(bottomLeftWallRayStart, Vector2.left, 0.1f, layers))
            { //If the player is on a wall to their left
                if (!onWall)
                {
                    AkSoundEngine.PostEvent("Player_Land", this.gameObject);
                }
                onWall = true;
                onRightWall = false;
            } else 
            { //Player is not on a wall
                onWall = false;
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
    }
    void JumpAndFall() {
        if (jumpInput && grounded && !hasJumped) { //Normal Jumping
            jumpScript.BasicJump();
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
        yield return new WaitForSeconds(minJumpTime);
        minJumpActive = false;
    }

    public IEnumerator WalljumpInputDelay(int direction) {
        postWalljumpInputs = direction;
        spriteRenderer.flipX = Convert.ToBoolean((direction + 1) / 2);
        yield return new WaitForSeconds(walljumpInputDelay);
        postWalljumpInputs = 0;
    }

    void RunBoostSlide() {
        //Handle Sliding
        if (slideInput && grounded && !sliding && Mathf.Abs(rb.velocityX) >= velocityToSlide && !hasSlid) {
            slideScript.StartSliding();
        } else if ((!slideInput || Mathf.Abs(rb.velocityX) < velocityEndSlide || !grounded) && sliding) {
            slideScript.StopSliding();
        }
        if (sliding) {
            slideScript.WhileSliding();
        }
        

        
        //Handle Boosting
        if (boostCloakInput && runInput != 0 && boostCharge > minimumBoostCharge && grounded && !hasBoostCloaked) { //Can only boost if enough charge and on the ground, as well as holding in the boost button and a direction
            boostScript.StartBoosting();
        } else if (!boostCloakInput && grounded || boostCharge < minimumBoostCharge || rb.velocityX == 0) {
            boostScript.StopBoosting();
        }
        if (boosting) {
            boostScript.WhileBoosting();
        } else {
            boostScript.NotBoosting();
        }

        //Handle left/right movement with inputs
        //if (runInput == -1 && postWalljumpInputs != -1 && !sliding && (grounded || horizontalVelocity < dynamicMaxRunSpeed)) { //If not recently jumped off a left wall
        //    facingRight = false;
        //    rb.velocityX += -acceleration * Time.deltaTime;
        //    if (Mathf.Sign(rb.velocityX) == 1) {
        //        rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime;
        //    }
        //} else if (runInput == 1 && postWalljumpInputs != 1 && !sliding && (grounded || horizontalVelocity < dynamicMaxRunSpeed)) { //If not recently jumped off a right wall
        //    facingRight = true;
        //    rb.velocityX += acceleration * Time.deltaTime;
        //    if (Mathf.Sign(rb.velocityX) == -1) {
        //        rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime;
        //    }
        if (runInput != 0 && postWalljumpInputs != -1 && !sliding && (grounded || rb.velocityX < dynamicMaxRunSpeed)) {
            runningScript.Accelerate(runInput);
        } else if (rb.velocityX != 0 && postWalljumpInputs == 0 && grounded){
            runningScript.Decelerate();
        }

        //Decide what the max velocity is and cap the player if necessary
        if (rb.velocityY < 0) {
            dynamicMaxRunSpeed = maxRunSpeed * //Base max run speed
                                 boostingMaxRunSpeedMultiplier * //Boosting makes this multiplier not 1
                                 (1 - (fallSlowsRunMult * -rb.velocityY / maxFallSpeed)); //Falling slows down the horizontal speed
        } else {
            dynamicMaxRunSpeed = maxRunSpeed * boostingMaxRunSpeedMultiplier;
        }
        if (Mathf.Abs(rb.velocityX) > dynamicMaxRunSpeed && !sliding) {
            rb.velocityX -= (rb.velocityX - (dynamicMaxRunSpeed * Mathf.Sign(rb.velocityX))) * snapToMaxRunSpeedMult / 10; //Sets the speed to maxRunSpeed
        }

        //Footstep sound effect
        if (Mathf.Abs(rb.velocityX) > 0.1 && grounded) {
            footstepCount += (Mathf.Abs(rb.velocityX) * footstepRateScaler) * footstepRate * Time.deltaTime;
            if (horizontalVelocity < 20f)
            {
                if (footstepCount > 1)
                {
                    footstepRate = 0.1f;
                    playerFootstep.Post(gameObject);
                    footstepCount--;
                    //Alert noise
                    if (boosting)
                    {
                        AudioDetectionSystem.getAudioSystem().PlaySound(transform.position, boostFootStepSoundRange, boostFootStepSoundSuspicionIncrease);
                    }
                }
            }
            else if (horizontalVelocity >= 20f)
            {
                if (footstepCount > 1)
                {
                    footstepRate = 0.04f;
                    playerFootstep.Post(gameObject);
                    footstepCount--;
                    //Alert noise
                    if (boosting)
                    {
                        AudioDetectionSystem.getAudioSystem().PlaySound(transform.position, boostFootStepSoundRange, boostFootStepSoundSuspicionIncrease);
                    }
                }
            }
        }

        spriteRenderer.flipX = !facingRight;
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
}
