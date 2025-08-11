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

    // Start is called before the first frame update
    void Start()
    {
        //Get video player and start playing video
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.Play();
        //================= REBECCA ANIMATION STARTS PLAYING HERE ========================//
        cutsceneSound.Post(gameObject);
        cutsceneMusic.Post(gameObject);
        cutsceneAmbience.Post(gameObject);

        InputSystem.onAnyButtonPress.CallOnce(ExitScene);
        player.loopPointReached += EndReached;
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
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }

    }

    private void EndReached(VideoPlayer vp)
    {
        //================= REBECCA ANIMATION STOPS PLAYING HERE ==========================//


        //================= REBECCA ANIMATION STOPS PLAYING HERE ==========================//
    }

}
