using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerToGrade : MonoBehaviour
{
    //S, A, B, C
    [SerializeField] private float[] TimeBands = new float[3]
        { 
            60,
            120,
            180
        };

    private TextMeshProUGUI textToChange;

    private void Awake()
    {
        textToChange = GameObject.Find("TimeTaken").GetComponent<TextMeshProUGUI>();
    }
    public void DisplayGrade(float timeTaken)
    {
        if (timeTaken < TimeBands[0])
        {
            textToChange.text = "S";
        }
        else if(timeTaken < TimeBands[1])
        {
            textToChange.text = "A";
        }
        else if (timeTaken < TimeBands[1])
        {
            textToChange.text = "B";
        }
        else
        {
            textToChange.text = "C";
        }
    }
}
