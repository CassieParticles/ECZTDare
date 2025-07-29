using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookaheadTracker : MonoBehaviour
{
    public float maxDistance;
    public Sprite displaySprite;
    public bool TrackLeft;
    public bool TrackRight;

    //Used by manager to track what objects are and aren't displayed already
    [NonSerialized]public LookaheadGUI displayingGUI;

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
