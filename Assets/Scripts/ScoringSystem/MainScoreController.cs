using System.Collections.Generic;
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
    bool currentlyScoring = false;
    ScoreTimer timer;
    StealthScoreTracker stealthTracker;

    //Level tracking
    List<float> times;
    List<float> stealthScores;

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
        timer.paused = true;
    }

    public void Unpause()
    {
        timer.paused = false;
    }

    public void EndSection()
    {
        //Only end section if it was tracking
        if(!currentlyScoring){ return; }
        currentlyScoring = false;

        float time = timer.time;
        int stealth = stealthTracker.score;

        //Add scores to list
        times.Add(time);
        stealthScores.Add(stealth);

        Debug.Log(time);
        Debug.Log(stealth);

        //Destroy old stealth objects
        Destroy(timer.gameObject);
        Destroy(stealthTracker.gameObject);

        //TODO: Display score in cool and fancy way
        GameObject cutscene = Instantiate(ScoreCutscenePrefab);
        cutscene.GetComponent<CutsceneControl>().DisplayScore(time, stealth);
    }

    public void EndLevel()
    {
        //If currently on a section, end it
        EndSection();

        //TODO: Ask the user for a 3 letter name (continue in separate function)

        //Collect scores into cumulative score
        float totalTime = 0;
        float totalStealthScore = 0;
        for(int i=0;i<times.Count;++i)
        {
            totalTime += times[i];
            totalStealthScore += stealthScores[i];
        }

        //TODO: Add score to leaderboard

        //TODO: Display leaderboard

        //Destroy Main score controller
        instance = null;
        Destroy(gameObject);
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
        stealthScores = new List<float>();
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
