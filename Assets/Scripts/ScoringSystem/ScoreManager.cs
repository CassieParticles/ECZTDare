using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{


    private static ScoreManager instance;
    public static ScoreManager GetInstance() {
        return instance;
    }

    private SaveData currentSaveData = new SaveData();


    private int level = 1;
    public string level1Name = "Level 1";
    public string level2Name = "Boss Level (2v3)";
    private int section = 1;

    public List<ScoreData> scores = new List<ScoreData>();
     
    public void SaveScoresToJson() {
        string filePath = Application.persistentDataPath + "/ScoreData.json"; 

        //Aquire the old save data so it anything that isnt overwritten remains
        if (System.IO.File.Exists(filePath)) {
            string oldSaveDataString = System.IO.File.ReadAllText(filePath);
            if (oldSaveDataString == "" || oldSaveDataString == "{}") {
                string blankSaveDataString = "{\"level1Scores\":[],\"level2Scores\":[]}";
                oldSaveDataString = blankSaveDataString;
                System.IO.File.WriteAllText(filePath, blankSaveDataString);

            }
            currentSaveData = JsonUtility.FromJson<SaveData>(oldSaveDataString);
        }

        if (SceneManager.GetActiveScene().name == level1Name) {
            level = 1;
        } else if (SceneManager.GetActiveScene().name == level2Name) {
            level = 2;
        }

        if (currentSaveData != null) {
            //Set the relevant save data
            if (level == 1) {

                if (currentSaveData.level1Scores.Count == scores.Count) {
                    for (int i = 0; i < scores.Count - 1; i++) {
                        if (scores[i].chaseSection) { //If chase section, compare times
                            if (scores[i].chaseTimeSeconds < currentSaveData.level1Scores[i].chaseTimeSeconds) {
                                currentSaveData.level1Scores[i].chaseTimeSeconds = scores[i].chaseTimeSeconds;
                            }
                        } else { //If stealth section, compare stealth scores
                            if (scores[i].stealthScore > currentSaveData.level1Scores[i].stealthScore) {
                                currentSaveData.level1Scores[i].stealthScore = scores[i].stealthScore;
                            }
                        }
                    }
                } else {
                    currentSaveData.level1Scores = scores;
                }
            } else if (level == 2) {

                if (currentSaveData.level2Scores.Count == scores.Count) {
                    for (int i = 0; i < scores.Count - 1; i++) {
                        if (scores[i].chaseSection) { //If chase section, compare times
                            if (scores[i].chaseTimeSeconds < currentSaveData.level2Scores[i].chaseTimeSeconds) {
                                currentSaveData.level2Scores[i].chaseTimeSeconds = scores[i].chaseTimeSeconds;
                            }
                        } else { //If stealth section, compare stealth scores
                            if (scores[i].stealthScore > currentSaveData.level2Scores[i].stealthScore) {
                                currentSaveData.level2Scores[i].stealthScore = scores[i].stealthScore;
                            }
                        }
                    }
                } else {
                    currentSaveData.level2Scores = scores;
                }
            } else {
                return;
            }
        } else {
            Debug.Log("Saving Score Data Failed");
        }

        //Save the save data to the correct json file
        string saveDataString = JsonUtility.ToJson(currentSaveData);
        System.IO.File.WriteAllText(filePath, saveDataString);
        Debug.Log("Score Data saved successfully to " + filePath);
    }

    public void SaveSectionScore(ScoreData score) {
        if (scores.Count >= section) {
            scores[section - 1] = score;
        } else {
            scores.Add(score);
        }
        //Debug.Log("Saved Section " + section);
        section++;
    }

    //Singleton stuff
    private void Awake() {
        //Ensure only one instance can exist at any one time
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Quit() {
        //Destroy scoreManager when level ends
        Destroy(gameObject);
    }
}

[System.Serializable]
public class ScoreData {
    public bool chaseSection; 
    public bool stealthSection;
    public float chaseTimeSeconds;
    public int stealthScore;
}
