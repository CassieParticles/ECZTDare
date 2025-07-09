using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static PlayerControls;

public class ControlsScript : MonoBehaviour {
    public PlayerControls controls;
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

    public enum Controls {
        RunningLeft,
        RunningRight,
        Jumping,
        Sliding,
        BoostCloaking,
        Hacking,
    }

    public void Awake() {
        //controls = GameObject.Find("PlayerControls").GetComponent<PlayerControls.GameplayControlsActions>();
        controls = new PlayerControls();
        menu = GetComponent<MenuScript>();

        //rebindLeftButton = GameObject.Find("RebindLeftButton");
        //rebindRightButton = GameObject.Find("RebindRightButton");
        //rebindJumpButton = GameObject.Find("RebindJumpButton");
        //rebindSlideButton = GameObject.Find("RebindSlideButton");
        //rebindBoostCloakButton = GameObject.Find("RebindBoostCloakButton");
        //rebindHackButton = GameObject.Find("RebindHackButton");

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

        //resetRunButton = GameObject.Find("ResetRunButton");
        //resetJumpButton = GameObject.Find("ResetJumpButton");
        //resetSlideButton = GameObject.Find("ResetSlideButton");
        //resetBoostCloakButton = GameObject.Find("ResetBoostCloakButton");
        //resetHackButton = GameObject.Find("ResetHackButton");

    }

    public void Update() { //For some stupid reason this update function doesnt run specifically in build mode I hate it :(
        //if (menu.keybindsOpen) {
            //rebindLeftButtonKey.text = controls.GameplayControls.Running.bindings[1].ToDisplayString();
            //rebindRightButtonKey.text = controls.GameplayControls.Running.bindings[2].ToDisplayString();
            //rebindJumpButtonKey.text = controls.GameplayControls.Jumping.bindings[0].ToDisplayString();
            //rebindSlideButtonKey.text = controls.GameplayControls.Sliding.bindings[0].ToDisplayString();
            //rebindBoostCloakButtonKey.text = controls.GameplayControls.BoostCloak.bindings[0].ToDisplayString();
            //rebindHackButtonKey.text = controls.GameplayControls.Hacking.bindings[0].ToDisplayString();

            //rebindSlideButtonKey.text = "Sliding hehhehehee";
            /*
            if (controls.GameplayControls.Running.bindings[1].hasOverrides || controls.GameplayControls.Running.bindings[2].hasOverrides) {
                resetRunButton.SetActive(true);
            } else {
                resetRunButton.SetActive(false);
            }
            if (controls.GameplayControls.Jumping.bindings[0].hasOverrides) {
                resetJumpButton.SetActive(true);
            } else {
                resetJumpButton.SetActive(false);
            }
            if (controls.GameplayControls.Sliding.bindings[0].hasOverrides) {
                resetSlideButton.SetActive(true);
            } else {
                resetSlideButton.SetActive(false);
            }
            if (controls.GameplayControls.BoostCloak.bindings[0].hasOverrides) {
                resetBoostCloakButton.SetActive(true);
            } else {
                resetBoostCloakButton.SetActive(false);
            }
            if (controls.GameplayControls.Hacking.bindings[0].hasOverrides) {
                resetHackButton.SetActive(true);
            } else {
                resetHackButton.SetActive(false);
            }
            */
        //}
    }
    public void RemapInput(string reboundAction) {
        buttonClick.Post(gameObject);
        InputActionRebindingExtensions.RebindingOperation rebinder;
        switch (reboundAction) {
            case "RunningLeft":
                rebinder = controls.GameplayControls.Running.PerformInteractiveRebinding(1).Start();
                return;
            case "RunningRight":
                rebinder = controls.GameplayControls.Running.PerformInteractiveRebinding(2).Start();
                return;
            case "Jumping":
                rebinder = controls.GameplayControls.Jumping.PerformInteractiveRebinding().Start();
                return;
            case "Sliding":
                rebinder = controls.GameplayControls.Sliding.PerformInteractiveRebinding().Start();
                return;
            case "Dashing":
                rebinder = controls.GameplayControls.Dashing.PerformInteractiveRebinding().Start();
                return;
            case "Hacking":
                rebinder = controls.GameplayControls.Hacking.PerformInteractiveRebinding().Start();
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
            case "Hacking":
                controls.GameplayControls.Hacking.RemoveAllBindingOverrides();
                rebindHackButton.GetComponent<Button>().Select();
                return;
        }
    }

    






}
