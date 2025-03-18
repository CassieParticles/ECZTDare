using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ConveyorHackable : Hackable
{
    [SerializeField] float defaultSpeed = 4f;
    [SerializeField] float hackedSpeed = -4f;
    public float currentSpeed;
    private bool toggled;

    void Awake() {
        currentSpeed = defaultSpeed;
        toggled = false;
    }

    public override void OnHack() {
        toggled = !toggled;
        currentSpeed = toggled ? hackedSpeed : defaultSpeed;
        Cooldown();
    }
    public IEnumerator Cooldown() {
        beingHacked = true;
        yield return new WaitForSeconds(1);
        beingHacked = false;
    }
}
