using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyBehaviour : MonoBehaviour
{
    //Alert noises
    public AK.Wwise.Event inViewCone;
    public AK.Wwise.Event enemyAlerted;

    public enum SuspicionState
    {
        Idle,
        Suspect,
        HighAlert,
        Chase
    };

    /// <summary>
    /// Idle, Suspect, high alert and chasing thresholds, make sure they are in ascending order
    /// </summary>
    public float[] SuspicionLevel = new float[4] 
    {
        0,
        40,
        70,
        100
    };

    //Alarm system attached to enemy, set up by designed if alarm is wanted
    [SerializeField] protected AlarmSystem alarm = null;

    //Parameters for suspicion rate
    [SerializeField, Range(0, 5000)] public float suspicionScaleRate;
    [SerializeField, Range(0, 5000)] public float suspicionDecayRate;
    [SerializeField, Range(0, 1)] public float minimumDistanceScalar = 0.5f; 


    //Enemy suspicion and minimum suspicion, used when alarm is active
    public float suspicion;
    public float minimumSuspicion;

    //Suspicion state, this and last frame, allows enemy to know when it's state changed
    [NonSerialized] public SuspicionState suspicionState;
    public SuspicionState lastFrameSuspicionState { get; protected set; }


    

    //Vision cone attached to enemy
    public VisionCone visionCone{ get; protected set; }

    //Player, null for when enemy cannot see the player
    public GameObject Player { get; protected set; }



    //Call when the enemy first sees the player
    public void SeePlayer(GameObject player)
    {
        Player = player;
        inViewCone.Post(gameObject);
        //Handle other "seeing the player" stuff

        //Tell GUI player has been seen
        GUIAlarmHandler alarmHandler = FindAnyObjectByType<GUIAlarmHandler>();
        if (alarmHandler)
        {
            alarmHandler.EnemySeePlayer();
        }

    }

    //Call when the enemy stops being able to see the player
    public void LosePlayer()
    {
        Player = null;
        inViewCone.Stop(gameObject);
        //Handle other "losing the player" stuff

        //Tell GUI player has been lost
        GUIAlarmHandler alarmHandler = FindAnyObjectByType<GUIAlarmHandler>();
        if (alarmHandler)
        {
            alarmHandler.EnemyLosePlayer();
        }
    }

    private void UpdateSuspicionColour()
    {
        switch (suspicionState)
        {
            case SuspicionState.Idle:
                visionCone.SetColour(Color.white);
                break;
            case SuspicionState.Suspect:
                visionCone.SetColour(Color.yellow);
                break;
            case SuspicionState.HighAlert:
                visionCone.SetColour(new Color(1, 0.5f, 0));    //Orange, not a predefined colour
                break;
            case SuspicionState.Chase:
                visionCone.SetColour(Color.red);
                break;
        }
    }

    //Called on awake of overriden classes
    protected void Setup()
    {
        //Collect vision cone
        visionCone = transform.GetChild(0).GetComponent<VisionCone>();

        suspicion = 0;
        minimumSuspicion = 0;
        suspicionState = SuspicionState.Idle;

        Player = null;
    }

    //Should be called by all inheriting from BaseEnemy
    protected void BaseUpdate()
    {
        lastFrameSuspicionState = suspicionState;

        //Sets the RTPC Value of suspicion to the suspicion float value.
        AkSoundEngine.SetRTPCValue("suspicion", suspicion, this.gameObject);

        //Update suspicion state and vision cone colour with changing suspicion
        if (suspicion < SuspicionLevel[1])  //Below suspect threshold
        {
            if(suspicionState!=SuspicionState.Idle)
            {
                suspicionState = SuspicionState.Idle;
                UpdateSuspicionColour();
            }
            
        }
        else if (suspicion < SuspicionLevel[2]) //Below high alert threshold
        {
            if (suspicionState != SuspicionState.Suspect)
            {
                suspicionState = SuspicionState.Suspect;
                UpdateSuspicionColour();
            }
        }
        else if (suspicion < SuspicionLevel[3])  //Below chase threshold
        {
            if (suspicionState != SuspicionState.HighAlert)
            {
                suspicionState = SuspicionState.HighAlert;
                UpdateSuspicionColour();
            }
        }
        else    //Chase state
        {
            if(suspicionState!=SuspicionState.Chase)
            {
                suspicionState = SuspicionState.Chase;
                UpdateSuspicionColour();
            }
        }
    }

    private float calcSuspicionIncreaseRate(GameObject player)
    {
        if(!player)
        {
            return 0;
        }
        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = transform.position;

        //Get a scalar from 1 to 0 based for player's distance affecting scale rate
        float distance = (playerPos - enemyPos).magnitude;
        float visionConeLength = visionCone.distance;
        float distScalar = Mathf.Clamp(visionCone.GetPlayerDistanceScalar(), 0.05f, 1);

        return Mathf.Max(distScalar,minimumDistanceScalar) * suspicionScaleRate * Time.fixedDeltaTime;
    }
    /// <summary>
    /// Sets the enemy's suspicion state to the level specified and sets suspicion to that amount
    /// </summary>
    /// <param name="level"> The level of supicion the enemy should be at</param>
    public void SetSuspicionState(SuspicionState level)
    {
        suspicionState = level;
        suspicion = Mathf.Max(SuspicionLevel[(int)level] + 1,suspicion);

        //Update vision cone visual
        UpdateSuspicionColour();
    }

    /// <summary>
    /// Increase the suspicion of the enemy
    /// </summary>
    public void CalcSuspicionIncrease()
    {
        if (suspicion < SuspicionLevel[3])
        {
            //Update vision cone visual
            visionCone.RecalcConeTex();
            suspicion += calcSuspicionIncreaseRate(Player);
        }
    }
    /// <summary>
    /// Check if the suspicion should decay, and if so, handle suspicion decay
    /// </summary>
    public void CalcSuspicionDecay()
    {
        if (suspicion > minimumSuspicion + suspicionDecayRate * Time.fixedDeltaTime)
        {
            //Update vision cone visual
            visionCone.RecalcConeTex();
            suspicion -= suspicionDecayRate * Time.fixedDeltaTime;
        }
    }
}
