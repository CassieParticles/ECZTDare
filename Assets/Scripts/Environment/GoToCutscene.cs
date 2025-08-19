using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToCutscene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If score system exists
        if (MainScoreController.GetInstance() && MainScoreController.GetInstance().currentlyScoring)
        {
            MainScoreController.GetInstance().EndSection(true);
            FindAnyObjectByType<MenuScript>().Win();
        }
        else //If score system doesn't exist
        {
            FindAnyObjectByType<MenuScript>().Win();
        }
    }
}
