using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookaheadManager : MonoBehaviour
{
    [SerializeField] private GameObject trackerGUIPrefab;

    //List is stored in sorted order for x pos (left list is descending, right list is ascending)
    List<LookaheadTracker> leftTrackers;
    List<LookaheadTracker> rightTrackers;

    List<GameObject> trackerGUIPool;
    readonly int trackerPoolSize = 10;

    private void Awake()
    {
        leftTrackers = new List<LookaheadTracker>();
        rightTrackers = new List<LookaheadTracker>();
        trackerGUIPool = new List<GameObject>();
    }

    private void Start()
    {
        //Createnew GUI objects
        for (int i = 0; i < trackerPoolSize; i++)
        {
            GameObject newGUI = Instantiate(trackerGUIPrefab);
            newGUI.transform.SetParent(transform);
            newGUI.GetComponent<RectTransform>().position = Vector3.zero;
            newGUI.SetActive(false);
            trackerGUIPool.Add(newGUI);
        }
    }

    private GameObject GetFreeTrackerGUI()
    {
        //Find first inactive tracker GUI
        for(int i=0;i< trackerPoolSize;++i)
        {
            if (!trackerGUIPool[i].activeSelf)
            {
                return trackerGUIPool[i];
            }
        }
        Debug.LogError("ERROR: INSUFFICIENT POOL SIZE, INCREASE POOL SIZE IN LOOKAHEADMANAGER");
        return null;
    }

    public void AddTracker(LookaheadTracker tracker)
    {
        if(tracker.TrackLeft)
        {
            leftTrackers.Add(tracker);
        }
        if (tracker.TrackRight)
        {
            rightTrackers.Add(tracker);
        }
    }

    private void FixedUpdate()
    {
        Debug.Log("Test line");
    }
}
