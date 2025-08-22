using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneAnim : MonoBehaviour
{
    [SerializeField] string nextSceneName;

    public AK.Wwise.Event cutsceneSound;
    public AK.Wwise.Event cutsceneMusic;
    public AK.Wwise.Event cutsceneAmbience;
    public AK.Wwise.Event buttonClick;

    [Range(0f, 10f)] public float audioDelay = 0f;

    private IDisposable disposableCallback;

    // Start is called before the first frame update
    void Start()
    {
        //Get video player and start playing video
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.Play();
        //================= REBECCA ANIMATION STARTS PLAYING HERE ========================//
        
        cutsceneMusic.Post(gameObject);
        cutsceneAmbience.Post(gameObject);
        StartCoroutine(PlaySoundAfterDelay());

        disposableCallback = InputSystem.onAnyButtonPress.Call(ExitScene);
        player.loopPointReached += EndReached;

        MenuScript menu = FindAnyObjectByType<MenuScript>();
        if (menu)
        {
            menu.canPause = false;
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Callback disposed of");
        disposableCallback.Dispose();
    }

    IEnumerator PlaySoundAfterDelay()
    {
        yield return new WaitForSecondsRealtime(audioDelay);
        cutsceneSound.Post(gameObject);
    }

    private void ExitScene(InputControl control)
    {
        //================= REBECCA SCENE EXITS HERE ======================================//
        cutsceneSound.Stop(gameObject);
        cutsceneMusic.Stop(gameObject);
        cutsceneAmbience.Stop(gameObject);
        buttonClick.Post(gameObject);


        //================= REBECCA SCENE EXITS HERE ======================================//
        MenuScript menu = FindAnyObjectByType<MenuScript>();
        if (menu)
        {
            menu.ChangeScene(nextSceneName);
            Debug.Log("Change sceneA");
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.Log("Change sceneB");
        }

    }

    private void EndReached(VideoPlayer vp)
    {
        //================= REBECCA ANIMATION STOPS PLAYING HERE ==========================//


        //================= REBECCA ANIMATION STOPS PLAYING HERE ==========================//
    }

}
