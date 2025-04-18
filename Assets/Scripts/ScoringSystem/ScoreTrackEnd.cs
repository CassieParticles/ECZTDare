using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrackEnd : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            //Colliding with player
            MainScoreController.GetInstance().EndSection(false,false);
        }
    }
}
