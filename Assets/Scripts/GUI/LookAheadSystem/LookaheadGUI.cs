using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LookaheadGUI : MonoBehaviour
{
    //Positions had to be heavily shrunk cause when added they get multiplied by ~28 and go super far out
    static readonly Vector3 leftPosition=new Vector3(-800,0,0);
    static readonly Vector3 rightPosition=new Vector3(800,0,0);
    //Called when lookahead GUI is told to appear

    private LookaheadTracker tracker;

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void LookaheadStart(LookaheadTracker tracker, bool onLeftSide)
    {
        //Update sprite
        GetComponentInChildren<Image>().sprite = tracker.displaySprite;
        //Move to correct side
        if(onLeftSide)
        {
            GetComponent<RectTransform>().localPosition = leftPosition;
        }
        else
        {
            GetComponent<RectTransform>().localPosition = rightPosition;
        }
        //Set tracker
        this.tracker = tracker;
    }

    public void UpdateInformation(float distanceFromScreen)
    {
        text.text = Mathf.Floor(distanceFromScreen).ToString() + "m";
    }
}
