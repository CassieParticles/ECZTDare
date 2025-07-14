using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpotlightMovement : MonoBehaviour
{
    [SerializeField]private float travelTime = 2.0f;

    private Vector2 desiredLocation;
    private Vector2 velocity;

    public void MoveTo(Vector2 location)
    {
        desiredLocation = location;
        Vector2 distance = desiredLocation - (Vector2)transform.position;
        velocity = distance / travelTime;
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
            //Get distance and direction
            Vector2 toMove = desiredLocation - (Vector2)transform.position;

            if(toMove.magnitude < Time.fixedDeltaTime)
            {
                transform.position = (Vector3)desiredLocation;
            }

            transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
        }
    }
}
