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

    public void SeePlayer(GameObject player)
    {
        suspicion += suspicionScalar * Time.fixedDeltaTime;
    }

    private void Awake()
    {
        suspicion = 0.0f;
        suspicionLevel = SuspicionLevel.Idle;
        visionCone = transform.GetChild(0).gameObject;
        turnCCW = true;
        initialAngle = transform.GetChild(0).rotation.eulerAngles.z;
        if (initialAngle > 180)
        {
            initialAngle -= 360;
        }
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

        if (visionAngle.z > 180)
        {
            visionAngle.z -= 360;
        }

        if (Mathf.Abs(visionAngle.z - initialAngle) > maxTurnAngle) {
            turnCCW = !turnCCW; 
        }
        visionAngle.z += turnSpeed * Time.fixedDeltaTime * (turnCCW ? 1 : -1);  //Rotate by
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
