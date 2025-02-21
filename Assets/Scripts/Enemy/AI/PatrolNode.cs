using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolNode : MonoBehaviour
{
    [SerializeField,Range(0,10)] private float delayTime;

    public float getDelay()
    {
        return delayTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(delayTime == 0)
        {
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
        else
        {
            Gizmos.DrawCube(transform.position, new Vector3(0.2f, 0.2f, 0.2f));
        }
        
    }
}
