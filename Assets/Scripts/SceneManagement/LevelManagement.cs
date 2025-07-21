using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagement : MonoBehaviour
{
    public enum Levels
    {
        MainMenu,
        Tutorial,
        Level1,
        Level2,
    }

    private Levels currentLevel;

    private void Awake()
    {
        
        currentLevel = (Levels)SceneManager.GetActiveScene().buildIndex;
    }
    private void GoToLevel(Levels level)
    {
        //Exit early if already in scene
        if(currentLevel == level)
        {
            return;
        }

        //Go to the level 
        SceneManager.LoadScene((int)level);
        currentLevel = level;
    }




}
