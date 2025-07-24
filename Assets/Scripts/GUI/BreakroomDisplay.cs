using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BreakroomDisplay : MonoBehaviour
{

    [Tooltip("How long the script waits before starting to show the display when entering a breakroom")]
    public float scoreDisplayDelay = 0.5f;
    [Tooltip("How long the script waits between showing parts of the score, so section number, then type, then the score")]
    public float scoreDisplayGapDuration = 0.3f;
    [Tooltip("How long the script takes to visually scale the numbers up until it reaches the final score for that section")] 
    public float scoreScalingDuration = 1f;


    public List<ScoreData> scores;
    public List<string> scoresText;
    [SerializeField] TextMeshProUGUI TextBox;

    private string sectionNumberText;
    private string sectionTypeText;
    private string scoreUnitText;
    private string scoreValue;

    private void Awake() {
        scores = new List<ScoreData>();
        scoresText = new List<string>();
    }

    public void AddScore(ScoreData score) {
        //if (scores == null) {
        //    scores = new List<ScoreData>();
        //    scoresText = new List<string>();
        //}
        TextBox = GetComponentInChildren<TextMeshProUGUI>();
        scores.Add(score);

        string formattedText = "";

        sectionNumberText = "Section " + scores.Count; //What section
        sectionTypeText = ""; //What type of section it is, stealth or chase
        scoreUnitText = ""; //What unit the section uses, Time or Stealth Score
        scoreValue = ""; //The score, in either time or stealth score
        if (score.chaseSection) {
            sectionTypeText = "Chase";
            scoreUnitText = "Time: ";

            scoreValue = FormatTime(score.chaseTimeSeconds);
            //Format the time in the way we want
            //float time = score.chaseTimeSeconds;
            //int minutes = Mathf.FloorToInt(time / 60F);
            //int seconds = Mathf.FloorToInt(time - minutes * 60);
            //int milliseconds = Mathf.FloorToInt((time * 100) % 100);
            //scoreValue = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);


        } else if (score.stealthSection) {
            sectionTypeText = "Stealth";
            scoreUnitText = "Score: ";


            scoreValue = FormatStealth(score.stealthScore);
            //Format the stealth score in the way we want
            //int stealthScore = score.stealthScore;
            //int thousand = Mathf.FloorToInt(stealthScore / 1000);
            //int remainder = Mathf.FloorToInt(stealthScore % 1000);
            //scoreValue = string.Format("{0} {1}", thousand, remainder);
        }

        formattedText = string.Format("{0}\n{1}\n{2}{3}\n", sectionNumberText, sectionTypeText, scoreUnitText, scoreValue);
        

        scoresText.Add(formattedText);
        StartCoroutine(DisplayAnimation(scores.Count - 1));

    }

    public IEnumerator DisplayAnimation(int index) {
        //Setup the text that comes before the animted one
        TextBox.text = "";
        for (int i = 0; i < scoresText.Count && i < index; i++) {
            TextBox.text += scoresText[i];
        }
        //Display the score piece by piece
        yield return new WaitForSeconds(scoreDisplayDelay);
        TextBox.text += sectionNumberText + "\n";
        yield return new WaitForSeconds(scoreDisplayGapDuration);
        TextBox.text += sectionTypeText + "\n";
        yield return new WaitForSeconds(scoreDisplayGapDuration);
        TextBox.text += scoreUnitText;
        yield return new WaitForSeconds(scoreDisplayGapDuration);

        //Scale up the score until it reaches the achieved score
        float scalingTimer = 0;
        string tempTextboxText = TextBox.text;
        if (scores[scores.Count - 1].chaseSection) {
            //Setup for scaling the time
            float time = scores[scores.Count - 1].chaseTimeSeconds;
            float startTime = time + 60;

            while (scalingTimer < scoreScalingDuration) {
                float scalingProgress = scalingTimer / scoreScalingDuration;
                TextBox.text = tempTextboxText + FormatTime(Mathf.Lerp(startTime, time, scalingProgress));

                scalingTimer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            TextBox.text = tempTextboxText + FormatTime(time);

        } else if (scores[scores.Count - 1].stealthSection) {
            //Setup for scaling the stealth score
            int score = scores[scores.Count - 1].stealthScore;
            int startScore = 0;

            while (scalingTimer < scoreScalingDuration) {
                float scalingProgress = scalingTimer / scoreScalingDuration;
                TextBox.text = tempTextboxText + FormatStealth(Mathf.CeilToInt(Mathf.Lerp(startScore, score, scalingProgress)));

                scalingTimer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            TextBox.text = tempTextboxText + FormatStealth(score);
        }
        




        yield return new WaitForFixedUpdate();
    }

    private string FormatTime(float time) {
        //Format the time in the way we want
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private string FormatStealth(int stealth) {
        int thousand = Mathf.FloorToInt(stealth / 1000);
        int remainder = Mathf.FloorToInt(stealth % 1000);
        int hundred = Mathf.FloorToInt(remainder / 100);
        remainder = Mathf.FloorToInt(hundred % 100);
        int ten = Mathf.FloorToInt(remainder / 10);
        remainder = Mathf.FloorToInt(ten % 10);
        return string.Format("{0} {1}{2}{3}", thousand, hundred, ten, remainder);
    }
}
