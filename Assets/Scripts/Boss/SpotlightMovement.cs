using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpotlightMovement : MonoBehaviour
{
    [SerializeField]private float travelTime = 2.0f;

    private Vector2 desiredLocation;

    public void MoveTo(Vector2 location)
    {
        desiredLocation = location;
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

            transform.position += ((Vector3)toMove * Time.fixedDeltaTime) / travelTime;
        }
    }
}
