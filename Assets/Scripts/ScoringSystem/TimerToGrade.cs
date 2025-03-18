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
    //S, A, B, C
    [SerializeField] private float[] StealthBands = new float[3]
    {
        100,
        80,
        60
    };
    public void DisplayGrade(float timeTaken, float stealthScore)
    {
        TextMeshProUGUI textToChange = GameObject.Find("TimeTaken").GetComponent<TextMeshProUGUI>();
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

        textToChange.text += " : ";

        if(stealthScore > StealthBands[0])
        {
            textToChange.text += "S";
        }
        else if (stealthScore > StealthBands[1])
        {
            textToChange.text += "A";
        }
        else if (stealthScore > StealthBands[1])
        {
            textToChange.text += "B";
        }
        else
        {
            textToChange.text += "C";
        }
    }
}
