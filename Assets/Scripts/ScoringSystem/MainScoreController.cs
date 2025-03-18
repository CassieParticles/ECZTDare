using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScoreController : MonoBehaviour
{
    private static MainScoreController instance;
    [SerializeField] GameObject TimerObjectPrefab;
    [SerializeField] GameObject StealthObjectPrefab;

    private ScoreTimer TimerObject;
    private StealthScoreTracker StealthObject;

    private float timeTaken;
    private float stealthScore;
    private void Awake()
    {
        //Ensure object always exists
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += ChangeLevel;
    }

    void ChangeLevel(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Level1")
        {
            StartTimer();
        }
        if(scene.name == "WinScreen")
        {
            DisplayScore();
        }
        if(scene.name=="LoseScene")
        {
            PauseTimer();
        }
    }

    public static MainScoreController GetInstance()
    {
        return instance;
    }

    public void StartTimer()
    {
        //First load
        if(TimerObject== null)
        {
            TimerObject = Instantiate(TimerObjectPrefab).GetComponent<ScoreTimer>();
            StealthObject = Instantiate(StealthObjectPrefab).GetComponent<StealthScoreTracker>();
            timeTaken = 0;
            stealthScore = 0;
        }
        else//Unpause (coming back from dead)
        {
            TimerObject.paused = false;
        }
    }

    public void PauseTimer()
    {
        TimerObject.paused = true;
    }

    public void StopTimer()
    {
        timeTaken = TimerObject.time;
        stealthScore = StealthObject.score;
        Debug.Log("Time taken: " + timeTaken);
        Destroy(TimerObject.gameObject);
        Destroy(StealthObject.gameObject);
        TimerObject = null;
        StealthObject = null;
    }

    private void DisplayScore()
    {
        TimerToGrade gradeCalculator = GetComponent<TimerToGrade>();

        gradeCalculator.DisplayGrade(timeTaken,stealthScore);
    }
}
