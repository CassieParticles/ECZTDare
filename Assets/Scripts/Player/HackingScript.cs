using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

public class HackingScript: MonoBehaviour, IKeyboardWASDActions {

    PlayerControls controls;
    InputAction hackAction;

    public Hackable target;

    [SerializeField] float range = 10f;

    // Start is called before the first frame update
    void Start()
    {
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
            float thisDistance = (hackable.transform.position - transform.position).magnitude;
            if (thisDistance < distance) {
                target = hackable;
                distance = thisDistance;
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
