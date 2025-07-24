using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayUI : MonoBehaviour
{
    VideoPlayer videoPlayer;

    private void Awake()
    {
        GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        videoPlayer.loopPointReached += VideoEnd;
    }

    private void VideoEnd(VideoPlayer vp)
    {
        CloseVideo();
    }
    public void OpenVideo(VideoClip clipToPlay)
    {
        if (!clipToPlay){ return; }
        if (!videoPlayer){ return; }

        //Play video
        videoPlayer.clip = clipToPlay;
        videoPlayer.Play();

        //Pause game stuff
        //Set timescale to 0
        Time.timeScale = 0;
        //Set menu script "canBePaused" to false
        MenuScript menuScript = FindAnyObjectByType<MenuScript>();
        if (menuScript)
        {
            menuScript.canPause = false;
        }

        gameObject.SetActive(true);
    }

    public void CloseVideo()
    {
        gameObject.SetActive(false);

        if(!videoPlayer){ return; }

        //Stop video
        videoPlayer.Pause();
        videoPlayer.time = 0;

        //Resume game stuff
        //Set timescale to 1
        Time.timeScale = 1;
        //Set menu script "canBePaused" to true
        MenuScript menuScript = FindAnyObjectByType<MenuScript>();
        if (menuScript)
        {
            menuScript.canPause = true;
        }
    }
}
