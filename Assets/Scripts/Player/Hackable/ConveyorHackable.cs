using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ConveyorHackable : Hackable
{
    public AK.Wwise.Event ConveyorBelt;

    [SerializeField] float defaultSpeed = 4f;
    [SerializeField] float hackedSpeed = -4f;
    public float currentSpeed;
    [Range(0.1f, 25f)] public float animationSpeed;
    private bool toggled;

    private Animator animator;

    void Awake() {
        ConveyorBelt.Post(gameObject);
        currentSpeed = defaultSpeed;
        toggled = false;
        animator = GetComponent<Animator>();
    }

    public override void OnHack() 
    {
        base.OnHack();
        toggled = !toggled;
        currentSpeed = toggled ? hackedSpeed : defaultSpeed;
        animator.SetFloat("Speed", currentSpeed / animationSpeed);
        Cooldown();
    }
    public IEnumerator Cooldown() {
        beingHacked = true;
        yield return new WaitForSeconds(1);
        beingHacked = false;
    }
}
