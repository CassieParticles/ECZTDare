using JetBrains.Annotations;
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

    public void SaveScoresToJson(int savefile) {
        string filePath = Application.persistentDataPath + "/ScoreData" + savefile + ".json"; //For example ScoreData1.json

        //Aquire the old save data so it anything that isnt overwritten remains
        string oldSaveDataString = System.IO.File.ReadAllText(filePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(oldSaveDataString);

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
        System.IO.File.WriteAllText(filePath, saveDataString);
        Debug.Log("Score Data saved successfully to " +  filePath);
    }

    public void SaveSectionScore(ScoreData score) {
        scores[section - 1] = score;
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

public enum scoreType {
    chase,
    stealth
}
public class ScoreData {
    public scoreType type;
    public float chaseTimeSeconds;
    public int stealthScore;
}
