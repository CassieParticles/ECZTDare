using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
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
    private float wallClingVelocity;

    //Effective deceleration for when sliding
    private float effectiveDeceleration;

    //Simple short timer so that the player doesnt stop being grounded when crouching
    private float slideGroundedTimer;

    //Effective deceleration for when sliding
    private float effectiveAcceleration;

    [NonSerialized] public Rigidbody2D rb;
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float maxRunSpeed = 8; //The fastest the player can go horizontally
    [SerializeField] private float acceleration = 20; //Speeding up when running
    [SerializeField] private float deceleration = 15; //Slowing down when no longer running / running in opposite direction
    [SerializeField] private float snapToMaxRunSpeedMult = 1f; //How quickly the player snaps back to max running speed when running faster than it

    [SerializeField][Range(0f, 1f)] private float snapToLedgeTopRayHeight = 0.22f; //Height of the ray that needs to be not hitting something to snap to a ledge
    [SerializeField][Range(0f, 1f)] private float snapToLedgeBottomRayHeight = 0.05f; //Height of the ray that needs to be hitting something to snap to a ledge

    [SerializeField] private float slideDeceleration = 1; //Slowing down sliding
    [SerializeField] private float velocityToSlide = 12; //Velocity the player needs to be to be able to slide
    [SerializeField] private float velocityEndSlide = 5; //Velocity the player needs to be to be able to slide

    [SerializeField] private float boostMaxRunSpeedMultiplier = 1.5f; //Multiplier for the max run speed when boosting
    [SerializeField] private float boostAcceleration = 25; //New acceleration when boosting
    [SerializeField] private float boostRecharge = 50f; //Boost recharge rate
    [SerializeField] private float boostDepletion = 10f; //Boost depletion rate
    [SerializeField] private float minimumBoostCharge = 5; //The minimum boost required to start boosting

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

    [SerializeField] private float boostFootStepSoundRange = 10f;
    [SerializeField] private float boostFootStepSoundSuspicionIncrease = 15f;

    [SerializeField] private float boostJumpSoundRange =25f;
    [SerializeField] private float boostJumpSoundSuspicionIncrease = 35f;

    [SerializeField] private float boostSlideSoundRange = 15f;
    [SerializeField] private float boostSlideSoundSuspicionIncrease = 20f;

    [NonSerialized] public bool grounded; //Grounded is only for the ground, a seperate one will be used for walls
    [NonSerialized] public bool minJumpActive; //If the player is in the first part of a jump where they cant fastfall
    [NonSerialized] public bool onWall; //If the player is on a wall
    [NonSerialized] public bool onRightWall; //If the wall the player is on is to the right
    [NonSerialized] public int jumpedOffWall; //-1 for left wall, 0 for neither, and 1 for right wall
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
    InputAction boostAction;

    int runInput;
    bool jumpInput;
    bool hasJumped; //If the player has jumped while holding the jump key
    bool slideInput;
    bool hasSlid; //If the player has slid while holding the slide key
    bool boostInput;
    bool hasBoosted; //If the player has boosted while holding the boost key

    float horizontalVelocity;

    private LayerMask layers;

    Vector2 colliderSize;

    AlarmSystem alarm;

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
        RunBoostSlide();

        //Set up variables for animation and audio
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocityX));
        animator.SetFloat("yVelocity", rb.velocityY);

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

        boostInput = boostAction.ReadValue<float>() > 0;
        if (!boostInput) {
            hasBoosted = false;
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
            jumpedOffWall = 0;
        } else {
            grounded = false;
        }

        //Grounds the player if they start sliding
        if (slideGroundedTimer > 0) {
            slideGroundedTimer -= Time.deltaTime;
            grounded = true;
        }

        animator.SetBool("Grounded", grounded);

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
        //animator set bool onWall

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
            rb.velocityY = jumpStrength;
            //Plays the Player_Jump sound
            AkSoundEngine.PostEvent("Player_Jump", this.gameObject);
            if (boosting)
            {
                AudioDetectionSystem.getAudioSystem().PlaySound(transform.position, boostJumpSoundRange, boostJumpSoundSuspicionIncrease);
            }
            animator.SetBool("Grounded", false);
            hasJumped = true;
            StartCoroutine(MinJumpDuration());
        } else if (jumpInput && onWall && !hasJumped) { //Walljumping
            int whichWallJump = Convert.ToInt32(onRightWall) * 2 - 1;
            hasJumped = true;
            facingRight = !onRightWall;
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
            facingRight = !onRightWall;
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

    void RunBoostSlide() {
        //Handle Sliding
        if (slideInput && grounded && !sliding && Mathf.Abs(rb.velocityX) >= velocityToSlide && !hasSlid) {
            //Plays the slide sound.
            playerSlide.Post(gameObject);
            if(boosting)
            {
                AudioDetectionSystem.getAudioSystem().PlaySound(transform.position, boostSlideSoundRange, boostSlideSoundSuspicionIncrease);
            }
            sliding = true;
            hasSlid = true;
            collider.size = new Vector2(colliderSize.x * 1.5f, colliderSize.y * 0.3f);
            transform.position = new Vector2(transform.position.x, transform.position.y - colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
            effectiveDeceleration = slideDeceleration;
            slideGroundedTimer = 0.02f;
        } else if ((!slideInput || Mathf.Abs(rb.velocityX) < velocityEndSlide || !grounded) && sliding) {
            //Stops the slide sound.
            playerSlide.Stop(gameObject);
            sliding = false;
            transform.position = new Vector2(transform.position.x, transform.position.y + colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
            collider.size = colliderSize;
            effectiveDeceleration = deceleration;
        }
        if (sliding)
        {
            //Sets the RTPC Value of horizontalVelocity to the horizontalVelocity float value.
            AkSoundEngine.SetRTPCValue("horizontalVelocity", horizontalVelocity);
        }

        
        //Handle Boosting
        if (boostInput && runInput != 0 && boostCharge > minimumBoostCharge && grounded && !hasBoosted) { //Can only boost if enough charge and on the ground, as well as holding in the boost button and a direction
            if (sliding) { //Stops the player from sliding
                //Stops the slide sound.
                playerSlide.Stop(gameObject);
                sliding = false;
                transform.position = new Vector2(transform.position.x, transform.position.y + colliderSize.y * 0.31f); //Lower the player so they arent midair when sliding
                collider.size = colliderSize;
                effectiveDeceleration = deceleration;
            }
            boosting = true;
            hasBoosted = true;
            //Plays the boost start sound.
            boostStart.Post(gameObject);
            //Plays the boost rush sound.
            boostRush.Post(gameObject);
        } else if (!boostInput && grounded || boostCharge < minimumBoostCharge || rb.velocityX == 0) {
            if(boosting)
            {
                //Stops the boost rush sound.
                boostRush.Stop(gameObject);
                //Plays the boost stop sound.
                boostStop.Post(gameObject);
            }
            boosting = false;
        }
        if (boosting) {
            spriteRenderer.color = Color.red;
            //Sets the RTPC Value of horizontalVelocity to the horizontalVelocity float value.
            AkSoundEngine.SetRTPCValue("horizontalVelocity", horizontalVelocity);
            if (boostCharge - boostRecharge * Time.deltaTime > 0) {
                boostCharge -= boostRecharge * Time.deltaTime;
            } else {
                boostCharge = 0;
            }
            effectiveAcceleration = boostAcceleration;
            boostingMaxRunSpeedMultiplier = boostMaxRunSpeedMultiplier;
        } else {
            spriteRenderer.color = Color.white;
            if (boostCharge + boostDepletion * Time.deltaTime < 100f) {
                boostCharge += boostDepletion * Time.deltaTime;
            } else {
                boostCharge = 100f;
            }
            boostingMaxRunSpeedMultiplier = 1;
            effectiveAcceleration = acceleration;
        }
        
        //Handle left/right movement with inputs
        if (runInput == -1 && postWalljumpInputs != -1 && !sliding && (grounded || rb.velocityX < dynamicMaxRunSpeed)) { //If not recently jumped off a left wall
            facingRight = false;
            rb.velocityX += -acceleration * Time.deltaTime;
            if (Mathf.Sign(rb.velocityX) == 1) {
                rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime;
                
            }
        } else if (runInput == 1 && postWalljumpInputs != 1 && !sliding && (grounded || rb.velocityX < dynamicMaxRunSpeed)) { //If not recently jumped off a right wall
            facingRight = true;
            rb.velocityX += acceleration * Time.deltaTime;
            if (Mathf.Sign(rb.velocityX) == -1) {
                rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime;
            }
        } else if (rb.velocityX != 0 && postWalljumpInputs == 0 && grounded){
            rb.velocityX += effectiveDeceleration * -rb.velocityX * Time.deltaTime; //Decelerate when not holding left or right
            if (Mathf.Abs(rb.velocityX) < 0.1) {
                rb.velocityX = 0;
            }
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
            if (footstepCount > 1) {
                playerFootstep.Post(gameObject);
                footstepCount--;
                //Alert noise
                if(boosting)
                {
                    AudioDetectionSystem.getAudioSystem().PlaySound(transform.position,boostFootStepSoundRange,boostFootStepSoundSuspicionIncrease);
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
        Gizmos.DrawLine(topLeftSnapRayStart, topLeftSnapRayStart + new Vector2(-rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
        Gizmos.DrawLine(bottomLeftSnapRayStart, bottomLeftSnapRayStart + new Vector2(-rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
        Gizmos.DrawLine(topRightSnapRayStart, topRightSnapRayStart + new Vector2(rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
        Gizmos.DrawLine(bottomRightSnapRayStart, bottomRightSnapRayStart + new Vector2(rb.velocityX * Time.fixedDeltaTime * predictionSnap, 0));
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
