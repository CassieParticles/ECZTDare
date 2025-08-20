using JetBrains.Annotations;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static PlayerControls;

public class ControlsScript : MonoBehaviour {
    public PlayerControls controls;
    public PlayerInput playerInput;
    public AK.Wwise.Event buttonClick;

    MenuScript menu;

    public GameObject rebindLeftButton;
    public GameObject rebindRightButton;
    public GameObject rebindJumpButton;
    public GameObject rebindSlideButton;
    public GameObject rebindBoostCloakButton;
    public GameObject rebindHackButton;

    TextMeshProUGUI rebindLeftButtonKey;
    TextMeshProUGUI rebindRightButtonKey;
    TextMeshProUGUI rebindJumpButtonKey;
    TextMeshProUGUI rebindSlideButtonKey;
    TextMeshProUGUI rebindBoostCloakButtonKey;
    TextMeshProUGUI rebindHackButtonKey;

    public GameObject resetRunButton;
    public GameObject resetJumpButton;
    public GameObject resetSlideButton;
    public GameObject resetBoostCloakButton;
    public GameObject resetHackButton;

    public bool[] overrides = new bool[3];

    public enum Controls {
        RunningLeft,
        RunningRight,
        Jumping,
        Sliding,
        Dash,
        Cloak,
        Hacking,
        Pause
    }

    public string GetBoundControl(Controls control)
    {
        if(playerInput.currentControlScheme == "KeyboardMouse")
        {
            return getKeyboardControl(control);
        }
        else
        {
            return getControllerControl(control);
        }
    }

    private string getKeyboardControl(Controls control)
    {
        switch (control)
        {
            case Controls.RunningLeft:
                return controls.GameplayControls.Running.bindings[1].ToDisplayString();
            case Controls.RunningRight:
                return controls.GameplayControls.Running.bindings[2].ToDisplayString();
            case Controls.Jumping:
                return controls.GameplayControls.Jumping.bindings[0].ToDisplayString();
            case Controls.Sliding:
                return controls.GameplayControls.Sliding.bindings[0].ToDisplayString();
            case Controls.Dash:
                return controls.GameplayControls.Dashing.bindings[0].ToDisplayString();
            case Controls.Cloak:
                return controls.GameplayControls.Cloaking.bindings[0].ToDisplayString();
            case Controls.Hacking:
                return controls.GameplayControls.Hacking.bindings[0].ToDisplayString();
            case Controls.Pause:
                return "ESC";
            default:    //Never hit, unless new controls are added
                Debug.LogWarning("WARNING: UNKNOWN KEY, PLEASE UPDATE WITH NEW BINDING");
                return null;
        }
    }

    private string getControllerControl(Controls control)
    {
        switch (control)
        {
            case Controls.RunningLeft:
                return controls.GameplayControls.Running.bindings[4].ToDisplayString();
            case Controls.RunningRight:
                return controls.GameplayControls.Running.bindings[5].ToDisplayString();
            case Controls.Jumping:
                return controls.GameplayControls.Jumping.bindings[1].ToDisplayString();
            case Controls.Sliding:
                return controls.GameplayControls.Sliding.bindings[1].ToDisplayString();
            case Controls.Dash:
                return controls.GameplayControls.Dashing.bindings[1].ToDisplayString();
            case Controls.Cloak:
                return controls.GameplayControls.Cloaking.bindings[1].ToDisplayString();
            case Controls.Hacking:
                return controls.GameplayControls.Hacking.bindings[1].ToDisplayString();
            case Controls.Pause:
                return "PAUSE";
            default:    //Never hit, unless new controls are added
                Debug.LogWarning("WARNING: UNKNOWN KEY, PLEASE UPDATE WITH NEW BINDING");
                return null;
        }
    }

    public void Setup() {
        controls = new PlayerControls();
        menu = GetComponent<MenuScript>();
        playerInput = GetComponent<PlayerInput>();

        

        rebindLeftButtonKey = rebindLeftButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindRightButtonKey = rebindRightButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindJumpButtonKey = rebindJumpButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindSlideButtonKey = rebindSlideButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindBoostCloakButtonKey = rebindBoostCloakButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindHackButtonKey = rebindHackButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

    }

    public void Update() {
        if (playerInput.currentControlScheme == "KeyboardMouse") {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void RemapInput(string reboundAction) {
        buttonClick.Post(gameObject);
        InputActionRebindingExtensions.RebindingOperation rebinder;
        int runIndexModifier = 1;
        if (playerInput.currentControlScheme == "Gamepad") {
            runIndexModifier = 4;
        }
        switch (reboundAction) {
            case "RunningLeft":
                rebinder = controls.GameplayControls.Running.PerformInteractiveRebinding(runIndexModifier).
                    WithControlsExcluding("<Gamepad>/leftStick/right").
                    WithControlsExcluding("<Gamepad>/dpad/right").
                    WithControlsExcluding("<Gamepad>/dpad/down").
                    WithControlsExcluding("<Gamepad>/dpad/x").
                    WithControlsExcluding("<Gamepad>/dpad/y").
                    WithControlsExcluding("<Gamepad>/rightStick").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Keyboard>/escape").
                    WithControlsExcluding("<Keyboard>/r").
                    WithControlsExcluding("<Keyboard>/anyKey").
                    WithCancelingThrough("<Keyboard>/escape").
                    WithCancelingThrough("<Gamepad>/start").
                    Start();
                return;
            case "RunningRight":
                rebinder = controls.GameplayControls.Running.PerformInteractiveRebinding(runIndexModifier + 1).
                    WithControlsExcluding("<Gamepad>/leftStick/left").
                    WithControlsExcluding("<Gamepad>/dpad/left").
                    WithControlsExcluding("<Gamepad>/dpad/down").
                    WithControlsExcluding("<Gamepad>/dpad/x").
                    WithControlsExcluding("<Gamepad>/dpad/y").
                    WithControlsExcluding("<Gamepad>/rightStick").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Keyboard>/escape").
                    WithControlsExcluding("<Keyboard>/r").
                    WithControlsExcluding("<Keyboard>/anyKey").
                    WithCancelingThrough("<Keyboard>/escape").
                    WithCancelingThrough("<Gamepad>/start").
                    Start();
                return;
            case "Jumping":
                rebinder = controls.GameplayControls.Jumping.PerformInteractiveRebinding().
                    WithControlsExcluding("<Gamepad>/leftStick/left").
                    WithControlsExcluding("<Gamepad>/leftStick/right").
                    WithControlsExcluding("<Gamepad>/rightStick").
                    WithControlsExcluding("<Gamepad>/dpad/down").
                    WithControlsExcluding("<Gamepad>/dpad/left").
                    WithControlsExcluding("<Gamepad>/dpad/right").
                    WithControlsExcluding("<Gamepad>/dpad/x").
                    WithControlsExcluding("<Gamepad>/dpad/y").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Keyboard>/escape").
                    WithControlsExcluding("<Keyboard>/r").
                    WithControlsExcluding("<Keyboard>/anyKey").
                    WithCancelingThrough("<Keyboard>/escape").
                    WithCancelingThrough("<Gamepad>/start").
                    Start();
                return;
            case "Sliding":
                rebinder = controls.GameplayControls.Sliding.PerformInteractiveRebinding().
                    WithControlsExcluding("<Gamepad>/leftStick/left").
                    WithControlsExcluding("<Gamepad>/leftStick/right").
                    WithControlsExcluding("<Gamepad>/rightStick").
                    WithControlsExcluding("<Gamepad>/dpad/left").
                    WithControlsExcluding("<Gamepad>/dpad/right").
                    WithControlsExcluding("<Gamepad>/dpad/x").
                    WithControlsExcluding("<Gamepad>/dpad/y").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Keyboard>/escape").
                    WithControlsExcluding("<Keyboard>/r").
                    WithControlsExcluding("<Keyboard>/anyKey").
                    WithCancelingThrough("<Keyboard>/escape").
                    WithCancelingThrough("<Gamepad>/start").
                    Start();
                return;
            case "Dashing":
                rebinder = controls.GameplayControls.Dashing.PerformInteractiveRebinding().
                    WithControlsExcluding("<Gamepad>/leftStick/left").
                    WithControlsExcluding("<Gamepad>/leftStick/right").
                    WithControlsExcluding("<Gamepad>/rightStick").
                    WithControlsExcluding("<Gamepad>/dpad/down").
                    WithControlsExcluding("<Gamepad>/dpad/left").
                    WithControlsExcluding("<Gamepad>/dpad/right").
                    WithControlsExcluding("<Gamepad>/dpad/x").
                    WithControlsExcluding("<Gamepad>/dpad/y").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Keyboard>/escape").
                    WithControlsExcluding("<Keyboard>/r").
                    WithControlsExcluding("<Keyboard>/anyKey").
                    WithCancelingThrough("<Keyboard>/escape").
                    WithCancelingThrough("<Gamepad>/start").
                    Start();
                return;
            case "Cloaking":
                rebinder = controls.GameplayControls.Cloaking.PerformInteractiveRebinding().
                    WithControlsExcluding("<Gamepad>/leftStick/left").
                    WithControlsExcluding("<Gamepad>/leftStick/right").
                    WithControlsExcluding("<Gamepad>/rightStick").
                    WithControlsExcluding("<Gamepad>/dpad/down").
                    WithControlsExcluding("<Gamepad>/dpad/left").
                    WithControlsExcluding("<Gamepad>/dpad/right").
                    WithControlsExcluding("<Gamepad>/dpad/x").
                    WithControlsExcluding("<Gamepad>/dpad/y").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Keyboard>/escape").
                    WithControlsExcluding("<Keyboard>/r").
                    WithControlsExcluding("<Keyboard>/anyKey").
                    WithCancelingThrough("<Keyboard>/escape").
                    WithCancelingThrough("<Gamepad>/start").
                    Start();
                return;
            case "Hacking":
                rebinder = controls.GameplayControls.Hacking.PerformInteractiveRebinding().
                    WithControlsExcluding("<Gamepad>/leftStick/left").
                    WithControlsExcluding("<Gamepad>/leftStick/right").
                    WithControlsExcluding("<Gamepad>/rightStick").
                    WithControlsExcluding("<Gamepad>/dpad/down").
                    WithControlsExcluding("<Gamepad>/dpad/left").
                    WithControlsExcluding("<Gamepad>/dpad/right").
                    WithControlsExcluding("<Gamepad>/dpad/x").
                    WithControlsExcluding("<Gamepad>/dpad/y").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Keyboard>/escape").
                    WithControlsExcluding("<Keyboard>/r").
                    WithControlsExcluding("<Keyboard>/anyKey").
                    WithCancelingThrough("<Keyboard>/escape").
                    WithCancelingThrough("<Gamepad>/start").
                    Start();
                return;
        }
    }

    public void ResetInput(string reboundAction) {
        buttonClick.Post(gameObject);
        switch (reboundAction) {
            case "Running":
                controls.GameplayControls.Running.RemoveAllBindingOverrides();
                rebindLeftButton.GetComponent<Button>().Select();
                return;
            case "Jumping":
                controls.GameplayControls.Jumping.RemoveAllBindingOverrides();
                rebindJumpButton.GetComponent<Button>().Select();
                return;
            case "Sliding":
                controls.GameplayControls.Sliding.RemoveAllBindingOverrides();
                rebindSlideButton.GetComponent<Button>().Select();
                return;
            case "Dashing":
                controls.GameplayControls.Dashing.RemoveAllBindingOverrides();
                rebindBoostCloakButton.GetComponent<Button>().Select();
                return;
            case "Cloaking":
                controls.GameplayControls.Cloaking.RemoveAllBindingOverrides();
                rebindBoostCloakButton.GetComponent<Button>().Select();
                return;
            case "Hacking":
                controls.GameplayControls.Hacking.RemoveAllBindingOverrides();
                rebindHackButton.GetComponent<Button>().Select();
                return;
        }
    }
}
