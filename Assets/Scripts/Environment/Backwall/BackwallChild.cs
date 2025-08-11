using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwallChild : MonoBehaviour
{
    BackwallDetectability parentBackwall;

    private void Awake()
    {
        parentBackwall = GetComponentInParent<BackwallDetectability>();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementScript>())
        {
            parentBackwall.collidersFiring++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementScript>())
        {
            parentBackwall.collidersFiring--;
        }
    }
}
