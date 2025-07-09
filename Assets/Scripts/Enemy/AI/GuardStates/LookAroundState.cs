using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundState : BaseState
{

    private IEnumerator LookAround()
    {
        for(int i=0;i<lookAngles.Length;i++)
        {
            guardBehaviour.Look(lookAngles[i]);
            yield return new WaitForSeconds(2.0f);
        }
        finishedLooking = true;
    }
    Coroutine lookAround;

    float[] lookAngles = new float[3];
    bool finishedLooking;
    public LookAroundState(GameObject guard) : base(guard)
    {
    }

    public override void Start()
    {
        guardBehaviour.StopMoving();
        //Generate the random angles to look in
        for (int i = 0; i < lookAngles.Length; i++)
        {
            lookAngles[i] = Random.Range(0.0f, 180.0f);
        }
        finishedLooking = false;

        lookAround = guardBehaviour.StartCoroutine(LookAround());
    }

    public override void Stop()
    {
        if(lookAround!=null)
        {
            guardBehaviour.StopCoroutine(lookAround);
        }
    }

    public override GuardStates RunTick()
    {
        

        //Exit once finished looking
        if (finishedLooking){ lookAround = null; return GuardStates.Patrol; }

        //Continue looking
        return GuardStates.LookAround;
    }
}
