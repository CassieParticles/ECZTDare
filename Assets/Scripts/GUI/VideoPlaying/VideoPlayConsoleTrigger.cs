using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayConsoleTrigger : MonoBehaviour
{
    [SerializeField] VideoClip clip;
    private void ConsoleTriggerVideo()
    {
        //Get video play UI
        VideoPlayUI videoPlay = FindAnyObjectByType<VideoPlayUI>(FindObjectsInactive.Include);
        videoPlay.OpenVideo(clip);
    }

    private void Start()
    {
        GetComponent<ConsoleHackable>().AddConsoleListener(ConsoleTriggerVideo);
    }
}
