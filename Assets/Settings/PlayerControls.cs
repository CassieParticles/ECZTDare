//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Settings/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay Controls"",
            ""id"": ""74c0dcec-bbb9-4bd3-8303-a2926d0238ae"",
            ""actions"": [
                {
                    ""name"": ""Running"",
                    ""type"": ""Value"",
                    ""id"": ""e22b3110-135c-4db7-86f4-ce419d8d014f"",
                    ""expectedControlType"": ""Double"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jumping"",
                    ""type"": ""Button"",
                    ""id"": ""0034a420-c116-4424-a006-5c0960accdd4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sliding"",
                    ""type"": ""Button"",
                    ""id"": ""00a14053-f6ca-4d6e-8a86-f7b7162fdc97"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BoostCloak"",
                    ""type"": ""Button"",
                    ""id"": ""b8836efe-f0ac-4a8c-8aa2-44c79320d10e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hacking"",
                    ""type"": ""Button"",
                    ""id"": ""0a3f9b72-8ca2-4c9f-99d1-940ae8a2132b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""AD"",
                    ""id"": ""55cf8c4c-9bda-4749-ad1b-75de81b569a8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Running"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""c409a018-b89c-49fd-9ece-bfae0b6f9be3"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Running"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""bc810921-71ad-4bfa-956a-345852629d8d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Running"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8670e16d-60a8-4c50-8780-f6349d3b9e15"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jumping"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e624fdd0-c99c-4baf-9790-7ba3a3d95070"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sliding"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49f45dda-8f7c-45ea-ad35-64f5b8b288f4"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BoostCloak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c60c6067-75bd-4128-8754-2d7a0aba5b17"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hacking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay Controls
        m_GameplayControls = asset.FindActionMap("Gameplay Controls", throwIfNotFound: true);
        m_GameplayControls_Running = m_GameplayControls.FindAction("Running", throwIfNotFound: true);
        m_GameplayControls_Jumping = m_GameplayControls.FindAction("Jumping", throwIfNotFound: true);
        m_GameplayControls_Sliding = m_GameplayControls.FindAction("Sliding", throwIfNotFound: true);
        m_GameplayControls_BoostCloak = m_GameplayControls.FindAction("BoostCloak", throwIfNotFound: true);
        m_GameplayControls_Hacking = m_GameplayControls.FindAction("Hacking", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay Controls
    private readonly InputActionMap m_GameplayControls;
    private List<IGameplayControlsActions> m_GameplayControlsActionsCallbackInterfaces = new List<IGameplayControlsActions>();
    private readonly InputAction m_GameplayControls_Running;
    private readonly InputAction m_GameplayControls_Jumping;
    private readonly InputAction m_GameplayControls_Sliding;
    private readonly InputAction m_GameplayControls_BoostCloak;
    private readonly InputAction m_GameplayControls_Hacking;
    public struct GameplayControlsActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayControlsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Running => m_Wrapper.m_GameplayControls_Running;
        public InputAction @Jumping => m_Wrapper.m_GameplayControls_Jumping;
        public InputAction @Sliding => m_Wrapper.m_GameplayControls_Sliding;
        public InputAction @BoostCloak => m_Wrapper.m_GameplayControls_BoostCloak;
        public InputAction @Hacking => m_Wrapper.m_GameplayControls_Hacking;
        public InputActionMap Get() { return m_Wrapper.m_GameplayControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayControlsActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayControlsActionsCallbackInterfaces.Add(instance);
            @Running.started += instance.OnRunning;
            @Running.performed += instance.OnRunning;
            @Running.canceled += instance.OnRunning;
            @Jumping.started += instance.OnJumping;
            @Jumping.performed += instance.OnJumping;
            @Jumping.canceled += instance.OnJumping;
            @Sliding.started += instance.OnSliding;
            @Sliding.performed += instance.OnSliding;
            @Sliding.canceled += instance.OnSliding;
            @BoostCloak.started += instance.OnBoostCloak;
            @BoostCloak.performed += instance.OnBoostCloak;
            @BoostCloak.canceled += instance.OnBoostCloak;
            @Hacking.started += instance.OnHacking;
            @Hacking.performed += instance.OnHacking;
            @Hacking.canceled += instance.OnHacking;
        }

        private void UnregisterCallbacks(IGameplayControlsActions instance)
        {
            @Running.started -= instance.OnRunning;
            @Running.performed -= instance.OnRunning;
            @Running.canceled -= instance.OnRunning;
            @Jumping.started -= instance.OnJumping;
            @Jumping.performed -= instance.OnJumping;
            @Jumping.canceled -= instance.OnJumping;
            @Sliding.started -= instance.OnSliding;
            @Sliding.performed -= instance.OnSliding;
            @Sliding.canceled -= instance.OnSliding;
            @BoostCloak.started -= instance.OnBoostCloak;
            @BoostCloak.performed -= instance.OnBoostCloak;
            @BoostCloak.canceled -= instance.OnBoostCloak;
            @Hacking.started -= instance.OnHacking;
            @Hacking.performed -= instance.OnHacking;
            @Hacking.canceled -= instance.OnHacking;
        }

        public void RemoveCallbacks(IGameplayControlsActions instance)
        {
            if (m_Wrapper.m_GameplayControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayControlsActions @GameplayControls => new GameplayControlsActions(this);
    public interface IGameplayControlsActions
    {
        void OnRunning(InputAction.CallbackContext context);
        void OnJumping(InputAction.CallbackContext context);
        void OnSliding(InputAction.CallbackContext context);
        void OnBoostCloak(InputAction.CallbackContext context);
        void OnHacking(InputAction.CallbackContext context);
    }
}
