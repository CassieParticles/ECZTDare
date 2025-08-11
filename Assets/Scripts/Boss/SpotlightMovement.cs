using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpotlightMovement : MonoBehaviour
{
    [SerializeField] private float speed=5;
    public float travelTime
    {
        get
        {
            return distance / speed;
        }
    }

    private Vector2 desiredLocation;
    private Vector2 velocity;
    private float distance;

    Coroutine travelTimeCheck;

    private IEnumerator TravelTimeCheck()
    {
        yield return new WaitForSeconds(travelTime);
        travelTimeCheck = null; 
    }

    public void MoveTo(Vector2 location)
    {
        if (travelTimeCheck != null)
        {
            StopCoroutine(travelTimeCheck);
        }
        desiredLocation = location;
        Vector2 displacement = desiredLocation - (Vector2)transform.position;
        velocity = displacement.normalized * speed;
        distance = displacement.magnitude;

        //Start new coroutine
        travelTimeCheck = StartCoroutine(TravelTimeCheck());
    }

    public void TeleportTo(Vector2 location)
    {
        if(travelTimeCheck != null)
        {
            StopCoroutine(travelTimeCheck);
        }
        desiredLocation = location;
        transform.position = location;
    }

    private void Awake()
    {
        desiredLocation = transform.position;
    }

    private void FixedUpdate()
    {
        //If spotlight needs to move to new location
        if ((Vector2)transform.position != desiredLocation)
        {
            if(travelTimeCheck==null)
            {
                transform.position = (Vector3)desiredLocation;
            }
            else
            { 
                transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
            }
        }
    }
}
