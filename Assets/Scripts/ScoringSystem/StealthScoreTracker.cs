using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthScoreTracker : MonoBehaviour
{
    public float score { get; private set; }
    private void Awake()
    {
        score = 120;
    }

    public static StealthScoreTracker GetTracker()
    {
        return FindFirstObjectByType<StealthScoreTracker>();
    }
    public void AddScore(float scoreAdd) { score += scoreAdd; }
    public void RemoveScore(float scoreRemove) { score -= scoreRemove; }

}
