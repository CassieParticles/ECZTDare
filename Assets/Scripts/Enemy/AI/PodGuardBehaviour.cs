using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodGuardBehaviour : GuardBehaviour
{

    [SerializeField] private float startupTime=3.0f;
    private GuardPod pod;

    public void StartGuard()
    {
        guardMoveAnimation.SetBool("Awake", false);
        guardBehaviour.MoveToState(GuardStates.StartUp);

    }

    public void ReturnToPod()
    {
        guardBehaviour.MoveToState(GuardStates.ReturnToPod);
    }

    protected new void Awake()
    {
        base.Awake();
        pod = GetComponentInParent<GuardPod>();

        //Add return to pod behaviour
        guardBehaviour.AddState(GuardStates.StartUp, new StartUpState(gameObject, startupTime));
        guardBehaviour.AddState(GuardStates.ReturnToPod, new ReturnToPodState(gameObject,pod));
    }
}
