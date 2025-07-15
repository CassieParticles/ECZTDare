using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : BaseEnemyBehaviour
{

    private void Awake()
    {
        Setup();
    }
    private void FixedUpdate()
    {
        BaseUpdate();
        enemySight.UpdateVisual();
        if(Player)
        {
            CalcSuspicionIncrease();
        }
    }
}
