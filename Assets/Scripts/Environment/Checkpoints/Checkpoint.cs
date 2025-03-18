using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    CheckpointManager checkpointManager;
    int index;

    public void SetIndex(int index)
    {
        this.index = index;
    }

    private void Awake()
    {
        checkpointManager = transform.parent.GetComponent<CheckpointManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<MovementScript>())
        {
            checkpointManager.CheckpointReach(index);
        }
    }
}
