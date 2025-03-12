using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScoreController : MonoBehaviour
{
    private static MainScoreController instance;
    [SerializeField] GameObject TimerObjectPrefab;

    private GameObject TimerObject;

    private float timeTaken;
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
        timeTaken = 0;
    }

    

    public void StopTimer()
    {
        timeTaken = TimerObject.GetComponent<ScoreTimer>().time;
        Debug.Log("Time taken: " + timeTaken);
        Destroy(TimerObject);
        TimerObject = null;
    }

    private void DisplayScore()
    {
        TextMeshProUGUI text = GameObject.Find("TimeTaken").GetComponent<TextMeshProUGUI>();
        TimerToGrade gradeCalculator = GameObject.Find("TimerGrade").GetComponent<TimerToGrade>();
        gradeCalculator.DisplayGrade(timeTaken);
    }
}
