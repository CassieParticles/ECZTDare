using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPodState : BaseState
{
    private GuardPod pod;
    public ReturnToPodState(GameObject guard, GuardPod pod) : base(guard)
    {
        this.pod = pod;
    }

    public override void Start()
    {
        guardBehaviour.MoveTo(pod.transform.position);
    }

    public override void Stop()
    {
        
    }
    public override GuardStates RunTick()
    {
        //If guard is at destination
        if(guardBehaviour.getDistLeft() < 0.1f)
        {
            return GuardStates.Idle;
        }
        return GuardStates.ReturnToPod;
    }

}
