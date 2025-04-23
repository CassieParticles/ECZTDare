using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : BaseEnemyBehaviour
{
    MenuScript menu;
    [SerializeField] GameObject menuCanvasPrefab;

    AlarmMusicHandler musicHandler;
    [SerializeField] private PatrolRoute patrolRoute;
    [SerializeField] private bool investigateAlarmLoc = false;

    //The speed at which footstep sounds are triggered. Whenever footstepRate is 1 a footstep is played
    [SerializeField][Range(0.01f, 3.0f)] private float footstepRate = 1f;

    //How much the velocity of the player affects the footstep frequency
    [SerializeField][Range(0.01f, 3.0f)] private float footstepRateScaler = 1f;

    public float walkSpeed = 5.0f;
    public float alertSpeed = 10.0f;
    public float chaseSpeed = 25.0f;

    public float acceleration=15f;
    private float desiredSpeed;

    /// <summary>
    /// How long will a guard be chasing the player before they call the alarm
    /// </summary>
    public float chaseAlarmTimer=1.0f;

    //Used to determine when to trigger footstep sounds.
    private float footstepCount = 0.0f;

    public AK.Wwise.Event guardFootstep;
    public AK.Wwise.Event alarmActivationSound;

    //AI behaviour
    private NavMeshAgent agent;
    private StateMachine guardBehaviour = new StateMachine();

    public Vector3 PointOfInterest;

    private Animator guardMoveAnimation;
    private SpriteRenderer spriteRenderer;

    //Disables user input, if set to true, also sets all movement to 0 (prevent directions being "held down")
    [SerializeField]private bool inCutscene = false;



    public void changeSpeed(float speed)
    {
        desiredSpeed = speed;
    }

    public void MoveTo(Vector3 position)
    {
        agent.SetDestination(position);
        LookAt(position);
    }

    public void LookAt(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Look(angle);
    }

    public void Look(float angle)
    {
        visionCone.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void StopMoving()
    {
        agent.SetDestination(transform.position);
    }

    public float getDistLeft()
    {
        return agent.remainingDistance;
    }

    public Vector3 getCurrentDestination()
    {
        return agent.destination;
    }

    private void AlarmOn(Vector3 playerPosition)
    {
        SetSuspicionState(SuspicionState.HighAlert);
        minimumSuspicion = SuspicionLevel[(int)SuspicionState.HighAlert];
        changeSpeed(alertSpeed);
        if ((playerPosition-transform.position).sqrMagnitude < 50 * 50)
        {
            PointOfInterest = playerPosition;
            guardBehaviour.MoveToState(GuardStates.Investigate);
        }

    }

    private void AlarmOff()
    {
        minimumSuspicion = 0;
        changeSpeed(walkSpeed);
    }

    private void HearNoise(Vector3 noiseLocation, float suspicionIncrease, AudioSource source)
    {
        PointOfInterest = noiseLocation;
        suspicion += suspicionIncrease;
        if(suspicion >= 100)
        {
            suspicion = 99;
        }
        if (source == AudioSource.Player)
        {
            guardBehaviour.MoveToState(GuardStates.HearNoise);
        }
        else if (source == AudioSource.Hacked)
        {
            guardBehaviour.MoveToState(GuardStates.Investigate);
        }
    }

    private void CatchPlayer()
    {
        if(StealthScoreTracker.GetTracker())
        {
            StealthScoreTracker.GetTracker().DeductPoints(StealthScoreTracker.Sources.Caught);
        }
        if (MainScoreController.GetInstance())
        {
            MainScoreController.GetInstance().Pause();
        }
        //Sets the "Music" State Group's active State to "Hidden"
        AkSoundEngine.SetState("Music", "NoMusic");
        musicHandler.music.Stop(gameObject);
        menu.Lose();
    }

    private void Awake()
    {
        Setup();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if(patrolRoute)
        {
            guardBehaviour.AddState(GuardStates.Patrol,new PatrolState(gameObject,patrolRoute));
        }
        else
        {
            guardBehaviour.AddState(GuardStates.Patrol, new PatrolState(gameObject, transform.position,visionCone.transform.rotation.eulerAngles.z));
        }

        guardBehaviour.AddState(GuardStates.Idle, new IdleState(gameObject));
        guardBehaviour.AddState(GuardStates.HearNoise,new HeardNoiseState(gameObject));
        guardBehaviour.AddState(GuardStates.Observe,new ObserveState(gameObject));
        guardBehaviour.AddState(GuardStates.Investigate,new InvestigateState(gameObject));
        guardBehaviour.AddState(GuardStates.Chase, new ChaseState(gameObject,alarm));
        guardBehaviour.AddState(GuardStates.RaiseAlarm, new RaiseAlarmState(gameObject, alarm));
        guardBehaviour.AddState(GuardStates.Bumped, new BumpedState(gameObject));

        guardMoveAnimation = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    void Start()
    {
        if (GameObject.Find("Menu Canvas") == null) {
            menu = Instantiate(menuCanvasPrefab).GetComponent<MenuScript>();
            menu.gameObject.name = "Menu Canvas";
        } else {
            menu = GameObject.Find("Menu Canvas").GetComponent<MenuScript>();
        }

        musicHandler = GameObject.Find("MusicSystem").GetComponent<AlarmMusicHandler>();
        if (patrolRoute){ patrolRoute.AddGuard(gameObject); }
        
        guardBehaviour.Start(GuardStates.Idle);

        if(alarm)
        {
            alarm.AddAlarmEnableFunc(AlarmOn);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }

        AudioDetectionSystem.getAudioSystem().AddListener(gameObject, HearNoise);

        desiredSpeed = agent.speed;
    }

    void FixedUpdate()
    {
        if (inCutscene)
        {
            return;
        }

        //Update animation parameters
        guardMoveAnimation.SetFloat("xVelocity", Mathf.Abs(agent.velocity.x));
        if (agent.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (agent.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }

        BaseUpdate();

        guardBehaviour.BehaviourTick();
        CalcSuspicionIncrease();

        if (lastFrameSuspicionState != suspicionState)
        {
            if(suspicionState < SuspicionState.HighAlert)
            {
                changeSpeed(walkSpeed);
            }
            if(suspicionState>=SuspicionState.HighAlert)
            {
                changeSpeed(alertSpeed);
            }
        }


        if(Math.Abs(agent.speed-desiredSpeed) < acceleration * Time.fixedDeltaTime)
        {
            agent.speed = desiredSpeed;
        }
        else
        {
            agent.speed += acceleration * Time.fixedDeltaTime * Mathf.Sign(desiredSpeed - agent.speed);
        }

        if (agent.velocity != Vector3.zero)
        {
            //Footstep sound effect
            if (Mathf.Abs(agent.velocity.x) > 0.1)
            {
                footstepCount += (Mathf.Abs(agent.velocity.x) * footstepRateScaler) * footstepRate * Time.deltaTime;
                if (footstepCount > 1)
                {
                    guardFootstep.Post(gameObject);
                    footstepCount--;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.name=="Player")
        {
            if (guardBehaviour.getCurrentState() == GuardStates.Chase)
            {
                CatchPlayer();
            }
            else if(guardBehaviour.getCurrentState()!=GuardStates.Bumped)
            {
                PointOfInterest = collision.transform.position;
                guardBehaviour.MoveToState(GuardStates.Bumped);
            }

        }
    }

}
