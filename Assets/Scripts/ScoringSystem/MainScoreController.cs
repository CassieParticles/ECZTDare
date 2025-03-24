using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScoreController : MonoBehaviour
{
    //Prefabs for instantiation
    [SerializeField] GameObject TimerObjectPrefab;
    [SerializeField] GameObject StealthObjectPrefab;

    //static field 
    private static MainScoreController instance;

    //Current section tracking variables
    bool currentlyScoring;
    ScoreTimer timer;
    StealthScoreTracker stealthTracker;

    //Level tracking
    List<float> times;
    List<float> stealthScores;

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

        //Add scores to list
        times.Add(timer.time);
        stealthScores.Add(stealthTracker.score);

        //Destroy old stealth objects
        Destroy(timer.gameObject);
        Destroy(stealthTracker.gameObject);
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
    }
    private void Awake()
    {
        //Ensure only one instance can exist at any one time
        if (instance)
        {
            Destroy(gameObject);
        }

        times = new List<float>();
        stealthScores = new List<float>();
    }

    private void OnDestroy()
    {
        if(currentlyScoring)
        {
            //Destroy old stealth objects
            Destroy(timer.gameObject);
            Destroy(stealthTracker.gameObject);
        }
    }
}
