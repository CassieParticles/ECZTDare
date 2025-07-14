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
        if(Player)
        {
            enemySight.UpdateVisual();
            CalcSuspicionIncrease();
        }
    }
}
