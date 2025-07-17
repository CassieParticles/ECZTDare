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

    public void MoveTo(Vector2 location)
    {
        desiredLocation = location;
        Vector2 displacement = desiredLocation - (Vector2)transform.position;
        velocity = displacement.normalized * speed;
        distance = displacement.magnitude;
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

            if(toMove.sqrMagnitude < (speed * speed) * (Time.fixedDeltaTime * Time.fixedDeltaTime))
            {
                transform.position = (Vector3)desiredLocation;
            }

            transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
        }
    }
}
