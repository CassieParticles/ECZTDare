using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraBehavior : MonoBehaviour
{

    [SerializeField]
    private float[] thresholds = new float[3] { 33.3f, 66.6f, 100.0f };



    [SerializeField, Range(0, 200)]
    private float suspicionScalar = 20.0f;
    public enum SuspicionLevel
    {
        Idle,
        Suspect,
        HighAlert,
        Alarm
    };

    private float suspicion;

    private SuspicionLevel suspicionLevel;



    public void SeePlayer(GameObject player)
    {
        suspicion += suspicionScalar * Time.fixedDeltaTime;
    }

    private void Awake()
    {
        suspicion = 0.0f;
        suspicionLevel = SuspicionLevel.Idle;
    }

    private void FixedUpdate()
    {
        if(suspicionLevel != SuspicionLevel.Alarm)
        {
            //Alarm is currently being raised
        }
        else if(suspicion > thresholds[(int)suspicionLevel])
        {

        }
    }
}
