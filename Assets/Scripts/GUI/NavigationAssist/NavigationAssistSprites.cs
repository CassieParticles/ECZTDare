using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAssistSprites : MonoBehaviour
{
    private void Start()
    {
        NavigationAssistHandler navAssistHandler = FindAnyObjectByType<NavigationAssistHandler>();
        if(navAssistHandler && navAssistHandler.navigationAssistEnabled)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
