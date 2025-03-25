using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hackable : MonoBehaviour
{
    [NonSerialized] public bool beingHacked;
    [SerializeField] protected float hackingNoiseRadius = 10.0f;
    public virtual void OnHack() 
    {
        AudioDetectionSystem.getAudioSystem().PlaySound(transform.position, hackingNoiseRadius, 15, AudioSource.Hacked);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f,0.1f,0.2f);
        Gizmos.DrawWireSphere(transform.position, hackingNoiseRadius);
    }
}
