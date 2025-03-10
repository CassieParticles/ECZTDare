using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hackable : MonoBehaviour
{
    public bool beingHacked;
    public abstract void OnHack();
}
