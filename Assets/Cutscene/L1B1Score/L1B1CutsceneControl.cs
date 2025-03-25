using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class L1B1CutsceneControl : MonoBehaviour
{
    [SerializeField] GameObject ScoreGUIPrefab;

    GameObject ScoreGUI;

    public void DisplayScore(float timeTaken, int stealthScore)
    {
        //Create GUI and get text
        ScoreGUI = Instantiate(ScoreGUIPrefab);
        GameObject TimeScoreText = ScoreGUI.transform.GetChild(1).gameObject;
        GameObject StealthScoreText = ScoreGUI.transform.GetChild(2).gameObject;

        //Update text with score
        TimeScoreText.GetComponent<TextMeshProUGUI>().text += timeTaken.ToString();
        StealthScoreText.GetComponent<TextMeshProUGUI>().text += stealthScore.ToString();
    }
}
