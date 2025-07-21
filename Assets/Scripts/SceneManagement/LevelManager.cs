using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelManager : MonoBehaviour
{
    public enum Levels
    {
        MainMenu,
        Tutorial,
        Level1,
        Level2,
    }

    public delegate void LevelChangeCallback(Levels newLevel, bool reload);

    private static LevelManager instance;
    private Levels currentLevel;

    private List<LevelChangeCallback> callbackFunctions;

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

        callbackFunctions = new List<LevelChangeCallback>();
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

        foreach(LevelChangeCallback callback in callbackFunctions)
        {
            callback(level, false);
        }
    }

    //Go to the next level in the game
    public void GoToNextLevel()
    {
        int nextLevel = ((int)currentLevel + 1) % 4;
        GoToLevel((Levels)nextLevel);
    }

    public void ReloadLevel()
    {
        foreach (var callback in callbackFunctions)
        {
            callback(currentLevel,true);
        }
        SceneManager.LoadScene((int)currentLevel);
    }

    public Levels getCurrentLevel()
    {
        return currentLevel;
    }

    public void AddCallback(LevelChangeCallback callback)
    {
        callbackFunctions.Add(callback);
    }

    public void RemoveCallback(LevelChangeCallback callback)
    {
        callbackFunctions.Remove(callback);
    }
}
