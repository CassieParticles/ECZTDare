using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPauseArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Get timer
        ScoreTimer scoreTimer = FindAnyObjectByType<ScoreTimer>();
        if (!scoreTimer){ return; }

        scoreTimer.paused = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Get timer
        ScoreTimer scoreTimer = FindAnyObjectByType<ScoreTimer>();
        if (!scoreTimer)
        { return; }

        scoreTimer.paused = false;
    }
}
