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

    private GameObject TimerObject;
    private GameObject StealthObject;

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
    }

    public static MainScoreController GetInstance()
    {
        return instance;
    }

    public void StartTimer()
    {
        TimerObject = Instantiate(TimerObjectPrefab);
        StealthObject = Instantiate(StealthObjectPrefab);
        timeTaken = 0;
        stealthScore = 0;
    }

    

    public void StopTimer()
    {
        timeTaken = TimerObject.GetComponent<ScoreTimer>().time;
        stealthScore = StealthObject.GetComponent<StealthScoreTracker>().score;
        Debug.Log("Time taken: " + timeTaken);
        Destroy(TimerObject);
        Destroy(StealthObject);
        TimerObject = null;
        StealthObject = null;
    }

    private void DisplayScore()
    {
        TimerToGrade gradeCalculator = GetComponent<TimerToGrade>();

        gradeCalculator.DisplayGrade(timeTaken,stealthScore);
    }
}
