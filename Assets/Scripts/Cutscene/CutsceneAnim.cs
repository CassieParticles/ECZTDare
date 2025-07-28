using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Get video player and start playing video
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.Play();
        //================= REBECCA ANIMATION STARTS PLAYING HERE ========================//


        //================= REBECCA ANIMATION STARTS PLAYING HERE ========================//
        InputSystem.onAnyButtonPress.CallOnce(ExitScene);
        player.loopPointReached += EndReached;
    }

    private void ExitScene(InputControl control)
    {
        //================= REBECCA SCENE EXITS HERE ======================================//


        //================= REBECCA SCENE EXITS HERE ======================================//
        SceneManager.LoadScene(0);
    }

    private void EndReached(VideoPlayer vp)
    {
        //================= REBECCA ANIMATION STOPS PLAYING HERE ==========================//


        //================= REBECCA ANIMATION STOPS PLAYING HERE ==========================//
    }

}
