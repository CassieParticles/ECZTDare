using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerBotBehaviour : MonoBehaviour
{
    //How much torque do they have, and what speed can they reach with it
    [SerializeField] private float forceToMove=3;
    [SerializeField] private float maxSpeed=80;

    [SerializeField] private PatrolRoute patrolRoute;

    private Vector2 currentDestination;
    private float errorDelta = 0.08f;

    private Rigidbody2D cleanerRigidbody;

    private void Start()
    {
        //Add cleaner bot
        patrolRoute.AddGuard(gameObject);
        currentDestination = patrolRoute.GetCurrNode(gameObject).position;

        cleanerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 vectorToTravel = currentDestination - (Vector2)transform.position;
        if(vectorToTravel.sqrMagnitude < errorDelta)
        {
            //Go to next node
            currentDestination = patrolRoute.GetNextNode(gameObject).position;
        }
        //Get direction force needs to apply
        bool travelRight = vectorToTravel.x > 0;
        //If abs velocity is less then max speed, or velocity direction different to direction of wanted travel
        bool shouldAccelerate = (Mathf.Abs(cleanerRigidbody.velocityX) < maxSpeed) || (travelRight != (cleanerRigidbody.velocityX > 0));

        Vector2 forceDirection = Vector2.right * (travelRight ? 1 : -1) * (shouldAccelerate ? 1 : 0);

        //Apply force
        cleanerRigidbody.AddForce(forceDirection * forceToMove);
    }
}
