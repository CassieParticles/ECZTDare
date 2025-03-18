using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTimer : MonoBehaviour
{
    public float time {  get; private set; }
    [NonSerialized] public bool paused;
    // Update is called once per frame

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        time += paused ? 0 : Time.deltaTime;
        TimerDisplay timerDisplay = FindFirstObjectByType<TimerDisplay>();
        if (timerDisplay)
        {
            timerDisplay.UpdateGUI(time);
        }
    }
}
