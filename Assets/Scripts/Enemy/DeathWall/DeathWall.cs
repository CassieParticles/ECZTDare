using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    public struct WallMoveData
    {
        public float closeDist;
        public float mediumDist;

        public float speedClose;
        public float speedMedium;
        public float speedFar;

        public float AccCloseMedium;
        public float AccMediumFar;

        public bool facingRight;

        public float yPosition;
    }

    private enum Distance
    {
        Close,
        Medium,
        Far
    }

    WallMoveData wallData;

    private Distance currentDistance;

    private float desiredSpeed;
    private float currentSpeed;
    private float currentAcc;

    GameObject Player;

    private void Awake()
    {
        currentSpeed = wallData.speedMedium;
        desiredSpeed = wallData.speedMedium;
        currentDistance = Distance.Medium;

        Player = FindAnyObjectByType<MovementScript>().gameObject;
    }

    public void SetData(WallMoveData data)
    {
        wallData = data;
        Vector3 position = transform.position;
        position.y = data.yPosition;
        transform.position = position;
    }

    private void UpdateDistance()
    {
        float distance = Player.transform.position.x - transform.position.x;
        if (distance < wallData.closeDist && currentDistance != Distance.Close)
        {
            desiredSpeed = wallData.speedClose;
            currentAcc = wallData.AccCloseMedium;
            currentDistance = Distance.Close;
            Debug.Log("Close speed");
        }
        else if (distance >= wallData.closeDist && distance < wallData.mediumDist && currentDistance != Distance.Medium)
        {
            desiredSpeed = wallData.speedMedium;
            currentAcc = currentDistance == Distance.Close ? wallData.AccCloseMedium : wallData.AccMediumFar;
            currentDistance = Distance.Medium;
            Debug.Log("Medium speed");
        }
        else if (distance >= wallData.mediumDist && currentDistance != Distance.Far)
        {
            desiredSpeed = wallData.speedFar;
            currentAcc = wallData.AccMediumFar;
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
