using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemyBehaviour : MonoBehaviour
{
    public AK.Wwise.Event inViewCone;
    public AK.Wwise.Event enemyAlerted;
    public List<AK.Wwise.Event> foundEmira;
    public List<AK.Wwise.Event> lostEmira;
    public List<AK.Wwise.Event> recognizeEmira;
    public List<string> foundEmiraText;
    public List<string> lostEmiraText;
    public List<string> recognizeEmiraText;
    private bool canRecognizeEmira;

    private Subtitle subtitle;
    private List<AK.Wwise.Event> currentFoundEmira;
    private List<AK.Wwise.Event> currentLostEmira;
    private List<string> currentFoundEmiraText;
    private List<string> currentLostEmiraText;

    private void RefreshVoicelines(string type) {
        if (type == "found") {
            currentFoundEmira = foundEmira;
            currentFoundEmiraText = foundEmiraText;
        } else {
            currentLostEmira = lostEmira;
            currentLostEmiraText = lostEmiraText;

        }
    }

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
    public SuspicionState lastFrameSuspicionState { get; protected set; }

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
                visionCone.SetColour(new Color(1, 0.5f, 0));
                break;
            case SuspicionState.Chase:
                visionCone.SetColour(Color.red);
                break;
        }
    }

    //Called on awake of overriden classes
    protected void Setup()
    {
        visionCone = transform.GetChild(0).GetComponent<VisionCone>();
        suspicion = 0;
        minimumSuspicion = 0;
        suspicionState = SuspicionState.Idle;
        Player = null;

        subtitle = GetComponent<Subtitle>();
        RefreshVoicelines("found");
        RefreshVoicelines("lost");
    }

    //Should be called by all inheriting from BaseEnemy
    protected void BaseUpdate()
    {
        lastFrameSuspicionState = suspicionState;
        //Sets the RTPC Value of suspicion to the suspicion float value.
        AkSoundEngine.SetRTPCValue("suspicion", suspicion, this.gameObject);

        if (suspicion < SuspicionLevel[1])  //Below suspect threshold
        {
            if(suspicionState!=SuspicionState.Idle)
            {
                suspicionState = SuspicionState.Idle;
                UpdateSuspicionColour();
                playedSound = false;
            }
            
        }
        else if (suspicion < SuspicionLevel[2]) //Below high alert threshold
        {
            if (suspicionState != SuspicionState.Suspect)
            {
                suspicionState = SuspicionState.Suspect;
                UpdateSuspicionColour();
                playedSound = false;
            }
        }
        else if (suspicion < SuspicionLevel[3])  //Below chase threshold
        {
            if (suspicionState != SuspicionState.HighAlert)
            {
                suspicionState = SuspicionState.HighAlert;
                UpdateSuspicionColour();
                if (playedSound)
                {

                    if (gameObject.GetComponent<GuardBehaviour>() != null) {
                        int randomVoiceline = Mathf.FloorToInt(UnityEngine.Random.Range(0, lostEmira.Count));
                        lostEmira[randomVoiceline].Post(this.gameObject);
                        subtitle.StartSubtitle(lostEmiraText[randomVoiceline]);
                        if (lostEmira.Count <= 2) {
                            RefreshVoicelines("found");
                        } else {
                            lostEmira.RemoveAt(randomVoiceline);
                            lostEmiraText.RemoveAt(randomVoiceline);
                        }
                    }

                }
                playedSound = false;
            }
        }
        else
        {
            if(suspicionState!=SuspicionState.Chase)
            {
                suspicionState = SuspicionState.Chase;
                UpdateSuspicionColour();
                if (!playedSound)
                {
                    playedSound = true;
                    //Play sound
                    enemyAlerted.Post(this.gameObject);


                    if (gameObject.GetComponent<GuardBehaviour>() != null) {
                        if (!canRecognizeEmira) {
                            
                            canRecognizeEmira = true;
                            int randomVoiceline = Mathf.FloorToInt(UnityEngine.Random.Range(0, foundEmira.Count));
                            foundEmira[randomVoiceline].Post(this.gameObject);
                            subtitle.StartSubtitle(foundEmiraText[randomVoiceline]);
                            if (foundEmira.Count <= 2) {
                                RefreshVoicelines("lost");
                            } else {
                                foundEmira.RemoveAt(randomVoiceline);
                                foundEmiraText.RemoveAt(randomVoiceline);
                            }
                        } else {
                            int randomVoiceline = Mathf.FloorToInt(UnityEngine.Random.Range(0, recognizeEmira.Count));
                            recognizeEmira[randomVoiceline].Post(this.gameObject);
                            subtitle.StartSubtitle(recognizeEmiraText[randomVoiceline]);
                        }
                    }


                }
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
