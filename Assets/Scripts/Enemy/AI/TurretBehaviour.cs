using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : BaseEnemyBehaviour
{
    private void FixedUpdate()
    {
        if(Player)
        {
            Debug.Log("The player is here");
        }
    }
}
