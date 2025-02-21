using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BaseEnemyBehaviour : MonoBehaviour
{
    public AK.Wwise.Event inViewCone;

    public enum SuspicionLevel
    {
        Idle,
        Suspect,
        HighAlert,
        Alarm
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
    protected SuspicionLevel suspicionState;

    protected VisionCone visionCone;

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
        suspicionState = SuspicionLevel.Idle;
        Player = null;
    }
}
