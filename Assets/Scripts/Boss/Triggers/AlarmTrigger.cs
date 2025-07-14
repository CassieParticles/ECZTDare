
using UnityEngine;

class AlarmTrigger : BaseTrigger
{
    [SerializeField] private AlarmSystem alarm;

    void AlarmOn(Vector3 playerPosition, GameObject alarmCaller)
    {
        reciever.RecieveSignal(anchor);
    }

private void Start()
    {
        if(alarm)
        {
            //Attach listener function
            alarm.AddAlarmEnableFunc(AlarmOn);
        }
    }
}
