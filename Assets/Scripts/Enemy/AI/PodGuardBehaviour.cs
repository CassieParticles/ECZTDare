using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodGuardBehaviour : GuardBehaviour
{
    private GuardPod pod;

    public void SetPod(GuardPod pod)
    {
        this.pod = pod;
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
    }
}
