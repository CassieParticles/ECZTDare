using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BreakroomDisplay : MonoBehaviour
{
    public struct SectionStrings {
        public string sectionNumberText;
        public string sectionTypeText;
        public string scoreUnitText;
        public string scoreValue;
    }


    [Tooltip("How long the script waits before starting to show the display when entering a breakroom")]
    public float scoreDisplayDelay = 0.5f;
    [Tooltip("How long the script waits between showing parts of the score, so section number, then type, then the score")]
    public float scoreDisplayGapDuration = 0.3f;
    [Tooltip("How long the script takes to visually scale the numbers up until it reaches the final score for that section")] 
    public float scoreScalingDuration = 1f;

    [HideInInspector] public bool scoringCoroutineRunning;


    public List<ScoreData> scores;
    public List<string> scoresText;
    public List<string> lettersText;
    public List<SectionStrings> sectionText;
    [SerializeField] TextMeshProUGUI TextBox;
    [SerializeField] TextMeshProUGUI LetterTextbox;

    private string sectionNumberText;
    private string sectionTypeText;
    private string scoreUnitText;
    private string scoreValue;

    private void Awake() {
        if (scores == null) {
            scores = new List<ScoreData>();
            scoresText = new List<string>();
            sectionText = new List<SectionStrings>();
        }
    }

    public void AddScore(List<ScoreData> importedScores) {
        scoringCoroutineRunning = true;
        TextBox = GetComponentInChildren<TextMeshProUGUI>();
        scores = importedScores;
        scoresText = new List<string>();
        sectionText = new List<SectionStrings>();
        lettersText = new List<string>();

        ScoreData score = scores[scores.Count - 1];

        ScoreManager scoreManager = ScoreManager.GetInstance();

        string formattedText = "";

        for (int i = 0; i <= scores.Count - 1; i++) {
            sectionText.Add(new SectionStrings());
            SectionStrings tempSectionString = new SectionStrings();
            tempSectionString.sectionNumberText = "Section " + (i + 1); //What section
            tempSectionString.sectionTypeText = ""; //What type of section it is, stealth or chase
            tempSectionString.scoreUnitText = ""; //What unit the section uses, Time or Stealth Score
            tempSectionString.scoreValue = ""; //The score, in either time or stealth score

            if (scores[i].chaseSection) {
                tempSectionString.sectionTypeText = "Chase";
                tempSectionString.scoreUnitText = "Time: ";
                tempSectionString.scoreValue = FormatTime(scores[i].chaseTimeSeconds);
                lettersText.Add(scoreManager.TimeToLetter(scores[i].chaseTimeSeconds) + "\n\n");

            } else if (scores[i].stealthSection) {
                tempSectionString.sectionTypeText = "Stealth";
                tempSectionString.scoreUnitText = "Score: ";
                tempSectionString.scoreValue = FormatStealth(scores[i].stealthScore);
                lettersText.Add(scoreManager.ScoreToLetter(scores[i].stealthScore) + "\n\n");
            }
            sectionText[i] = tempSectionString;

            formattedText = string.Format("{0}\n{1}\n{2}{3}\n\n", tempSectionString.sectionNumberText, tempSectionString.sectionTypeText, tempSectionString.scoreUnitText, tempSectionString.scoreValue);
            scoresText.Add(formattedText);

            

            
        }


        


        StartCoroutine(DisplayAnimation(scores.Count - 1));

    }

    public AK.Wwise.Event sRankBeep;
    public AK.Wwise.Event goodBeep;
    public AK.Wwise.Event badBeep;

    public IEnumerator DisplayAnimation(int index) {
        scoringCoroutineRunning = true;
        //Setup the text that comes before the animted one
        TextBox.text = "";
        LetterTextbox.text = "";
        for (int i = 0; i < scoresText.Count && i < index; i++) {
            TextBox.text += scoresText[i];
            LetterTextbox.text += lettersText[i];
        }
        //Display the score piece by piece
        yield return new WaitForSecondsRealtime(scoreDisplayDelay);
        TextBox.text += sectionText[scores.Count - 1].sectionNumberText + "\n";
        yield return new WaitForSecondsRealtime(scoreDisplayGapDuration);
        TextBox.text += sectionText[scores.Count - 1].sectionTypeText + "\n";
        yield return new WaitForSecondsRealtime(scoreDisplayGapDuration);
        TextBox.text += sectionText[scores.Count - 1].scoreUnitText;
        yield return new WaitForSecondsRealtime(scoreDisplayGapDuration);

        ScoreManager scoreManager = ScoreManager.GetInstance();

        //Scale up the score until it reaches the achieved score
        float scalingTimer = 0;
        string tempTextboxText = TextBox.text;
        string tempLetterText = LetterTextbox.text;
        string letter = "";
        if (scores[scores.Count - 1].chaseSection) {
            //Setup for scaling the time
            float time = scores[scores.Count - 1].chaseTimeSeconds;
            float startTime = time + 60;

            while (scalingTimer < scoreScalingDuration) {
                float scalingProgress = scalingTimer / scoreScalingDuration;
                float scaledTime = Mathf.Lerp(startTime, time, scalingProgress);
                TextBox.text = tempTextboxText + FormatTime(scaledTime);
                LetterTextbox.text = tempLetterText + scoreManager.TimeToLetter(scaledTime);

                scalingTimer += Time.fixedUnscaledDeltaTime;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            
            TextBox.text = tempTextboxText + FormatTime(time);
            letter = scoreManager.TimeToLetter(time);
            LetterTextbox.text = tempLetterText + letter;

        } else if (scores[scores.Count - 1].stealthSection) {
            //Setup for scaling the stealth score
            int score = scores[scores.Count - 1].stealthScore;
            int startScore = 0;

            while (scalingTimer < scoreScalingDuration) {
                float scalingProgress = scalingTimer / scoreScalingDuration;
                int scaledScore = Mathf.CeilToInt(Mathf.Lerp(startScore, score, scalingProgress));
                TextBox.text = tempTextboxText + FormatStealth(scaledScore);
                LetterTextbox.text = tempLetterText + scoreManager.ScoreToLetter(scaledScore);

                scalingTimer += Time.fixedUnscaledDeltaTime;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            TextBox.text = tempTextboxText + FormatStealth(score);
            letter = scoreManager.ScoreToLetter(score);
            LetterTextbox.text = tempLetterText + letter;

        }

        // REBECCA ADD AUDIO FOR FINAL LETTER SOUND HERE
        if (letter == "S") {
            //S rank
            sRankBeep.Post(gameObject);
        } else if (letter == "A") {
            //A rank
            goodBeep.Post(gameObject);
        } else if (letter == "B") {
            //guess
            badBeep.Post(gameObject);
        } else {
            //C rank
            badBeep.Post(gameObject);
        }
        

        yield return new WaitForSecondsRealtime(0.5f);
        scoringCoroutineRunning = false;
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
