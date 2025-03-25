using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

public class SettingsButtonScript : MonoBehaviour {
    PlayerControls.GameplayControlsActions controls;
    public AK.Wwise.Event buttonClick;

    public enum Controls {
        RunningLeft,
        RunningRight,
        Jumping,
        Sliding,
        BoostCloaking,
        Hacking,
    }

    public void Awake() {
        controls = GameObject.Find("PlayerControls").GetComponent<PlayerControls.GameplayControlsActions>();
    }

    public void RemapInput(Controls reboundAction) {
        buttonClick.Post(gameObject);
        InputActionRebindingExtensions.RebindingOperation rebinder;
        switch (reboundAction) {
            case Controls.Jumping:
                rebinder = controls.Jumping.PerformInteractiveRebinding().WithControlsExcluding("Mouse").Start();
                return;
            case Controls.Sliding:
                rebinder = controls.Sliding.PerformInteractiveRebinding().WithControlsExcluding("Mouse").Start();
                return;
            case Controls.BoostCloaking:
                rebinder = controls.BoostCloak.PerformInteractiveRebinding().WithControlsExcluding("Mouse").Start();
                return;
            case Controls.Hacking:
                rebinder = controls.Hacking.PerformInteractiveRebinding().WithControlsExcluding("Mouse").Start();
                return;
        }
    }

    public void ResetInput(Controls reboundAction) {
        buttonClick.Post(gameObject);
        switch (reboundAction) {
            case Controls.Jumping:
                controls.Jumping.RemoveAllBindingOverrides();
                return;
            case Controls.Sliding:
                controls.Sliding.RemoveAllBindingOverrides();
                return;
            case Controls.BoostCloaking:
                controls.BoostCloak.RemoveAllBindingOverrides();
                return;
            case Controls.Hacking:
                controls.Hacking.RemoveAllBindingOverrides();
                return;
        }
    }








}
