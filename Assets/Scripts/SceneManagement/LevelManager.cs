using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

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
    private Levels prevLevel;

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
        prevLevel = currentLevel;
        Debug.Log("Current scene: " + currentLevel.ToString());

        callbackFunctions = new List<LevelChangeCallback>();

        SceneManager.sceneLoaded += SendObserver;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SendObserver;
    }

    private void SendObserver(Scene scene, LoadSceneMode mode)
    {
        Levels level = (Levels)scene.buildIndex;
        foreach (LevelChangeCallback callback in callbackFunctions)
        {
            callback(level, level==prevLevel);
        }
        currentLevel = level;
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
        prevLevel = currentLevel;
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
        prevLevel = currentLevel;
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
