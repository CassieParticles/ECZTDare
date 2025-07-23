using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : BaseEnemyBehaviour
{
    public AK.Wwise.Event turretFire;

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
        else
        {
            CalcSuspicionDecay();
        }

        if(Player && suspicion > 100)
        {
            //Play firing sound
            turretFire.Post(gameObject);

            //Kill player
            FindAnyObjectByType<MenuScript>().Lose();
        }
    }
}
