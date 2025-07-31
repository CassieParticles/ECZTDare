using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI timerText;

    // Update is called once per frame
    public void UpdateGUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        string niceTime = string.Format("{0:00}:{1:00}:{2:00}", minutes.ToString(), seconds.ToString(), milliseconds.ToString());

        timerText.text = niceTime;
    }
}
