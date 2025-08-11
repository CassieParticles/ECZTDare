using System.Collections;
using UnityEngine;

public class StartUpState : BaseState
{
    Coroutine StartUp;
    float startupTime;
    bool finished;
    public StartUpState(GameObject guard, float startupTime) : base(guard)
    {
        Uninterruptable = true;
        this.startupTime = startupTime;
    }

    private IEnumerator Startup()
    {
        yield return new WaitForSeconds(startupTime);
        finished = true;
        StartUp = null;
    }

    public override void Start()
    {
        finished = false;
        StartUp = guardBehaviour.StartCoroutine(Startup());
        guardBehaviour.enemySight.gameObject.SetActive(false);
    }

    public override void Stop()
    {
        if (StartUp != null)
        {
            guardBehaviour.StopCoroutine(StartUp);
            StartUp = null;
        }
        guardBehaviour.guardMoveAnimation.SetBool("Awake", true);
    }

    public override GuardStates RunTick()
    {
        if(finished)
        {
            guardBehaviour.enemySight.gameObject.SetActive(true);
            return GuardStates.Patrol;
        }
        return GuardStates.StartUp;
    }
}
