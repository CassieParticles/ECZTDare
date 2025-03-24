using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthScoreTracker : MonoBehaviour
{
    [SerializeField] float InitialScore = 10000;
    [SerializeField] float SeenByCameraCost = 100;
    [SerializeField] float SeenByGuardCost = 300;
    [SerializeField] float CaughtCost = 1000;
    public enum Sources
    {
        SeenByCamera,
        SeenByGuard,
        Caught
    }

    public float score { get; private set; }
    private void Awake()
    {
        score = InitialScore;
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
