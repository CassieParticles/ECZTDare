using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainScoreController : MonoBehaviour
{
    //Prefabs for instantiation
    [SerializeField] GameObject TimerObjectPrefab;
    [SerializeField] GameObject StealthObjectPrefab;

    [SerializeField] GameObject ScoreCutscenePrefab;

    //static field 
    private static MainScoreController instance;

    //Current section tracking variables
    public bool currentlyScoring { get; private set; } = false;
    ScoreTimer timer;
    StealthScoreTracker stealthTracker;

    //Level tracking
    List<float> times;
    List<int> stealthScores;
    int maxStealth;

    public static MainScoreController GetInstance()
    {
        return instance;
    }
    public void StartSection()
    {
        //Don't start tracking twice
        if (currentlyScoring){ return; }
        currentlyScoring = true;

        timer = Instantiate(TimerObjectPrefab).GetComponent<ScoreTimer>();
        stealthTracker = Instantiate(StealthObjectPrefab).GetComponent<StealthScoreTracker>();
    }

    public void Pause()
    {
        if (timer == null){ return; }
        timer.paused = true;
    }

    public void Unpause()
    {
        if (timer == null){ return; }
        timer.paused = false;
    }

    public void EndSection(bool endOfLevel)
    {
        //Only end section if it was tracking
        if(!currentlyScoring){ return; }
        currentlyScoring = false;

        float time = timer.time;
        int stealth = stealthTracker.score;

        //Add scores to list
        times.Add(time);
        stealthScores.Add(stealth);
        maxStealth += stealthTracker.MaxScore;

        Debug.Log(time);
        Debug.Log(stealth);

        //Destroy old stealth objects
        Destroy(timer.gameObject);
        Destroy(stealthTracker.gameObject);

        //TODO: Display score in cool and fancy way
        GameObject cutscene = Instantiate(ScoreCutscenePrefab);
        cutscene.GetComponent<CutsceneControl>().DisplayScore(time, stealth,endOfLevel);
    }

    public void EndLevel()
    {
        //If currently on a section, end it
        EndSection(true);

        //Collect scores into cumulative score
        float totalTime = 0;
        int totalStealthScore = 0;
        for (int i = 0; i < times.Count; ++i)
        {
            totalTime += times[i];
            totalStealthScore += stealthScores[i];
        }

        GameObject Menu = GameObject.Find("Menu Canvas");
        TextMeshProUGUI speedText = Menu.transform.Find("WinGroup/SpeedScoreText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI stealthText = Menu.transform.Find("WinGroup/StealthScoreText").GetComponent<TextMeshProUGUI>();

        //Format the time taken into mm:ss
        int minutes = (int)totalTime / 60;
        int seconds = (int)totalTime % 60;

        string timeStr = minutes.ToString();    //Minutes
        timeStr += ":";
        timeStr += seconds.ToString();

        //Format the stealth score into Score/MaxScore
        string stealthScore = totalStealthScore.ToString();
        stealthScore += ":";
        stealthScore += maxStealth.ToString();

        //Show score in win screen
        speedText.text = timeStr;
        stealthText.text = stealthScore;

        //Destroy Main score controller
        instance = null;
        Destroy(gameObject);
    }

    public void EndLevelCont()
    {



    }

    public void Quit()
    {
        //Destroy scoring object
        Destroy(gameObject);
    }


    private void Awake()
    {
        //Ensure only one instance can exist at any one time
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        times = new List<float>();
        stealthScores = new List<int>();
        maxStealth = 0;
    }

    private void OnDestroy()
    {
        if(currentlyScoring)
        {
            //Destroy active scoring trackers
            Destroy(timer.gameObject);
            Destroy(stealthTracker.gameObject);
        }
    }
}
