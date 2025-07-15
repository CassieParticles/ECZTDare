using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Find the death wall, and destroy it
        DeathWall wall = FindAnyObjectByType<DeathWall>();
        if (!wall){ return; }

        Destroy(wall.gameObject);
    }
}
