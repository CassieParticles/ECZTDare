using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public float elapsedTime;

    void Start()
    {
        elapsedTime = 0;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }
}
