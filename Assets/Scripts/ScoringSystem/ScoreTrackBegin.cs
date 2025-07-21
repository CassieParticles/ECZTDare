using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrackBegin : MonoBehaviour
{
    [SerializeField] private bool TrackStealth = true;
    [SerializeField] private bool TrackSpeed = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            //Colliding with player
            MainScoreController.GetInstance().StartSection(TrackStealth,TrackSpeed);
        }
    }
}
