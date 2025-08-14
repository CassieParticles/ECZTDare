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
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EnableNavAssist()
    {
        navigationAssistEnabled = true;
        NavigationAssistSprites sprites = FindAnyObjectByType<NavigationAssistSprites>(FindObjectsInactive.Include);
        if (sprites)
        {
            sprites.gameObject.SetActive(true);
        }
    }

    public void DisableNavAssist()
    {
        navigationAssistEnabled = false;
        NavigationAssistSprites sprites = FindAnyObjectByType<NavigationAssistSprites>(FindObjectsInactive.Include);
        if (sprites)
        {
            sprites.gameObject.SetActive(false);
        }
    }

    public void SetNavAssist(bool navAssist)
    {
        navigationAssistEnabled = navAssist;
        NavigationAssistSprites sprites = FindAnyObjectByType<NavigationAssistSprites>(FindObjectsInactive.Include);
        if (sprites)
        {
            sprites.gameObject.SetActive(navAssist);
        }
    }
}
