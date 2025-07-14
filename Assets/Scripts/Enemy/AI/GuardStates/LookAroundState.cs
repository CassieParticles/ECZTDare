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
        //Guard sees player
        if(guardBehaviour.Player)
        {
            //Get a line from the guard to the player, and check for intersection
            Vector2 playerDirection = guardBehaviour.Player.transform.position - guardBehaviour.enemySight.transform.position;
            RaycastHit2D rayHit = Physics2D.Raycast(guardAttached.transform.position, playerDirection, playerDirection.magnitude, 0b0110011);

            return GuardStates.Observe;

        }

        //Exit once finished looking
        if (finishedLooking){ lookAround = null; return GuardStates.Patrol; }

        //Continue looking
        return GuardStates.LookAround;
    }
}
