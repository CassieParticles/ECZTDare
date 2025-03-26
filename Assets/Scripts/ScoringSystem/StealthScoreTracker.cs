using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthScoreTracker : MonoBehaviour
{
    [SerializeField] int InitialScore = 10000;
    [SerializeField] int SeenByCameraCost = 100;
    [SerializeField] int SeenByGuardCost = 300;
    [SerializeField] int CaughtCost = 1000;

    public int MaxScore { get; private set; }
    public enum Sources
    {
        SeenByCamera,
        SeenByGuard,
        Caught
    }

    public int score { get; private set; }
    private void Awake()
    {
        score = InitialScore;
        MaxScore = InitialScore;
        DontDestroyOnLoad(gameObject);
    }

    public static StealthScoreTracker GetTracker()
    {
        return FindFirstObjectByType<StealthScoreTracker>();
    }

    public void DeductPoints(Sources source)
    {
        switch (source)
        {
            case Sources.SeenByCamera:
                score -= SeenByCameraCost;
                break;
            case Sources.SeenByGuard:
                score -= SeenByGuardCost;
                break;
            case Sources.Caught:
                score-= CaughtCost;
                break;
        }
    }
}
