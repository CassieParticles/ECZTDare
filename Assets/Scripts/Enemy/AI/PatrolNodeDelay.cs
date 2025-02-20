using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolNodeDelay : MonoBehaviour
{
    [SerializeField] private float delayTime;

    public float getDelay()
    {
        return delayTime;
    }
}
