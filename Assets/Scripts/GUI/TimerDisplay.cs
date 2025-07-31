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
        int minutes = Mathf.FloorToInt(time / 60);
        int minuteTens = Mathf.FloorToInt(minutes / 10);
        int minuteOnes = Mathf.FloorToInt(minutes % 10);

        int seconds = Mathf.FloorToInt(time - minutes * 60);
        int secondTens = Mathf.FloorToInt(seconds / 10);
        int secondOnes = Mathf.FloorToInt(seconds % 10);

        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        int millisecondTens = Mathf.FloorToInt(milliseconds / 10);
        int millisecondOnes = Mathf.FloorToInt(milliseconds % 10);

        string niceTime = string.Format("{0}{1}:{2}{3}:{4}{5}", minuteTens.ToString(), minuteOnes.ToString(), secondTens.ToString(), secondOnes.ToString(), millisecondTens.ToString(), millisecondOnes.ToString());

        timerText.text = niceTime;
    }
}
