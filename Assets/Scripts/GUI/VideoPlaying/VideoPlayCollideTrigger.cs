using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayCollideTrigger : MonoBehaviour
{
    [SerializeField] VideoClip clip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementScript>())
        {
            //Get video play UI
            VideoPlayUI videoPlay = FindAnyObjectByType<VideoPlayUI>(FindObjectsInactive.Include);
            videoPlay.OpenVideo(clip);
        }

    }
}
