using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeardNoiseState : BaseState
{
    public HeardNoiseState(GameObject guard) : base(guard)
    {
    }



    public override void Start()
    {

    }

    public override void Stop()
    {

    }

    public override GuardStates RunTick()
    {
        if (guardBehaviour.suspicionState == BaseEnemyBehaviour.SuspicionState.Chase)
        {
            return GuardStates.Chase;
        }
        else if (guardBehaviour.suspicionState == BaseEnemyBehaviour.SuspicionState.HighAlert)
        {
            return GuardStates.Investigate; //Enemy is on edge, investigate
        }
        else
        {
            return GuardStates.Observe;
        }
    }
}
