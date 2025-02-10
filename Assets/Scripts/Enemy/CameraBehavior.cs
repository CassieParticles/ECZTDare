using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class CameraBehavior : MonoBehaviour
{
    //Suspicion
    [SerializeField]
    private float[] thresholds = new float[3] { 33.3f, 66.6f, 100.0f };
    [SerializeField, Range(0, 200)]
    private float suspicionScalar = 20.0f;

    //Camera rotation
    [SerializeField,Range(0,180)]
    private float maxTurnAngle = 45;
    [SerializeField, Range(0, 60)]
    private float turnSpeed = 30;
    [SerializeField, Range(0.1f, 20)]
    private float pauseDuration = 1;

    public enum SuspicionLevel
    {
        Idle,
        Suspect,
        HighAlert,
        Alarm
    };
    private float suspicion;
    private SuspicionLevel suspicionLevel;


    private GameObject visionCone;
    private float initialAngle;
    bool turnCCW;
    bool turnPause;

    public void SeePlayer(GameObject player)
    {
        suspicion += suspicionScalar * Time.fixedDeltaTime;
    }

    private IEnumerator PauseCamera()
    {
        turnPause = false;
        yield return new WaitForSeconds(pauseDuration);
        turnPause = true;
        turnCCW = !turnCCW;
    }

    private void Awake()
    {
        suspicion = 0.0f;
        suspicionLevel = SuspicionLevel.Idle;
        visionCone = transform.GetChild(0).gameObject;

        turnCCW = true;
        initialAngle = transform.GetChild(0).rotation.eulerAngles.z;
        turnPause = true;
    }

    private void FixedUpdate()
    {
        //suspicion handling
        if(suspicionLevel == SuspicionLevel.Alarm)
        {
            //Alarm is currently being raised
            Debug.Log("Alarm is being raised");
        }
        else if(suspicion > thresholds[(int)suspicionLevel])
        {
            suspicionLevel++;
            Debug.Log("Gotten more suspicious");
        }

        //Turning handling
        Vector3 visionAngle = visionCone.transform.rotation.eulerAngles;
        float boundary = initialAngle + maxTurnAngle * (turnCCW ? 1 : -1);

        if (visionAngle.z > 180)
        {
            boundary += 360;
        }


        //If camera is past max angle
        if(turnCCW) //Turning CCW, check using upper bound
        {
            if(visionAngle.z > boundary)
            {
                visionAngle.z += turnSpeed * Time.fixedDeltaTime * -1;    //Take a step back, to prevent immediately firing again once it's started back up
                StartCoroutine(PauseCamera());
            }
        }
        else    //Turning CW, check using lower bound
        {
            if(visionAngle.z < boundary)
            {
                visionAngle.z += turnSpeed * Time.fixedDeltaTime;    //Take a step back, to prevent immediately firing again once it's started back up
                StartCoroutine(PauseCamera());
            }
        }



        visionAngle.z += turnSpeed * Time.fixedDeltaTime * (turnCCW ? 1 : -1) * (turnPause ? 1 : 0);  //Rotate by
        visionCone.transform.rotation = Quaternion.Euler(visionAngle);
    }

    private void OnDrawGizmosSelected()
    {
        float initialAngle = transform.GetChild(0).rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 initialPos = transform.GetChild(0).position;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(initialPos, initialPos + new Vector3(Mathf.Cos(initialAngle),Mathf.Sin(initialAngle)) * 15);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(initialPos, initialPos + new Vector3(Mathf.Cos(initialAngle + maxTurnAngle * Mathf.Deg2Rad), Mathf.Sin(initialAngle + maxTurnAngle * Mathf.Deg2Rad)) * 15);
        Gizmos.DrawLine(initialPos, initialPos + new Vector3(Mathf.Cos(initialAngle - maxTurnAngle * Mathf.Deg2Rad), Mathf.Sin(initialAngle - maxTurnAngle * Mathf.Deg2Rad)) * 15);
    }
}
