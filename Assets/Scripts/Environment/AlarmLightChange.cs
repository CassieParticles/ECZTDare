using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlarmLightChange : MonoBehaviour
{
    [SerializeField] private GameObject alarmLights;
    [SerializeField] private Color alarmColour;
    private Color defaultColour;

    Light2D[] lights;

    AlarmSystem alarmSystem;

    private void AlarmOn(Vector3 playerPosition, GameObject alarmCaller)
    {
        foreach(Light2D light in lights)
        {
            light.color = alarmColour;
        }
    }

    private void AlarmOff()
    {
        foreach (Light2D light in lights)
        {
            light.color = defaultColour;
        }
    }

    private void Awake()
    {
        alarmSystem = GetComponent<AlarmSystem>();

        //Get attached lights
        lights = alarmLights.transform.GetComponentsInChildren<Light2D>();
        defaultColour = lights[0].color;
    }

    private void Start()
    {
        alarmSystem.AddAlarmEnableFunc(AlarmOn);
        alarmSystem.AddAlarmDisableFunc(AlarmOff);

    }
}

    
