using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ConveyorHackable : Hackable
{
    public AK.Wwise.Event ConveyorBelt;

    private float defaultSpeed;
    [SerializeField] float hackedSpeed = -4f;
    [Range(0.1f, 25f)] public float animationSpeed;
    private bool toggled;

    private Animator animator;
    ConveyorBeltScript conveyorScript;

    void Awake() {
        ConveyorBelt.Post(gameObject);
        toggled = false;
        animator = GetComponent<Animator>();
        conveyorScript = GetComponent<ConveyorBeltScript>();
    }

    private void Start()
    {
        defaultSpeed = conveyorScript.currentSpeed;
    }

    public override void OnHack() 
    {
        base.OnHack();
        toggled = !toggled;
        conveyorScript.currentSpeed = toggled ? hackedSpeed : defaultSpeed;
        animator.SetFloat("Speed", conveyorScript.currentSpeed / animationSpeed);
        Cooldown();
    }
    public IEnumerator Cooldown() {
        beingHacked = true;
        yield return new WaitForSeconds(1);
        beingHacked = false;
    }
}
