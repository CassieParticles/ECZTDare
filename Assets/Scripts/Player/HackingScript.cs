using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

public class HackingScript: MonoBehaviour, IKeyboardWASDActions {

    PlayerControls controls;
    InputAction hackAction;
    MovementScript movementScript;

    public Hackable target;

    [SerializeField] float range = 10f;
    [SerializeField] float behindRange = 2f;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<MovementScript>();

        if (controls == null) {
            controls = new PlayerControls();
            controls.KeyboardWASD.SetCallbacks(this);
        }
        controls.KeyboardWASD.Enable();
        hackAction = controls.FindAction("Hacking");
    }

    // Update is called once per frame
    void Update()
    {
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

    public void OnBoostCloak(InputAction.CallbackContext context) {

    }
    public void OnHacking(InputAction.CallbackContext context) {

    }
    public void OnJumping(InputAction.CallbackContext context) {

    }
    public void OnRunning(InputAction.CallbackContext context) {

    }
    public void OnSliding(InputAction.CallbackContext context) {

    }
}
