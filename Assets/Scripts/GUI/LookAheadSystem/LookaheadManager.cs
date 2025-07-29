using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class LookaheadManager : MonoBehaviour
{
    [SerializeField] private GameObject trackerGUIPrefab;

    //List is stored in sorted order for x pos (left list is descending, right list is ascending)
    List<LookaheadTracker> leftTrackers;
    List<LookaheadTracker> rightTrackers;

    List<LookaheadGUI> trackerGUIPool;
    readonly int trackerPoolSize = 10;

    LinkedList<LookaheadGUI> leftGUIToUpdate;
    LinkedList<LookaheadGUI> rightGUIToUpdate;

    private Vector3 leftScreenSide = Vector3.zero;
    private Vector3 rightScreenSide = Vector3.right;
    private Vector3 unitVector = new Vector3(1, 1, 1);

    private void Awake()
    {
        leftTrackers = new List<LookaheadTracker>();
        rightTrackers = new List<LookaheadTracker>();
        trackerGUIPool = new List<LookaheadGUI>(trackerPoolSize);
        leftGUIToUpdate = new LinkedList<LookaheadGUI>();
        rightGUIToUpdate = new LinkedList<LookaheadGUI>();
    }

    private void Start()
    {
        //Createnew GUI objects
        for (int i = 0; i < trackerPoolSize; i++)
        {
            GameObject newGUI = Instantiate(trackerGUIPrefab);
            newGUI.transform.SetParent(transform);
            newGUI.GetComponent<RectTransform>().position = Vector3.zero;
            newGUI.GetComponent<RectTransform>().localScale = unitVector;
            newGUI.SetActive(false);
            trackerGUIPool.Add(newGUI.GetComponent<LookaheadGUI>());
        }
    }

    private LookaheadGUI GetFreeTrackerGUI()
    {
        //Find first inactive tracker GUI
        for(int i=0;i< trackerPoolSize;++i)
        {
            if (!trackerGUIPool[i].gameObject.activeSelf)
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

    public void RemoveTracker(LookaheadTracker tracker)
    {
        if (tracker.TrackLeft)
        {
            leftTrackers.Remove(tracker);
        }
        if (tracker.TrackRight)
        {
            rightTrackers.Remove(tracker);
        }

        if(tracker.displayingGUI)
        {
            tracker.displayingGUI.gameObject.SetActive(false);
            tracker.displayingGUI = null;
        }
    }

    //Update display visiblity
    private void CheckTrackers()
    {
        //Update left and right positions for edge of screen
        leftScreenSide.z = -Camera.main.transform.position.z;
        rightScreenSide.z = -Camera.main.transform.position.z;

        //Get left and right x positions
        float leftSideXPos = Camera.main.ViewportToWorldPoint(leftScreenSide).x;
        float rightSideXPos = Camera.main.ViewportToWorldPoint(rightScreenSide).x;

        //Check left trackers if any need to be shown
        foreach (LookaheadTracker tracker in leftTrackers)
        {
            //Get distance between this point, and left edge of screen
            float distanceToEdge = leftSideXPos - tracker.transform.position.x;
            //Object should be tracked
            if(distanceToEdge > 0 && distanceToEdge < tracker.maxDistance)
            {
                //Tracker not displayed yet, set up a new one
                if(!tracker.displayingGUI)
                {
                    LookaheadGUI displayGUI = GetFreeTrackerGUI();
                    //Ensure free tracker was found
                    if(!displayGUI){ continue; }

                    displayGUI.LookaheadStart(tracker,true);
                    displayGUI.gameObject.SetActive(true);

                    //Set position to left side of screen

                    tracker.displayingGUI = displayGUI;
                }

                AddToLeftList(tracker.displayingGUI,distanceToEdge);
            }
            else
            {
                //Deactivate the displaying GUI
                if(tracker.displayingGUI)
                {
                    tracker.displayingGUI.gameObject.SetActive(false);
                    tracker.displayingGUI = null;
                }
            }
        }


        //Check right trackers if any need to be shown
        foreach (LookaheadTracker tracker in rightTrackers)
        {
            //Get distance between this point, and left edge of screen
            float distanceToEdge = tracker.transform.position.x - rightSideXPos;
            //Object should be tracked
            if (distanceToEdge > 0 && distanceToEdge < tracker.maxDistance)
            {
                //Tracker not displayed yet, set up a new one
                if (!tracker.displayingGUI)
                {
                    LookaheadGUI displayGUI = GetFreeTrackerGUI();
                    //Ensure free tracker was found
                    if (!displayGUI)
                    { continue; }

                    displayGUI.LookaheadStart(tracker, false);
                    displayGUI.gameObject.SetActive(true);

                    //Set position to left side of screen

                    tracker.displayingGUI = displayGUI;
                }

                AddToRightList(tracker.displayingGUI, distanceToEdge);
            }
            else
            {
                //Deactivate the displaying GUI
                if (tracker.displayingGUI && !tracker.displayingGUI.onLeftSide)
                {
                    tracker.displayingGUI.gameObject.SetActive(false);
                    tracker.displayingGUI = null;
                }
            }
        }
    }

    private void AddToLeftList(LookaheadGUI gui, float distance)
    {
        gui.setDistance(distance);
        foreach(LookaheadGUI guiIt in leftGUIToUpdate)
        {
            if(distance < guiIt.distance)
            {
                leftGUIToUpdate.AddBefore(leftGUIToUpdate.Find(guiIt), gui);
                return;
            }
        }
        //Not been added to list, needs to be added to end
        leftGUIToUpdate.AddLast(gui);
    }

    private void AddToRightList(LookaheadGUI gui, float distance)
    {
        gui.setDistance(distance);
        foreach (LookaheadGUI guiIt in rightGUIToUpdate)
        {
            if (distance < guiIt.distance)
            {
                rightGUIToUpdate.AddBefore(rightGUIToUpdate.Find(guiIt), gui);
                return;
            }
        }
        //Not been added to list, needs to be added to end
        rightGUIToUpdate.AddLast(gui);
    }

    private void UpdateDisplays()
    {
        int count = 0;
        foreach(LookaheadGUI gui in leftGUIToUpdate)
        {
            gui.UpdateInformation(count++);
        }
        count = 0;
        foreach (LookaheadGUI gui in rightGUIToUpdate)
        {
            gui.UpdateInformation(count++);
        }
        leftGUIToUpdate.Clear();
        rightGUIToUpdate.Clear();
    }


    private void FixedUpdate()
    {
        CheckTrackers();

        UpdateDisplays();
    }
}
