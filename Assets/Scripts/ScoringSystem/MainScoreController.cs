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

    public static MainScoreController GetInstance()
    {
        return instance;
    }
    public void StartSection(bool trackStealth, bool trackSpeed)
    {
        if(!trackStealth && !trackSpeed){ return; }
        //Don't start tracking twice
        if (currentlyScoring){ return; }
        currentlyScoring = true;

        if(trackSpeed)
        {
            timer = Instantiate(TimerObjectPrefab).GetComponent<ScoreTimer>();
        }
        if(trackStealth)
        { 
            stealthTracker = Instantiate(StealthObjectPrefab).GetComponent<StealthScoreTracker>();
        }
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

        float time = -1;
        int stealth = -1;

        //Collect scores from score trackers
        if(timer)
        {
            time = timer.time;
        }
        if (stealthTracker)
        {
            stealth = stealthTracker.score;
        }

        //Saves the score for this section in the score manager
        ScoreData sectionScore = new ScoreData {
            chaseSection = timer ? true : false,
            stealthSection = stealthTracker ? true : false,
            chaseTimeSeconds = time,
            stealthScore = stealth
        };

        if (GameObject.Find("ScoreManager") != null) {
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().SaveSectionScore(sectionScore, endOfLevel);
        }

        //Destroy old stealth objects
        if (timer)
        {
            Destroy(timer.gameObject);
        }
        if(stealthTracker)
        {
            Destroy(stealthTracker.gameObject);
        }

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
    }

    private void OnDestroy()
    {
        //Destroy tracking objects
        if(timer)
        {
            Destroy(timer.gameObject);
        }
        if(stealthTracker)
        {
            Destroy(stealthTracker.gameObject);
        }
    }
}
