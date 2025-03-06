using System.Collections;
using UnityEngine;

public class CameraBehaviour : BaseEnemyBehaviour
{
    public AK.Wwise.Event cameraMoving;
    public AK.Wwise.Event cameraStop;

    [SerializeField, Range(0,180)] private float maxAngle;
    [SerializeField, Range(0, 60)] private float turnSpeed = 30;
    [SerializeField, Range(0.1f, 20)] private float pauseDuration = 1;

    public bool beingHacked;

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

    private void FollowPlayer()
    {
        if(!Player)
        {
            return;
        }
        Vector3 position = Player.transform.position;
        Vector3 direction = position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        visionCone.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Alarm(Vector3 playerPosition)
    {
        SetSuspicionState(SuspicionState.HighAlert);
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
        //If suspicion is high and can see the player, follow the player rather than turn normally
        bool FollowingPlayer = false;
        //Handle seeing the player
        if (Player)
        {
            FollowingPlayer = suspicionState == SuspicionState.Chase && Player;
            if (suspicion <= 100)
            {
                CalcSuspicionIncrease();
            }
            else
            {
                //Raise alarm
                if (alarm && !alarm.AlarmGoingOff())
                {
                    alarm.StartAlarm(Player.transform.position);
                }
            }
        }
        else if (suspicion > 0)
        {
            if(!alarm||!alarm.AlarmGoingOff())
            {
                CalcSuspicionDecay();
            }
        }

        //Handle state and colour changes
        BaseUpdate();

        //Handle camera rotation
        float target = initialAngle + (turningCCW ? maxAngle : -maxAngle);
        if(target > 360){ target -= 360; }
        if(target < 0){ target += 360; }

        if(FollowingPlayer)
        {
            FollowPlayer();
        }
        else if(!turningPaused)
        { 
            RotateCamera(turnSpeed * Time.fixedDeltaTime * (turningCCW ? 1 : -1));
        }

        //Change directions
        if(Mathf.Abs(visionCone.transform.rotation.eulerAngles.z - target) < 1f)
        {
            StartCoroutine(ChangeCameraDirection());
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
