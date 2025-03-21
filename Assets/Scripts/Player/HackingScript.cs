using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

public class HackingScript: MonoBehaviour, IGameplayControlsActions {

    public AK.Wwise.Event Hack_Fail;
    PlayerControls controls;
    InputAction hackAction;
    bool hackInput;

    MovementScript movementScript;
    Camera mainCamera;
    GameObject reticle;

    public Hackable target;

    //[SerializeField] float range = 10f;
    //[SerializeField] float behindRange = 2f;
    [SerializeField] float hackChargeRate = 50f;
    [SerializeField] int hackCharges = 3;

    public float hackCharge = 100;
    public bool hasHacked = false;

    MenuScript menu;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        reticle = GameObject.Find("HackingReticle");
        menu = GameObject.Find("Menu Canvas").GetComponent<MenuScript>();
        movementScript = GetComponent<MovementScript>();

        if (controls == null) {
            controls = new PlayerControls();
            controls.GameplayControls.SetCallbacks(this);
        }
        controls.GameplayControls.Enable();
        hackAction = controls.FindAction("Hacking");
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementScript.InputLocked) {
            if (hackCharge + hackChargeRate * Time.deltaTime < 100f) {
                hackCharge += hackChargeRate * Time.deltaTime;
            }

            hackInput = hackAction.ReadValue<float>() > 0;
            if (!hackInput) {
                hasHacked = false;
            }

            target = null;
            float distance = 1000;
        
            //Finds the closest hackable object
            foreach (Hackable hackable in FindObjectsByType<Hackable>(FindObjectsSortMode.None)) {

                //Needs to be on screen to be considered
                if (mainCamera.WorldToViewportPoint(hackable.transform.position).x > 0.97f || mainCamera.WorldToViewportPoint(hackable.transform.position).x < 0.03f ||
                    mainCamera.WorldToViewportPoint(hackable.transform.position).y > 0.97f || mainCamera.WorldToViewportPoint(hackable.transform.position).y < 0.03f) {
                    continue;
                }
            
                //Needs to free so it can be hacked
                if (hackable.beingHacked == true) {
                    continue;
                }

                //Makes a vector and gets its direction
                Vector3 MouseToHackableVector = hackable.transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);
                //bool direction = Convert.ToBoolean((Mathf.Sign(PlayerToHackableVector.x) + 1) / 2);
                //If within range and in the direction the player is facing
                if (MouseToHackableVector.magnitude < distance) {
                    //Debug.Log("Found hackable in range");
                    target = hackable;
                    distance = MouseToHackableVector.magnitude;
                }
            }
            if (target  != null) {
                reticle.SetActive(true);
                reticle.transform.position = target.transform.position;
            } else {
                reticle.SetActive(false);
            }
        }
    }

    public void OnHacking(InputAction.CallbackContext context) {
        if (target != null && !hasHacked && !menu.paused) {
            if (hackCharge >= 100f / hackCharges) {
                //If the target is currently being hacked dont let it
                target.OnHack();
                hasHacked = true;
                hackCharge -= 100f / hackCharges;
            } else {
                //Not enough charge
                Hack_Fail.Post(gameObject);
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
