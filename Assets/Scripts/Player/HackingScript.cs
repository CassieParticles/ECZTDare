using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

public class HackingScript: MonoBehaviour, IKeyboardWASDActions {

    PlayerControls controls;
    MovementScript movementScript;

    public Hackable target;

    [SerializeField] float range = 10f;
    [SerializeField] float behindRange = 2f;
    [SerializeField] float hackChargeRate = 50f;
    [SerializeField] int hackCharges = 3;

    public float hackCharge = 100;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<MovementScript>();

        if (controls == null) {
            controls = new PlayerControls();
            controls.KeyboardWASD.SetCallbacks(this);
        }
        controls.KeyboardWASD.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (hackCharge + hackChargeRate * Time.deltaTime < 100f) {
            hackCharge += hackChargeRate * Time.deltaTime;
        }

        target = null;
        float distance = range;
        
        //Finds the closest hackable object
        foreach (Hackable hackable in FindObjectsByType<Hackable>(FindObjectsSortMode.None)) {
            //Makes a vector and gets its direction
            Vector3 PlayerToHackableVector = hackable.transform.position - transform.position;
            bool direction = Convert.ToBoolean((Mathf.Sign(PlayerToHackableVector.x) + 1) / 2);
            //If within range and in the direction the player is facing
            if (PlayerToHackableVector.magnitude < distance && (Mathf.Abs(PlayerToHackableVector.x) <= behindRange || movementScript.facingRight == direction)) {
                //Debug.Log("Found hackable in range");
                target = hackable;
                distance = PlayerToHackableVector.magnitude;
            }
        }
    }

    public void OnHacking(InputAction.CallbackContext context) {
        if (target != null) {
            if (hackCharge >= 100f / hackCharges) {
                //If the target is currently being hacked dont let it
                target.OnHack();
                hackCharge -= 100f / hackCharges;
            } else {
                //Not enough charge
            }
        } //else No target

    }
    public void OnBoostCloak(InputAction.CallbackContext context) {

    }
    public void OnJumping(InputAction.CallbackContext context) {

    }
    public void OnRunning(InputAction.CallbackContext context) {

    }
    public void OnSliding(InputAction.CallbackContext context) {

    }
}
