using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayUI : MonoBehaviour
{
    VideoPlayer videoPlayer;

    TextMeshProUGUI additionalText;

    public void OpenVideo(VideoClip clipToPlay, TextMeshProUGUI additionalText = null)
    {
        gameObject.SetActive(true);
        if (!clipToPlay){ return; }
        if (!videoPlayer){ videoPlayer = GetComponent<VideoPlayer>(); }
        videoPlayer.enabled = true;

        

        foreach (Light2D light in FindObjectsByType<Light2D>(FindObjectsSortMode.None))
        {
            light.enabled = false;
        }

        //Enable additional text if there is any
        if(additionalText)
        {
            additionalText.gameObject.SetActive(true);
            this.additionalText=additionalText;
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

        //Disable additional text if there was one
        if(additionalText)
        {
            if (additionalText)
            {
                additionalText.gameObject.SetActive(false);
                additionalText = null;
            }
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
