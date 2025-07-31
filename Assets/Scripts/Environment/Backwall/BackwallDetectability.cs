using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwallDetectability : MonoBehaviour
{
    public bool playerVisible 
    {
        get
        {
            return collidersFiring>0;
        }
    }
    public int collidersFiring { get; set; }
}
