using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScoreController : MonoBehaviour
{
    private static MainScoreController instance;
    [SerializeField] GameObject TimerObjectPrefab;

    private GameObject TimerObject;
    private void Awake()
    {
        //Ensure object always exists
        if(instance)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static MainScoreController GetInstance()
    {
        return instance;
    }

    public void StartTimer()
    {
        TimerObject = Instantiate(TimerObjectPrefab);
    }

    public void StopTimer()
    {
        float timeTaken = TimerObject.GetComponent<ScoreTimer>().time;
    }
}
