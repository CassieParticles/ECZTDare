using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrackBegin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            //Colliding with player
            MainScoreController.GetInstance().StartSection();
        }
    }
}
