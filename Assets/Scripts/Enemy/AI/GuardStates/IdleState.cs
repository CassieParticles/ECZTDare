using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(GameObject guard) : base(guard)
    {
        camComp = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    Camera camComp;
    public override void Start()
    {

    }

    public override void Stop()
    {

    }

    public override GuardStates RunTick()
    {
        Vector3 viewPos = camComp.WorldToViewportPoint(guardAttached.transform.position);
        if (viewPos.x < -0.05 || viewPos.x > 1.05 || viewPos.y < -0.05 || viewPos.y > 1.05)
        {
            return GuardStates.Idle;
        }
        return GuardStates.Patrol;
    }


}
