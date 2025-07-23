using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAssistHandler : MonoBehaviour
{
    public bool navigationAssistEnabled { get; private set; }

    private static NavigationAssistHandler instance;
    private void Awake()
    {
        //Ensure this is singleton
        if(instance)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EnableNavAssist()
    {
        navigationAssistEnabled = true;
    }

    public void DisableNavAssist()
    {
        navigationAssistEnabled = false;
    }

    public void SetNavAssist(bool navAssist)
    {
        navigationAssistEnabled = navAssist;
        Debug.Log(navAssist.ToString());
    }
}
