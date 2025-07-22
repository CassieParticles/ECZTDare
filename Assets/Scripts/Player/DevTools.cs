using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    private bool debugDown = false;
    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Comma))
        {
            Debug.Log("Moving player to checkpoint");
            FindAnyObjectByType<CheckpointManager>().GoToCurrentCheckpoint();
        }
        if(Input.GetKeyDown(KeyCode.Period))
        {
            Debug.Log("Moving player to previous checkpoint");
            FindAnyObjectByType<CheckpointManager>().GoToPrevCheckpoint();
        }
        if(Input.GetKeyDown(KeyCode.Slash))
        {
            Debug.Log("Moving player to next checkpoint");
            FindAnyObjectByType<CheckpointManager>().GoToNextCheckpoint();
        }
        if(Input.GetKeyDown(KeyCode.Hash))
        {
            Debug.Log("Reloading level");
        }
    }
}
