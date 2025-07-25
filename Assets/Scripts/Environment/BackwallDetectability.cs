using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwallDetectability : MonoBehaviour
{
    public bool playerVisible { get; private set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<MovementScript>())
        {
            playerVisible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementScript>())
        {
            playerVisible = false;
        }
    }
}
