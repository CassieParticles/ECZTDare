using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationAssistPrompt : MonoBehaviour
{
    public void ChangeValue(bool newValue)
    {
        NavigationAssistHandler handler = FindAnyObjectByType<NavigationAssistHandler>();
        if(handler)
        {
            handler.SetNavAssist(newValue);
        }
    }

    public void OpenPrompt()
    {
        //Set timescale to 0
        Time.timeScale = 0;
        //Set menu script "canBePaused" to false
        MenuScript menuScript = FindAnyObjectByType<MenuScript>();
        if(menuScript)
        {
            menuScript.canPause = false;
        }
        //Maybe some audio stuff
        //Open the prompt
        gameObject.SetActive(true);
        transform.GetComponentInChildren<Toggle>().Select();
    }

    public void ClosePrompt()
    {
        //Set timescale to 1
        Time.timeScale = 1;
        //Set menu script "canBePaused" to true
        MenuScript menuScript = FindAnyObjectByType<MenuScript>();
        if (menuScript)
        {
            menuScript.canPause = true;
        }
        //Maybe some audio stuff
        //Close the prompt
        gameObject.SetActive(false);
    }
}
