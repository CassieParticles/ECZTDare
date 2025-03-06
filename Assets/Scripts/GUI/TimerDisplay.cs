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
        timerText.text = timerScript.elapsedTime.ToString("00.00");
    }
}
