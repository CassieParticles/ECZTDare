using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    public AK.Wwise.Event deathWall;

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

    private UpdateChaseDisplayText chaseDisplayText;
    private bool wallVisible;

    GameObject Player;

    private void Awake()
    {
        currentSpeed = wallData.speedMedium;
        desiredSpeed = wallData.speedMedium;
        currentDistance = Distance.Medium;

        //Starts the audio
        deathWall.Post(gameObject);

        Player = FindAnyObjectByType<MovementScript>().gameObject;

        chaseDisplayText = FindAnyObjectByType<UpdateChaseDisplayText>(FindObjectsInactive.Include);
        chaseDisplayText.StartDisplay();
        wallVisible = true;
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

            //Sets the "Music" State Group's active State to "Alarm_High"
            AkSoundEngine.SetState("Music", "Alarm_High");
        }
        else if (distance >= wallData.closeDist && distance < wallData.mediumDist && currentDistance != Distance.Medium)
        {
            desiredSpeed = wallData.speedMedium;
            currentAcc = currentDistance == Distance.Close ? wallData.AccCloseMedium : wallData.AccMediumFar;
            currentDistance = Distance.Medium;
            Debug.Log("Medium speed");

            //Sets the "Music" State Group's active State to "Alarm_Middle"
            AkSoundEngine.SetState("Music", "Alarm_Middle");
        }
        else if (distance >= wallData.mediumDist && currentDistance != Distance.Far)
        {
            desiredSpeed = wallData.speedFar;
            currentAcc = wallData.AccMediumFar;
            currentDistance = Distance.Far;
            Debug.Log("Far speed");

            //Sets the "Music" State Group's active State to "Alarm_Low"
            AkSoundEngine.SetState("Music", "Alarm_Low");
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

    private void DisplayDistance()
    {
        //Display or hide the GUI if wall is on or off screen
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (wallVisible && viewportPos.x < 0)
        {
            //No longer on screen
            wallVisible = false;
            chaseDisplayText.StartDisplay();
        }
        if(!wallVisible && viewportPos.x > 0)
        {
            //Now visible on screen
            wallVisible = true;
            chaseDisplayText.StopDisplay();
        }

        //No point updating GUI if it's not visible
        if (wallVisible)
        { return; }



        //Update text on the UI
        float distanceFromEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x - transform.position.x;

        if (chaseDisplayText)
        {
            chaseDisplayText.UpdateDistance(distanceFromEdge);
        }
    }

    private void FixedUpdate()
    {
        //Handle updating the distance
        UpdateDistance();

        //Handle updating speed
        UpdateSpeed();

        //Handle updating the GUI
        DisplayDistance();

        //Move the object
        transform.position += new Vector3(currentSpeed * Time.fixedDeltaTime,0,0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<MovementScript>())
        {
            chaseDisplayText.StopDisplay();
            FindAnyObjectByType<MenuScript>().Lose();
        }
    }

    private void OnDestroy()
    {
        if(chaseDisplayText)
        {
            chaseDisplayText.StopDisplay();
        }
    }
}
