using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class CameraBehavior : MonoBehaviour
{
    public AK.Wwise.Event inViewCone;
    public AK.Wwise.Event cameraMoving;

    //Suspicion
    [SerializeField]
    private float[] thresholds = new float[3] { 33.3f, 66.6f, 100.0f };
    [SerializeField, Range(0, 200)]
    private float suspicionScalar = 20.0f;
    [SerializeField]
    private bool ConnectedToAlarm = true;

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

    private GameObject player;

    private GameObject visionCone;
    private float initialAngle;
    bool turnCCW;
    bool turnPause;

    AlarmSystem alarm;

    public void SeePlayer(GameObject player)
    {
        this.player = player;
        //Start inside vision cone sound
        inViewCone.Post(gameObject);
    }

    public void LosePlayer()
    {
        this.player = null;
        //Stop inside vision cone sound
        inViewCone.Stop(gameObject);

        //BELOW IS A NOTE
        //Sets the "Music" State Group's active State to "Alarm"
        //AkSoundEngine.SetState("Music", "Alarm");
    }

    private void Alarm(Vector3 playerPosition)
    {
        Debug.Log("Alarm has been sounded");
    }

    private IEnumerator PauseCamera()
    {
        //Stop camera moving sound
        cameraMoving.Stop(gameObject);
        //Play camera stop moving sound
        AkSoundEngine.PostEvent("Camera_Stop", this.gameObject);
        turnPause = false;
        yield return new WaitForSeconds(pauseDuration);
        turnPause = true;
        turnCCW = !turnCCW;
        //Start camera moving sound
        cameraMoving.Post(gameObject);
    }

    private void Awake()
    {
        suspicion = 0.0f;
        suspicionLevel = SuspicionLevel.Idle;
        visionCone = transform.GetChild(0).gameObject;

        turnCCW = true;
        initialAngle = transform.GetChild(0).rotation.eulerAngles.z;
        turnPause = true;

        if (ConnectedToAlarm)
        {
            alarm = AlarmSystem.GetAlarmSystem();
        }

        //Start camera moving sound
        cameraMoving.Post(gameObject);
    }

    private void Start()
    {
        //Add the alarm function to the camera
        if (alarm)
        {
            alarm.AddAlarmEnableFunc(Alarm);
        }
    }

    private void FixedUpdate()
    {
        //suspicion handling

        //Can see player, increase suspicion
        if(player !=null)
        {
            suspicion += suspicionScalar * Time.fixedDeltaTime;
        }

        //Increase suspicion level and raise alarm if full
        if(suspicionLevel == SuspicionLevel.Alarm)
        {
            //Alarm is currently being raised
            if(alarm && !alarm.AlarmGoingOff())
            {
                //Play enemy alerted sound (currently it instantly raises the alarm, but a delay can be added)
                alarm.StartAlarm(player.transform.position);
            }
        }
        else if(suspicion > thresholds[(int)suspicionLevel])
        {
            suspicionLevel++;
        }

        //Turning handling
        Vector3 visionAngle = visionCone.transform.rotation.eulerAngles;
        float upperBound = initialAngle + maxTurnAngle * +1;
        float lowerBound = initialAngle + maxTurnAngle * -1;
        float usedBound = turnCCW ? upperBound : lowerBound;


        //3 cases
        //1.) Turning range doesn't cross 0/360 bound
        //2.) Upper bound crosses 0/360 bound
        //3.) Lower bound crosses 0/360 bound

        //Case 1, check can be simplified
        if (upperBound < 360 && lowerBound > 0)
        {
            if(Mathf.Abs(visionAngle.z - initialAngle) > maxTurnAngle)
            {
                visionAngle.z += turnSpeed * Time.fixedDeltaTime * (turnCCW ? -1 : 1);
                StartCoroutine(PauseCamera());
            }
        }
        //Case 2 (case 3 doesn't affect this)
        else if (upperBound > 360)
        {
            if(visionAngle.z < lowerBound)
            {
                upperBound -= 360;
            }
            if(visionAngle.z > upperBound)
            {
                visionAngle.z += turnSpeed * Time.fixedDeltaTime * (turnCCW ? -1 : 1);
                StartCoroutine(PauseCamera());
            }
        }
        //Case 3(case 2 doesn't affect this)
        else
        {
            if(visionAngle.z > upperBound)
            {
                lowerBound += 360;
            }
            if(visionAngle.z < lowerBound)
            {
                visionAngle.z += turnSpeed * Time.fixedDeltaTime * (turnCCW ? -1 : 1);
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
