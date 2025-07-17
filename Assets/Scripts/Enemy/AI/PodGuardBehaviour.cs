using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodGuardBehaviour : GuardBehaviour
{

    [SerializeField] private float startupTime=3.0f;
    private GuardPod pod;

    public void SetPod(GuardPod pod)
    {
        this.pod = pod;
    }

    public void StartGuard()
    {
        guardBehaviour.MoveToState(GuardStates.StartUp);
    }

    public void ReturnToPod()
    {
        guardBehaviour.MoveToState(GuardStates.ReturnToPod);
    }

    public void Reset()
    {
        
    }

    protected new void Start()
    {
        base.Start();

        //Add return to pod behaviour
        guardBehaviour.AddState(GuardStates.ReturnToPod, new ReturnToPodState(gameObject,pod));
        guardBehaviour.AddState(GuardStates.StartUp, new StartUpState(gameObject, startupTime));
    }
}
