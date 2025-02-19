using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class CameraBehaviour : BaseEnemyBehaviour
{
    public AK.Wwise.Event cameraMoving;
    public AK.Wwise.Event cameraStop;
    public AK.Wwise.Event enemyAlerted;

    [SerializeField, Range(0,180)] private float maxAngle;
    [SerializeField, Range(0, 60)] private float turnSpeed = 30;
    [SerializeField, Range(0.1f, 20)] private float pauseDuration = 1;

    private float initialAngle;
    private bool turningCCW;
    private bool turningPaused;

   

    private IEnumerator ChangeCameraDirection()
    {
        turningCCW = !turningCCW;
        RotateCamera(turnSpeed * Time.fixedDeltaTime * (turningCCW ? 1 : -1));
        turningPaused = true;

        //Stop camera moving sound
        cameraMoving.Stop(gameObject);
        //Play camera stop moving sound
        cameraStop.Post(gameObject);
        yield return new WaitForSeconds(pauseDuration);
        turningPaused = false;
        //Start camera moving sound
        cameraMoving.Post(gameObject);
    }

    private void RotateCamera(float angle)
    {
        Vector3 rotation = visionCone.transform.rotation.eulerAngles;
        rotation.z += angle;
        visionCone.transform.rotation = Quaternion.Euler(rotation);
    }

    private float GetSuspicionIncrease(Vector3 playerPosition)
    {
        return 50;
    }

    private void Alarm(Vector3 playerPosition)
    {
        Debug.Log("Alarm going off!");
    }

    private void AlarmOff()
    {

    }

    private void Awake()
    {
        Setup();
        initialAngle = visionCone.transform.rotation.eulerAngles.z;
        turningCCW = true;
        turningPaused = maxAngle == 0;

        //Start camera moving sound
        cameraMoving.Post(gameObject);
    }

    private void Start()
    {
        if (alarm)
        {
            alarm.AddAlarmEnableFunc(Alarm);
            alarm.AddAlarmDisableFunc(AlarmOff);
        }
    }

    private void FixedUpdate()
    {
        //Handle seeing the player
        if (Player)
        {
            if (suspicion <= 100)
            {
                suspicion += GetSuspicionIncrease(Player.transform.position) * Time.fixedDeltaTime;
            }
            else
            {
                //Raise alarm
                if (alarm && !alarm.AlarmGoingOff())
                {
                    enemyAlerted.Post(gameObject);
                    alarm.StartAlarm(Player.transform.position);
                }
            }
        }
        else if (suspicion > 0)
        {
            if (alarm && !alarm.AlarmGoingOff())
            {
                suspicion -= suspicionDecayRate * Time.fixedDeltaTime;
            }
            else if (!alarm)
            {
                suspicion -= suspicionDecayRate * Time.fixedDeltaTime;
            }
        }


        //Handle camera rotation
        float visionAngle = visionCone.transform.rotation.eulerAngles.z;
        float upperBound = initialAngle + maxAngle * +1;
        float lowerBound = initialAngle + maxAngle * -1;

        //Case 1: Area between upper and lower bound is continuous
        if (upperBound < 360 && lowerBound > 0)
        {
            if (Mathf.Abs(visionAngle - initialAngle) > maxAngle)
            {
                StartCoroutine(ChangeCameraDirection());
            }
        }
        //Case 2: Upper bound goes past 360 degrees
        else if (upperBound > 360)
        {
            if (visionAngle < lowerBound)
            {
                upperBound -= 360;
            }
            if (visionAngle > upperBound)
            {
                StartCoroutine(ChangeCameraDirection());
            }
        }
        //Case 3: Lower bound goes past 0 degrees
        else
        {
            if (visionAngle > upperBound)
            {
                lowerBound += 360;
            }
            if (visionAngle < lowerBound)
            {
                StartCoroutine(ChangeCameraDirection());
            }
        }

        //Turn camera
        if (!turningPaused)
        {
            RotateCamera(turnSpeed * Time.fixedDeltaTime * (turningCCW ? 1 : -1));
        }
    }
    private void OnDrawGizmosSelected()
    {
        float initialAngle = transform.GetChild(0).rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 initialPos = transform.GetChild(0).position;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(initialPos, initialPos + new Vector3(Mathf.Cos(initialAngle), Mathf.Sin(initialAngle)) * 15);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(initialPos, initialPos + new Vector3(Mathf.Cos(initialAngle + maxAngle * Mathf.Deg2Rad), Mathf.Sin(initialAngle + maxAngle * Mathf.Deg2Rad)) * 15);
        Gizmos.DrawLine(initialPos, initialPos + new Vector3(Mathf.Cos(initialAngle - maxAngle * Mathf.Deg2Rad), Mathf.Sin(initialAngle - maxAngle * Mathf.Deg2Rad)) * 15);
    }

}
