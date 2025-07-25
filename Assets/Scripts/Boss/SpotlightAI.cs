using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpotlightMovement),typeof(SignalReciever))]
public class SpotlightAI : MonoBehaviour,IRecieveSignals
{
    [SerializeField] private float suspicionScaleRate = 70.0f;
    [SerializeField] private float suspicionDecayRate = 40.0f;

    SpotlightMovement spotlight;
    BaseAnchor currentAnchor;
    BaseAnchor prevAnchor;

    MovementScript Player;

    private float suspicion;
    private bool playerDetected
    {
        get
        {
            BackwallDetectability backwall = FindAnyObjectByType<BackwallDetectability>();
            bool backwallCheck = !backwall || backwall.playerVisible;
            return Player && !Player.cloaking && backwallCheck;
        }
    }

    public bool IsDistractible()
    {
        return currentAnchor.IsDistractible();
    }

    private void Awake()
    {
        spotlight = GetComponent<SpotlightMovement>();
    }

    public BaseAnchor getCurrentAnchor()
    {
        return currentAnchor;
    }

    public void RecieveSignal(BaseAnchor anchor)
    {
        if(currentAnchor)
        {
            prevAnchor = currentAnchor;
            currentAnchor.RemoveSpotlight();
        }

        anchor.AddSpotlight(spotlight);
        currentAnchor = anchor;
    }

    public void BacktrackAnchor()
    {
        //Ensure there is a current anchor
        if (!currentAnchor){ return; }
        currentAnchor.RemoveSpotlight();
        //Go back to previous anchor (if it exists)
        if (prevAnchor)
        {
            prevAnchor.AddSpotlight(spotlight);
        }
        //Swap current and prev
        BaseAnchor tempAnchor = prevAnchor;
        prevAnchor = currentAnchor;
        currentAnchor = tempAnchor;
    }

    private void FixedUpdate()
    {
        //Adjust suspicion bsaed on if player is visible
        if (playerDetected)
        {
            AddSuspicion(suspicionScaleRate * Time.fixedDeltaTime);
            GetComponent<SpriteRenderer>().color= Color.red;
        }
        else
        {
            AddSuspicion(-suspicionDecayRate * Time.fixedDeltaTime);
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        //If suspicion is full
        if(suspicion>=100)
        {
            FindAnyObjectByType<MenuScript>().Lose();
        }
    }

    private void AddSuspicion(float n)
    {
        suspicion+= n;
        suspicion = Mathf.Clamp(suspicion, 0, 100);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //get if player
        if (collision.gameObject.GetComponent<MovementScript>())
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
