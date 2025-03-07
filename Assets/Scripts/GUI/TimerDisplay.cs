using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Timer timerScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        int minutes = Mathf.FloorToInt(timerScript.elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(timerScript.elapsedTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt((timerScript.elapsedTime * 1000) % 1000);
        string niceTime = string.Format("{0:0}:{1:00}:{2:000}", minutes, seconds, milliseconds);

        timerText.text = niceTime;
    }
}
