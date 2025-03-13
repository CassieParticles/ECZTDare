using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyBehaviour : MonoBehaviour
{
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
    [SerializeField, Range(0, 1000)] public float suspicionScaleRate;
    [SerializeField, Range(0, 1000)] public float suspicionDecayRate;


    //Fields used in enemy suspicion meter
    /// <summary>
    /// Level of suspicion of the enemy
    /// </summary>
    public float suspicion;
    public float minimumSuspicion;
    [NonSerialized] public SuspicionState suspicionState;

    private bool playedSound = false;

    public VisionCone visionCone{ get; protected set; }

    /// <summary>
    /// Player according to the enemy, null when player is not visible
    /// </summary>
    public GameObject Player { get; protected set; }



    //Call when the enemy first sees the player
    public void SeePlayer(GameObject player)
    {
        Player = player;
        inViewCone.Post(gameObject);
        //Handle other "seeing the player" stuff
    }

    //Call when the enemy stops being able to see the player
    public void LosePlayer()
    {
        Player = null;
        inViewCone.Stop(gameObject);
        //Handle other "losing the player" stuff
    }

    //Called on awake of overriden classes
    protected void Setup()
    {
        visionCone = transform.GetChild(0).GetComponent<VisionCone>();
        suspicion = 0;
        minimumSuspicion = 0;
        suspicionState = SuspicionState.Idle;
        Player = null;
    }

    //Should be called by all inheriting from BaseEnemy
    protected void BaseUpdate()
    {
        //Sets the RTPC Value of suspicion to the suspicion float value.
        AkSoundEngine.SetRTPCValue("suspicion", suspicion, this.gameObject);

        if (suspicion < SuspicionLevel[1])  //Below suspect threshold
        {
            suspicionState = SuspicionState.Idle;
            visionCone.SetColour(Color.white);
            playedSound = false;
        }
        else if (suspicion < SuspicionLevel[2]) //Below high alert threshold
        {
            suspicionState = SuspicionState.Suspect;
            visionCone.SetColour(Color.yellow);
            playedSound = false;
        }
        else if (suspicion < SuspicionLevel[3])  //Below chase threshold
        {
            suspicionState = SuspicionState.HighAlert;
            visionCone.SetColour(new Color(1, 0.5f, 0));
            playedSound = false;
        }
        else
        {
            suspicionState = SuspicionState.Chase;
            visionCone.SetColour(new Color(1, 0, 0));
            if(!playedSound)
            {
                playedSound = true;
                //Play sound
                enemyAlerted.Post(this.gameObject);
            }
        }



        //Put guard on "high alert" (won't go lower)
        if (suspicionState == SuspicionState.HighAlert)
        {
            minimumSuspicion = SuspicionLevel[(int)SuspicionState.HighAlert];
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
        float distScalar = Mathf.Clamp(1 - distance / visionConeLength, 0.05f, 1);

        return distScalar * suspicionScaleRate * Time.fixedDeltaTime;
    }
    /// <summary>
    /// Sets the enemy's suspicion state to the level specified and sets suspicion to that amount
    /// </summary>
    /// <param name="level"> The level of ssupicion the enemy should be at</param>
    public void SetSuspicionState(SuspicionState level)
    {
        suspicionState = level;
        
        suspicion = Mathf.Max(SuspicionLevel[(int)level] + 1,suspicion);
    }

    /// <summary>
    /// Increase the suspicion of the enemy
    /// </summary>
    public void CalcSuspicionIncrease()
    {
        if (suspicion < SuspicionLevel[3])
        {
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
            suspicion -= suspicionDecayRate * Time.fixedDeltaTime;
        }
    }
}
