using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : BaseEnemyBehaviour
{
    MenuScript menu;
    [SerializeField] GameObject menuCanvasPrefab;

    AlarmMusicHandler musicHandler;
    [SerializeField] private PatrolRoute patrolRoute;

    //The speed at which footstep sounds are triggered. Whenever footstepRate is 1 a footstep is played
    [SerializeField][Range(0.01f, 3.0f)] private float footstepRate = 1f;

    //How much the velocity of the player affects the footstep frequency
    [SerializeField][Range(0.01f, 3.0f)] private float footstepRateScalar = 1f;

    //Movement
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

    //Guard noises
    public AK.Wwise.Event guardFootstep;
    public AK.Wwise.Event alarmActivationSound;

    //Voice lines
    public List<AK.Wwise.Event> foundEmira;
    public List<AK.Wwise.Event> lostEmira;
    public List<AK.Wwise.Event> recognizeEmira;

    //Voice line subtitles
    [SerializeField] private List<string> foundEmiraText;
    [SerializeField] private List<string> lostEmiraText;
    [SerializeField] private List<string> recognizeEmiraText;

    //Subtitle object
    private Subtitle subtitle;

    //check if this is first time seeing Emira
    private bool canRecognizeEmira;

    //AI behaviour
    private NavMeshAgent agent;
    protected StateMachine guardBehaviour = new StateMachine();

    [NonSerialized]public Vector3 PointOfInterest;

    public Animator guardMoveAnimation { get; private set; }
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
        enemySight.LookAt(position);
    }

    public void Look(float angle)
    {
        //Cast to vision cone
        if((VisionCone)enemySight)
        {
            ((VisionCone)enemySight).Look(angle);
        }
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

    private void AlarmOn(Vector3 playerPosition, GameObject alarmCaller)
    {
        minimumSuspicion = SuspicionLevel[(int)SuspicionState.HighAlert];
        //Exit early if this guard called the alarm
        if (alarmCaller == gameObject){ return; }

        SetSuspicionState(SuspicionState.HighAlert);
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
        //If the player is visible, do not get distracted
        if(Player)
        {
            return;
        }
        //If the guard is raising the alarm, do not get distracted
        if(guardBehaviour.getCurrentState()==GuardStates.RaiseAlarm)
        {
            return;
        }
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

    private void SuspicionStateChanged(SuspicionState newState)
    {
        if (newState < SuspicionState.HighAlert)
        {
            changeSpeed(walkSpeed);
        }
        if (newState == SuspicionState.HighAlert)
        {
            changeSpeed(alertSpeed);
        }
        if(newState == SuspicionState.Chase)
        {
            changeSpeed(chaseSpeed);
        }

        //handle subtitles
        if (newState == SuspicionState.HighAlert)
        {
            if(lastFrameSuspicionState!=SuspicionState.Chase)
            {
                return;
            }
            //Pick a random voice line
            int randomVoiceline = Mathf.FloorToInt(UnityEngine.Random.Range(0, lostEmira.Count));
            
            //Post subtitle
            lostEmira[randomVoiceline].Post(this.gameObject);
            subtitle.StartSubtitle(lostEmiraText[randomVoiceline]);

            //Prevent repeated voice lines initially
            if (lostEmira.Count > 2)
            {
                lostEmira.RemoveAt(randomVoiceline);
                lostEmiraText.RemoveAt(randomVoiceline);
            }
        }

        if(newState == SuspicionState.Chase)
        {
            //First time seeing Emira
            if (!canRecognizeEmira)
            {
                canRecognizeEmira = true;

                //Pick random voice line
                int randomVoiceline = Mathf.FloorToInt(UnityEngine.Random.Range(0, foundEmira.Count));

                //Post voice line
                foundEmira[randomVoiceline].Post(this.gameObject);
                subtitle.StartSubtitle(foundEmiraText[randomVoiceline]);
            }
            else    
            {
                //Pick random voice line
                int randomVoiceline = Mathf.FloorToInt(UnityEngine.Random.Range(0, recognizeEmira.Count));

                //Post voice line
                recognizeEmira[randomVoiceline].Post(this.gameObject);
                subtitle.StartSubtitle(recognizeEmiraText[randomVoiceline]);
            } 
        }
    }

    private void UpdateAgentSpeed()
    {
        if (Math.Abs(agent.speed - desiredSpeed) < acceleration * Time.fixedDeltaTime)
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
                footstepCount += (Mathf.Abs(agent.velocity.x) * footstepRateScalar) * footstepRate * Time.deltaTime;
                if (footstepCount > 1)
                {
                    guardFootstep.Post(gameObject);
                    footstepCount--;
                }
            }
        }
    }

    protected void Awake()
    {
        //Call base set up function
        Setup();

        //Collect attached components
        agent = GetComponent<NavMeshAgent>();
        guardMoveAnimation = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        subtitle = GetComponent<Subtitle>();
        musicHandler = GameObject.Find("MusicSystem").GetComponent<AlarmMusicHandler>();

        //Initialize nav mesh agent for 2d movement
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        //Initialize guard AI state machine

        //Choose patrol state constructor based on if there is a patrol route available
        guardBehaviour.AddState(GuardStates.Patrol, patrolRoute ? 
            new PatrolState(gameObject, patrolRoute) : 
            new PatrolState(gameObject, transform.position, enemySight.transform.rotation.eulerAngles.z));

        guardBehaviour.AddState(GuardStates.Idle, new IdleState(gameObject));
        guardBehaviour.AddState(GuardStates.HearNoise,new HeardNoiseState(gameObject));
        guardBehaviour.AddState(GuardStates.Observe,new ObserveState(gameObject));
        guardBehaviour.AddState(GuardStates.Investigate,new InvestigateState(gameObject));
        guardBehaviour.AddState(GuardStates.Chase, new ChaseState(gameObject,alarm));
        guardBehaviour.AddState(GuardStates.RaiseAlarm, new RaiseAlarmState(gameObject, alarm));
        guardBehaviour.AddState(GuardStates.Bumped, new BumpedState(gameObject));
        guardBehaviour.AddState(GuardStates.LookAround,new LookAroundState(gameObject));
    }



    protected void Start()
    {
        //Collect menu system, initialize one if it doesn't exist
        if (GameObject.Find("Menu Canvas") == null) 
        {
            menu = Instantiate(menuCanvasPrefab).GetComponent<MenuScript>();
            menu.gameObject.name = "Menu Canvas";
        } else 
        {
            menu = GameObject.Find("Menu Canvas").GetComponent<MenuScript>();
        }

        //Set up guard with patrol route
        if (patrolRoute){ patrolRoute.AddGuard(gameObject); }

        //Set up listener functions
        if (alarm)
        {
            alarm.AddAlarmEnableFunc(AlarmOn);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }

        if (AudioDetectionSystem.getAudioSystem())
        { 
            AudioDetectionSystem.getAudioSystem().AddListener(gameObject, HearNoise); 
        }

        guardBehaviour.Start(GuardStates.Idle);

        desiredSpeed = agent.speed;
    }

    private void FixedUpdate()
    {
        //Early exit for cutscene
        if (inCutscene){ return; }

        //Call external behaviour functions
        BaseUpdate();
        guardBehaviour.BehaviourTick();

        if (guardBehaviour.getCurrentState() == GuardStates.Idle){ return; }

        //Update guard animation speed and direction
        guardMoveAnimation.SetFloat("xVelocity", Mathf.Abs(agent.velocity.x));
        if (agent.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (agent.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }


        CalcSuspicionIncrease();

        //IF state has changed this frame, call state change function
        if (lastFrameSuspicionState != suspicionState)
        {
            SuspicionStateChanged(suspicionState);
        }

        UpdateAgentSpeed();
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
