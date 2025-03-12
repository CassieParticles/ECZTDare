using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hackable : MonoBehaviour
{
    [NonSerialized] public bool beingHacked;
    [SerializeField] protected float hackingNoiseRadius = 10.0f;
    public abstract void OnHack();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hackingNoiseRadius);
    }
}
