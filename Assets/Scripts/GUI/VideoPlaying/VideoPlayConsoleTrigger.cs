using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayConsoleTrigger : MonoBehaviour
{
    [SerializeField] VideoClip clip;
    [SerializeField] TextMeshProUGUI additionalText;
    private void ConsoleTriggerVideo()
    {
        //Get video play UI
        VideoPlayUI videoPlay = FindAnyObjectByType<VideoPlayUI>(FindObjectsInactive.Include);
        videoPlay.OpenVideo(clip,additionalText);
    }

    private void Start()
    {
        GetComponent<ConsoleHackable>().AddConsoleListener(ConsoleTriggerVideo);
    }
}
