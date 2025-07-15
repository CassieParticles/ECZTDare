using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    private enum Distance
    {
        Close,
        Medium,
        Far
    }

    [SerializeField] private float closeDist=20;
    [SerializeField] private float mediumDist=30;
    //Far distance is above 30

    [SerializeField] private float speedClose=15;
    [SerializeField] private float speedMedium=50;
    [SerializeField] private float speedFar=90;

    [SerializeField] private float AccCloseMedium=5;
    [SerializeField] private float AccMediumFar=8;

    private Distance currentDistance;

    private float desiredSpeed;
    private float currentSpeed;
    private float currentAcc;

    GameObject Player;

    private void Awake()
    {
        currentSpeed = speedMedium;
        desiredSpeed = speedMedium;
        currentDistance = Distance.Medium;

        Player = FindAnyObjectByType<MovementScript>().gameObject;
    }

    private void UpdateDistance()
    {
        float distance = Player.transform.position.x - transform.position.x;
        if (distance < closeDist && currentDistance != Distance.Close)
        {
            desiredSpeed = speedClose;
            currentAcc = AccCloseMedium;
            currentDistance = Distance.Close;
            Debug.Log("Close speed");
        }
        else if (distance >= closeDist && distance < mediumDist && currentDistance != Distance.Medium)
        {
            desiredSpeed = speedMedium;
            currentAcc = currentDistance == Distance.Close ? AccCloseMedium : AccMediumFar;
            currentDistance = Distance.Medium;
            Debug.Log("Medium speed");
        }
        else if (distance >= mediumDist && currentDistance != Distance.Far)
        {
            desiredSpeed = speedFar;
            currentAcc = AccMediumFar;
            currentDistance = Distance.Far;
            Debug.Log("Far speed");
        }
    }
    private void UpdateSpeed()
    {
        if (currentSpeed == desiredSpeed)
        {
            return;
        }

        float delta = desiredSpeed - currentSpeed;
        if (delta < currentAcc * Time.fixedDeltaTime)
        {
            currentSpeed = desiredSpeed;
            return;
        }

        currentSpeed += currentAcc * Mathf.Sign(delta) * Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        //Handle updating the distance
        UpdateDistance();

        //Handle updating speed
        UpdateSpeed();

        //Move the object
        transform.position += new Vector3(currentSpeed * Time.fixedDeltaTime,0,0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<MovementScript>())
        {
            FindAnyObjectByType<MenuScript>().Lose();
        }
    }
}
