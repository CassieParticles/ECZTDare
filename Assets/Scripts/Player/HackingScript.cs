using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

public class HackingScript: MonoBehaviour, IGameplayControlsActions {

    public AK.Wwise.Event Hack_Fail;
    PlayerControls controls;
    InputAction hackAction;
    InputAction aimHackAction;
    bool hackInput;
    Vector2 aimHackInput;
    ControlsScript controlsScript;

    MovementScript movementScript;
    Camera mainCamera;
    GameObject reticle;

    Vector2 gamepadDirection = Vector2.zero;

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

        controlsScript = GameObject.Find("Menu Canvas").GetComponent<ControlsScript>();
        if (controlsScript != null) {
            controls = controlsScript.controls;
            controls.GameplayControls.SetCallbacks(this);
        } else {
            controls = new PlayerControls();
            controls.GameplayControls.SetCallbacks(this);
        }
        controls.GameplayControls.Enable();
        hackAction = controls.FindAction("Hacking");
        aimHackAction = controls.FindAction("AimHack");

        aimHackAction.performed += ctx => OnAimHack(ctx);
    }

    // Update is called once per frame
    void Update() {

        //aimHackInput = aimHackAction.ReadValue<Vector2>();
        
        //if (Mouse.current.wasUpdatedThisFrame) {
        //    usingMouse = true;
        //}



        if (!movementScript.InputLocked) {
            if (hackCharge + hackChargeRate * Time.deltaTime < 100f) {
                hackCharge += hackChargeRate * Time.deltaTime;
            }

            hackInput = hackAction.ReadValue<float>() > 0;
            if (!hackInput) {
                hasHacked = false;
            } else {
                Hack();
            }
            //hackInput = hackAction.ReadValue<float>() > 0;



            if (Cursor.visible || (target == null && !Cursor.visible)) {
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
                    Vector3 MouseToHackableVector = hackable.transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10));
                    //bool direction = Convert.ToBoolean((Mathf.Sign(PlayerToHackableVector.x) + 1) / 2);
                    //If within range and in the direction the player is facing
                    if (MouseToHackableVector.magnitude < distance && hackable.enabled) {
                        //Debug.Log("Found hackable in range");
                        target = hackable;
                        distance = MouseToHackableVector.magnitude;
                    }
                }
                
            } else {
                foreach (Hackable hackable in FindObjectsByType<Hackable>(FindObjectsSortMode.None)) {
                    //Needs to be on screen to be considered
                    if (mainCamera.WorldToViewportPoint(hackable.transform.position).x > 0.97f || mainCamera.WorldToViewportPoint(hackable.transform.position).x < 0.03f ||
                        mainCamera.WorldToViewportPoint(hackable.transform.position).y > 0.97f || mainCamera.WorldToViewportPoint(hackable.transform.position).y < 0.03f) {
                        continue;
                    }

                    if (hackable.GetComponent<HackableConsoleEnable>() != null) {
                        if (hackable.GetComponent<Hackable>().enabled) {
                            target = hackable;
                            break;
                        } else if (hackable == target) {
                            target = null;
                            break;
                        }
                    } else {
                        continue;
                    }
                }
            }

            if (target != null && target.GetComponent<HackableConsoleEnable>() != null && !target.GetComponent<HackableConsoleEnable>().enabled) {
                target = null;
            }

            if (target != null) {
                reticle.SetActive(true);
                reticle.transform.position = target.transform.position;
            } else {
                reticle.SetActive(false);
            }
        }
    }

    public void OnHacking(InputAction.CallbackContext context) {

    }

    public void Hack() {
        if (target != null && !hasHacked && !menu.paused) {
            if (hackCharge >= 100f / hackCharges) {
                //If the target is currently being hacked dont let it
                target.OnHack();
                hasHacked = true;
                hackCharge -= 100f / hackCharges;
                target = null;
            } else {
                //Not enough charge
                Hack_Fail.Post(gameObject);
            }
        } //else No target
    }
    public void OnAimHack(InputAction.CallbackContext context) {
        gamepadDirection = context.ReadValue<Vector2>();
        float smallestAngle = 180;

        foreach (Hackable hackable in FindObjectsByType<Hackable>(FindObjectsSortMode.None)) {
            
            //Needs to be on screen to be considered
            if (mainCamera.WorldToViewportPoint(hackable.transform.position).x > 0.97f || mainCamera.WorldToViewportPoint(hackable.transform.position).x < 0.03f ||
                mainCamera.WorldToViewportPoint(hackable.transform.position).y > 0.97f || mainCamera.WorldToViewportPoint(hackable.transform.position).y < 0.03f) {
                continue;
            }

            if (hackable.GetComponent<HackableConsoleEnable>() != null) {
                if (hackable.GetComponent<Hackable>().enabled) {
                    target = hackable;
                    smallestAngle = 0;
                    break;
                } else {
                    continue;
                }
            }

            //Needs to free so it can be hacked
            if (hackable.beingHacked == true) {
                continue;
            }

            Vector2 currentHackableVector = hackable.transform.position - transform.position;
            float currentHackableAngle = Vector2.Angle(gamepadDirection, currentHackableVector);
            if (currentHackableAngle < smallestAngle) {
                smallestAngle = currentHackableAngle;
                target = hackable;
            }
        }
    }
    public void OnCloaking(InputAction.CallbackContext context) {

    }

    public void OnDashing(InputAction.CallbackContext context) {

    }
    public void OnJumping(InputAction.CallbackContext context) {

    }
    public void OnRunning(InputAction.CallbackContext context) {

    }
    public void OnSliding(InputAction.CallbackContext context) {

    }

    public void OnPause(InputAction.CallbackContext context) {

    }

    public void OnReset(InputAction.CallbackContext context) {
    
    }
}
