using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookaheadTracker : MonoBehaviour
{
    public float maxDistance;
    public Sprite displaySprite;
    public bool TrackLeft;
    public bool TrackRight;

    private void Start()
    {
        //Look for manager, and add this if one exists
        LookaheadManager manager = FindAnyObjectByType<LookaheadManager>();
        if(manager)
        {
            manager.AddTracker(this);
        }
    }
}
