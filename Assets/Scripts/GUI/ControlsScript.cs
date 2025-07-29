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
        BoostCloaking,
        Hacking,
    }

    public void Setup() {
        //controls = GameObject.Find("PlayerControls").GetComponent<PlayerControls.GameplayControlsActions>();
        controls = new PlayerControls();
        menu = GetComponent<MenuScript>();
        playerInput = GetComponent<PlayerInput>();
        //kTransform = transform.Find("KeybindsGroup");

        //rebindLeftButton = kTransform.Find("RebindLeftButton").gameObject;
        //rebindRightButton = kTransform.Find("RebindRightButton").gameObject;
        //rebindJumpButton = kTransform.Find("RebindJumpButton").gameObject;
        //rebindSlideButton = kTransform.Find("RebindSlideButton").gameObject;
        //rebindBoostCloakButton = kTransform.Find("RebindBoostCloakButton").gameObject;
        //rebindHackButton = kTransform.Find("RebindHackButton").gameObject;

        rebindLeftButtonKey = rebindLeftButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindRightButtonKey = rebindRightButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindJumpButtonKey = rebindJumpButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindSlideButtonKey = rebindSlideButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindBoostCloakButtonKey = rebindBoostCloakButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rebindHackButtonKey = rebindHackButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        //rebindLeftButtonKey = GameObject.Find("RebindLeftKey").GetComponent<TextMeshProUGUI>();
        //rebindRightButtonKey = GameObject.Find("RebindRightKey").GetComponent<TextMeshProUGUI>();
        //rebindJumpButtonKey = GameObject.Find("RebindJumpKey").GetComponent<TextMeshProUGUI>();
        //rebindSlideButtonKey = GameObject.Find("RebindSlideKey").GetComponent<TextMeshProUGUI>();
        //rebindBoostCloakButtonKey = GameObject.Find("RebindBoostCloakKey").GetComponent<TextMeshProUGUI>();
        //rebindHackButtonKey = GameObject.Find("RebindHackKey").GetComponent<TextMeshProUGUI>();

        //resetRunButton = kTransform.Find("ResetRunButton").gameObject;
        //resetJumpButton = kTransform.Find("ResetJumpButton").gameObject;
        //resetSlideButton = kTransform.Find("ResetSlideButton").gameObject;
        //resetBoostCloakButton = kTransform.Find("ResetBoostCloakButton").gameObject;
        //resetHackButton = kTransform.Find("ResetHackButton").gameObject;

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
