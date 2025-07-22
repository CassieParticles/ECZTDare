using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BreakroomDisplay : MonoBehaviour
{

    [Tooltip("How long the script waits before starting to show the display when entering a breakroom")]
    public float scoreDisplayDelay = 0.5f;
    [Tooltip("How long the script takes to visually scale the numbers up until it reaches the final score for that section")] 
    public float scoreScalingDuration = 1f;


    List<ScoreData> scores;
    List<string> scoresText;
    [SerializeField] TextMeshProUGUI TextBox;

    public void AddScore(ScoreData score) {
        scores.Add(score);

        string formattedText = "";

        string sectionNumberText = "Section " + scores.Count; //What section
        string sectionTypeText = ""; //What type of section it is, stealth or chase
        string scoreUnitText = ""; //What unit the section uses, Time or Stealth Score
        string scoreValue = ""; //The score, in either time or stealth score
        if (score.chaseSection) {
            sectionTypeText = "Chase";
            scoreUnitText = "Time: ";
            
            //Format the time in the way we want
            float time = score.chaseTimeSeconds;
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            int milliseconds = Mathf.FloorToInt((time * 100) % 100);
            scoreValue = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);


        } else if (score.stealthSection) {
            sectionTypeText = "Stealth";
            scoreUnitText = "Score: ";

            //Format the stealth score in the way we want
            int stealthScore = score.stealthScore;
            int thousand = Mathf.FloorToInt(stealthScore / 1000);
            int remainder = Mathf.FloorToInt(stealthScore % 1000);
            scoreValue = string.Format("{0} {1}", thousand, remainder);
        }

        formattedText = string.Format("{0}\n{1}\n{2}{3}", sectionNumberText, sectionTypeText, scoreUnitText, scoreValue);
        

        scoresText.Add(formattedText);

        //TODO: Make it iterate through the list to display all scores
        TextBox.text = formattedText;

    }

    private IEnumerator DisplayAnimation(int index) {
        yield return new WaitForFixedUpdate();
    }
}
