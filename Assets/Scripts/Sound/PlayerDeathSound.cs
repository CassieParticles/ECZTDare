using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathSound : MonoBehaviour
{
    public AK.Wwise.Event playerDeath;

    private void OnEnable()
    {
        playerDeath.Post(gameObject);
    }
}
