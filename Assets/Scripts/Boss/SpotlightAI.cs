using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpotlightMovement),typeof(SignalReciever))]
public class SpotlightAI : MonoBehaviour,IRecieveSignals
{
    SpotlightMovement spotlight;
    BaseAnchor currentAnchor;

    MovementScript Player;
    private bool playerDetected
    {
        get
        {
            return Player && Player.cloaking;
        }
    }

    private void Awake()
    {
        spotlight = GetComponent<SpotlightMovement>();
    }

    public void RecieveSignal(BaseAnchor anchor)
    {
        if(currentAnchor)
        {
            currentAnchor.RemoveSpotlight();
        }

        anchor.AddSpotlight(spotlight);
        currentAnchor = anchor;
    }

    private void FixedUpdate()
    {
        if (playerDetected)
        {
            Debug.Log("Player visible");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //get if player
        if(collision.gameObject.GetComponent<MovementScript>())
        {
            Player=collision.gameObject.GetComponent<MovementScript>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //get if player
        if (collision.gameObject.GetComponent<MovementScript>())
        {
            Player = null;
        }
    }
}
