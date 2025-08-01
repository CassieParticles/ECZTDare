using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayUI : MonoBehaviour
{
    VideoPlayer videoPlayer;

    public void OpenVideo(VideoClip clipToPlay)
    {
        gameObject.SetActive(true);
        if (!clipToPlay){ return; }
        if (!videoPlayer){ videoPlayer = GetComponent<VideoPlayer>(); }
        videoPlayer.enabled = true;

        

        foreach (Light2D light in FindObjectsByType<Light2D>(FindObjectsSortMode.None))
        {
            light.enabled = false;
        }

        //Play video
        videoPlayer.clip = clipToPlay;
        if (videoPlayer.enabled)
        {
            videoPlayer.Play();
        }
        else
        {
            Debug.Log("Object is not active???");
        }



        //Pause game stuff
        //Set timescale to 0
        Time.timeScale = 0;
        //Set menu script "canBePaused" to false
        MenuScript menuScript = FindAnyObjectByType<MenuScript>();
        if (menuScript)
        {
            menuScript.canPause = false;
        }

        
    }

    public void CloseVideo()
    {
        if(!videoPlayer){ videoPlayer = GetComponent<VideoPlayer>(); }

        //Stop video
        videoPlayer.Pause();
        videoPlayer.time = 0;

        foreach (Light2D light in FindObjectsByType<Light2D>(FindObjectsSortMode.None))
        {
            light.enabled = true;
        }

        //Resume game stuff
        //Set timescale to 1
        Time.timeScale = 1;
        //Set menu script "canBePaused" to true
        MenuScript menuScript = FindAnyObjectByType<MenuScript>();
        if (menuScript)
        {
            menuScript.canPause = true;
        }
        gameObject.SetActive(false);
    }
}
