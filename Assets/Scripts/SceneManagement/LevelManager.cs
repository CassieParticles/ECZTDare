using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public enum Levels
    {
        MainMenu,
        Tutorial,
        Level1,
        Level2,
    }

    private static LevelManager instance;
    private Levels currentLevel;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        instance = null;
    }

    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        currentLevel = (Levels)SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Current scene: " + currentLevel.ToString());
    }

    //Go to the level pased into the function
    public void GoToLevel(Levels level)
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

    //Go to the next level in the game
    public void GoToNextLevel()
    {
        int nextLevel = ((int)currentLevel + 1) % 4;
        GoToLevel((Levels)nextLevel);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene((int)currentLevel);
    }

    public Levels getCurrentLevel()
    {
        return currentLevel;
    }
}
