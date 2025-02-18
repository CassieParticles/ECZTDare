using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyBehaviour : MonoBehaviour
{
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
    [SerializeField, Range(0, 1000)] protected float suspicionScaleRate;

    //Fields used in enemy suspicion meter
    protected float suspicion;
    protected SuspicionLevel suspicionState;

    protected VisionCone visionCone;

    protected void Setup()
    {
        visionCone = transform.GetChild(0).GetComponent<VisionCone>();
        suspicion = 0;
        suspicionState = SuspicionLevel.Idle;
    }
}
