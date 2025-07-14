using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : EnemySight
{
    public override float calcSuspicionIncreaseRate(GameObject player)
    {
        return 0.0f;
    }

    //Detection area doesn't move
    public override void LookAt(Vector3 position)
    {
        return;
    }

    //Fill in
    public override void UpdateVisual()
    {
        return;
    }
}
