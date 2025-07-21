using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{


    private static ScoreManager instance;
    public static ScoreManager GetInstance() {
        return instance;
    }

    private SaveData currentSaveData = new SaveData();


    public int level = 1;
    private int section = 1;

    public List<ScoreData> scores = new List<ScoreData> { 
            null, //Section 1
            null, //Section 2
            null  //Etc.
    };

    //Just collapse this its needed for save data but its not helpful otherwise
    public SaveData blankSaveData = new SaveData(); //{
    //    level1Scores = { new ScoreData {
    //        chaseSection = true, 
    //        chaseTimeSeconds = -1, 
    //        stealthScore = -1  
    //}, new ScoreData {
    //        chaseSection = false,
    //        chaseTimeSeconds = -1,
    //        stealthScore = -1
    //}, new ScoreData {
    //        chaseSection = true,
    //        chaseTimeSeconds = -1,
    //        stealthScore = -1
    //} },
    //    level2Scores = { new ScoreData {
    //        chaseSection = false,
    //        chaseTimeSeconds = -1,
    //        stealthScore = -1
    //}, new ScoreData {
    //        chaseSection = true,
    //        chaseTimeSeconds = -1,
    //        stealthScore = -1
    //}, new ScoreData {
    //        chaseSection = false,
    //        chaseTimeSeconds = -1,
    //        stealthScore = -1
    //} }
    //};

    public void SaveScoresToJson(int savefile) {
        string filePath = Application.persistentDataPath + "/ScoreData" + savefile + ".json"; //For example ScoreData1.json

        //Aquire the old save data so it anything that isnt overwritten remains
        if (!System.IO.File.Exists(filePath)) {
            System.IO.File.Create(filePath);
            string blankSaveDataString = JsonUtility.ToJson(blankSaveData);
            System.IO.File.WriteAllText(filePath, blankSaveDataString);
            //Debug.Log("blankSaveData: " + blankSaveDataString);
        } else {
            string oldSaveDataString = System.IO.File.ReadAllText(filePath);
            if (oldSaveDataString == "" || oldSaveDataString == "{}" || oldSaveDataString == "{\"level1Scores\":[],\"level2Scores\":[]}") {
                string blankSaveDataString = JsonUtility.ToJson(blankSaveData);
                System.IO.File.WriteAllText(filePath, blankSaveDataString);
            }
            currentSaveData = JsonUtility.FromJson<SaveData>(oldSaveDataString);
            Debug.Log("oldSaveData: " + oldSaveDataString);
        }

        //Set the relevant save data
        if (level == 1) {
            currentSaveData.level1Scores = scores;
        } else if (level == 2) {
            currentSaveData.level2Scores = scores;
        } else {
            return;
        }

        //Save the save data to the correct json file
        string saveDataString = JsonUtility.ToJson(currentSaveData);
        Debug.Log("savedData: " + saveDataString);
        System.IO.File.WriteAllText(filePath, saveDataString);
        Debug.Log("Score Data saved successfully to " +  filePath);
    }

    public void SaveSectionScore(ScoreData score) {
        scores[section - 1] = score;
        Debug.Log("Saved Section " + section);
        section++;

        CreateSaveFile(1);
    }

    public void CreateSaveFile(int savefile) {
        string filePath = Application.persistentDataPath + "/ScoreData" + savefile + ".json";
        if (!System.IO.File.Exists(filePath)) {
            System.IO.File.Create(filePath);
        }
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

    private void Start()
    {
        FindAnyObjectByType<LevelManager>().AddCallback(ChangeLevel);
    }

    public void Quit() {
        //Destroy scoreManager when level ends
        Destroy(gameObject);
        FindAnyObjectByType<LevelManager>().RemoveCallback(ChangeLevel);
    }

    private void ChangeLevel(LevelManager.Levels level, bool reload)
    {
        if(level == LevelManager.Levels.MainMenu)
        {
            Quit();
        }
    }
}
[System.Serializable]
public enum scoreType {
    chase,
    stealth
}
[System.Serializable]
public class ScoreData {
    public bool chaseSection; 
    public bool stealthSection;
    public float chaseTimeSeconds;
    public int stealthScore;
}
