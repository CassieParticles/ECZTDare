using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAssistHandler : MonoBehaviour
{
    public bool navigationAssistEnabled { get; private set; }

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
    }
}
