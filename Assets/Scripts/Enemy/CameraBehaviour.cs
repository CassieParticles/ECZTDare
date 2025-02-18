using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : BaseEnemyBehaviour
{
    [SerializeField, Range(0,180)] private float maxAngle;
    [SerializeField, Range(0, 60)] private float turnSpeed = 30;
    [SerializeField, Range(0.1f, 20)] private float pauseDuration = 1;

    private float initialAngle;
    private bool turningCCW;
    private bool turningPaused;

    private void Awake()
    {
        Setup();
    }

    private void FixedUpdate()
    {
        
    }
}
