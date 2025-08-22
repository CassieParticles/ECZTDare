using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class ComparisonDisplay : MonoBehaviour
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

    private const float imageWidth = 482;
    private const float imageX = -108;
    private void Awake() {

        if (scores == null) {
            scores = new List<ScoreData>();
            scoresText = new List<string>();
            sectionText = new List<SectionStrings>();
        }
    }

    public void SaveHighscoresFromJson() {
        TextBox = GetComponentInChildren<TextMeshProUGUI>();
        scoresText = new List<string>();
        sectionText = new List<SectionStrings>();
        lettersText = new List<string>();

        bool level1 = true;
        //Get the scores from the saved json file
        string filePath = Application.persistentDataPath + "/ScoreData.json";
        if (System.IO.File.Exists(filePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(filePath))) {
            string saveDataString = System.IO.File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(saveDataString);
            if (SceneManager.GetActiveScene().name == "Level 1") {
                scores = saveData.level1Scores;
                level1 = true;
            } else {
                scores = saveData.level2Scores;
                level1 = false;
            }
        }


        ScoreManager scoreManager = ScoreManager.GetInstance();

        string formattedText = "";

        if (scores != null && scores.Count != 0) {
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
        } else { //If no section score exists
            for (int i = 0; i < 3; i++) {
                sectionText.Add(new SectionStrings());
                SectionStrings tempSectionString = new SectionStrings();
                tempSectionString.sectionNumberText = "Section " + (i + 1);
                tempSectionString.sectionTypeText = ""; //What type of section it is, stealth or chase
                tempSectionString.scoreValue = ""; //What unit the section uses, Time or Stealth Score
                tempSectionString.scoreUnitText = ""; //What unit the section uses, Time or Stealth Score
                //Is a chase section
                if ((level1 && (i == 0 || i == 2)) || (!level1 && i == 1)) {
                    tempSectionString.sectionTypeText = "Chase";
                    scores.Add(new ScoreData() {
                        chaseSection = true,
                        stealthSection = false,
                        chaseTimeSeconds = 10000, //If you spend 3 hours on a level I wouldnt say you got a new highscore either
                        stealthScore = -10000
                    });
                } else { //Is a stealth section
                    tempSectionString.sectionTypeText = "Stealth";
                    scores.Add(new ScoreData() {
                        chaseSection = false,
                        stealthSection = true,
                        chaseTimeSeconds = 10000, //If you spend 3 hours on a level I wouldnt say you got a new highscore either
                        stealthScore = -10000
                    });
                }
                tempSectionString.scoreValue = "No Score";
                sectionText[i] = tempSectionString;
                formattedText = string.Format("{0}\n{1}\n{2}\n\n", tempSectionString.sectionNumberText, tempSectionString.sectionTypeText, tempSectionString.scoreValue);
                scoresText.Add(formattedText);
                lettersText.Add("N/A\n\n");
            }
        }


        


        //DisplayAnimation(scores.Count - 1);

    }

    public void DisplayScore(int index) {
        //Setup the text that comes before the animted one
        TextBox.text = "";
        LetterTextbox.text = "";
        for (int i = 0; i < scoresText.Count && i < index; i++) {
            TextBox.text += scoresText[i];
            LetterTextbox.text += lettersText[i];
        }
        //Display the score piece by piece
        //yield return new WaitForSecondsRealtime(scoreDisplayDelay);
        TextBox.text += sectionText[index].sectionNumberText + "\n";
        //yield return new WaitForSecondsRealtime(scoreDisplayGapDuration);
        TextBox.text += sectionText[index].sectionTypeText + "\n";
        //yield return new WaitForSecondsRealtime(scoreDisplayGapDuration);
        if (scores[index].chaseTimeSeconds != 10000 && scores[index].stealthScore != -10000) {
            TextBox.text += sectionText[index].scoreUnitText;
        } else {
            TextBox.text += sectionText[index].scoreValue;
        }
        //yield return new WaitForSecondsRealtime(scoreDisplayGapDuration);

        ScoreManager scoreManager = ScoreManager.GetInstance();

        //Scale up the score until it reaches the achieved score
        //float scalingTimer = 0;
        string tempTextboxText = TextBox.text;
        string tempLetterText = LetterTextbox.text;
        string letter = "";
        if (scores[index].chaseSection) {
            //Setup for scaling the time
            
            
            //float startTime = Mathf.Max(time + 60, scoreManager.BRankTime + 30);

            //while (scalingTimer < scoreScalingDuration) {
            //    float scalingProgress = scalingTimer / scoreScalingDuration;
            //    float scaledTime = Mathf.Lerp(startTime, time, scalingProgress);
            //    TextBox.text = tempTextboxText + FormatTime(scaledTime);
            //    LetterTextbox.text = tempLetterText + scoreManager.TimeToLetter(scaledTime);

            //    scalingTimer += Time.fixedUnscaledDeltaTime;
            //    yield return new WaitForSecondsRealtime(0.016f);
            //}
            if (scores[index].chaseTimeSeconds != 10000) {
                float time = scores[index].chaseTimeSeconds;
                TextBox.text = tempTextboxText + FormatTime(time);
            }
            letter = lettersText[index];
            LetterTextbox.text = tempLetterText + letter;


        } else if (scores[index].stealthSection) {
            //Setup for scaling the stealth score
            
            //int startScore = 0;

            //while (scalingTimer < scoreScalingDuration) {
            //    float scalingProgress = scalingTimer / scoreScalingDuration;
            //    int scaledScore = Mathf.CeilToInt(Mathf.Lerp(startScore, score, scalingProgress));
            //    TextBox.text = tempTextboxText + FormatStealth(scaledScore);
            //    LetterTextbox.text = tempLetterText + scoreManager.ScoreToLetter(scaledScore);

            //    scalingTimer += Time.fixedUnscaledDeltaTime;
            //    yield return new WaitForSecondsRealtime(0.016f);
            //}
            if (scores[index].stealthScore != -10000) {
                int score = scores[index].stealthScore;
                TextBox.text = tempTextboxText + FormatStealth(score);
            }
            letter = lettersText[index];
            LetterTextbox.text = tempLetterText + letter;

        }

        //yield return new WaitForSecondsRealtime(0.5f);
    }

    public void ClearTextBoxes() {
        TextBox.text = "";
        LetterTextbox.text = "";
    }

    private string FormatTime(float time) {
        //Format the time in the way we want
        int minutes = Mathf.FloorToInt(time / 60);
        int minuteTens = Mathf.FloorToInt(minutes / 10);
        int minuteOnes = Mathf.FloorToInt(minutes % 10);

        int seconds = Mathf.FloorToInt(time - minutes * 60);
        int secondTens = Mathf.FloorToInt(seconds / 10);
        int secondOnes = Mathf.FloorToInt(seconds % 10);

        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        int millisecondTens = Mathf.FloorToInt(milliseconds / 10);
        int millisecondOnes = Mathf.FloorToInt(milliseconds % 10);

        

        return string.Format(string.Format("{0}{1}:{2}{3}:{4}{5}", 
            minuteTens.ToString(), minuteOnes.ToString(), 
            secondTens.ToString(), secondOnes.ToString(), 
            millisecondTens.ToString(), millisecondOnes.ToString()
            ));
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
