using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAssistPromptOpenCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Find component
        NavigationAssistPrompt promptScript = FindAnyObjectByType<NavigationAssistPrompt>(FindObjectsInactive.Include);
        if (promptScript)
        {
            promptScript.OpenPrompt();
        }
    }
}
